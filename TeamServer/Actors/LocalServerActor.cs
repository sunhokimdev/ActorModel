using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamLibrary;
using static TeamLibrary.LocalActorMessage;
using static TeamLibrary.Message.ConfiguratorActorMessage;
using static TeamLibrary.ServerActorMessage;

namespace TeamServer
{
	public class LocalManageMentData
	{
		public IActorRef MyLocalActor { get; set; }
		public LocalData RealTimeLocalData { get; set; }
		public string LocalName { get; set; }
		public bool IsTerminated { get; set; }
	}

	public class LocalServerActor : ReceiveActor
	{
		private readonly ILoggingAdapter _logger;
		string recipe;
		Dictionary<int, LocalManageMentData> _localManager;
		Stack<int> _deletedLocalIDStock;
		int _localCount = 0;

		public LocalServerActor()
		{
			_logger = Context.GetLogger();
			_localManager = new Dictionary<int, LocalManageMentData>();
			_deletedLocalIDStock = new Stack<int>();
			FileInfo fi = new FileInfo(@"../../Recipe/Recipe.txt");
			if (fi.Exists)
			{
				StreamReader RecipeRead = File.OpenText(@"../../Recipe/Recipe.txt");
				recipe = RecipeRead.ReadToEnd();
				Console.WriteLine(recipe);
				RecipeRead.Close();
			}
			
			Receive<SubscribeMonitorLocal>(_ => Handle(_));
			Receive<MessageOfRequestLocalIDs>(_ => Handle(_));
			Receive<UpdateMonitor>(_ => Handle(_));
			Receive<UnsubscribeMonitorLocal>(_ => Handle(_));
			Receive<UpdateRecipe>(_ => Handle(_));
		}
		public static Props Props()
		{
			return Akka.Actor.Props.Create(() => new LocalServerActor());
		}
		public void Handle(SubscribeMonitorLocal createdLocalInfo)
		{
			int assignedLocalID = _localCount;

			if (_deletedLocalIDStock.Count != 0)
			{
				assignedLocalID = _deletedLocalIDStock.Pop();

				if (_localCount == assignedLocalID)
					_localCount--;
			}
			else
				_localCount++;
			/*
             * Dictionary에서 데이터가 존재하지 않으면 비교가 안될 수 있다.
             * 데이터를 가지고 있는지 []Operator에서 확인하고 있는 거 같음
             */
			try
			{
				_localManager[assignedLocalID] =
							   new LocalManageMentData()
							   {
								   MyLocalActor = createdLocalInfo.ActorOfLocal,
								   LocalName = createdLocalInfo.LocalName,
								   RealTimeLocalData = new LocalData(),
								   IsTerminated = false,
							   };

				Context.Parent.Tell(new MessageOfListViewUpdateLocalList(assignedLocalID, createdLocalInfo.LocalName)
				{});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			createdLocalInfo.ActorOfLocal.Tell(new MonitorLocalSubscribed(assignedLocalID, recipe));
		}
		// 로컬의 ID가 필요로 하다면
		public void Handle(MessageOfRequestLocalIDs requestedLocalID)
		{
			for (int i = 0; i < _localCount; i++)
			{
				// 삭제된 명단에 없다면 해당 로컬 아이디, 이름을 넘겨준다.
				if (!_deletedLocalIDStock.Contains(i))
				{
					Context.Parent.Tell(new MessageOfReturnLocalID(requestedLocalID.AssignedClientID, i, _localManager[i].LocalName)
					{});
				}
			}
		}
		// 로컬에서 데이터를 계속 보내주면 필요로 하는 데이터를 클라이언트에 보내준다.
		public void Handle(UpdateMonitor localData)
		{
			// 로컬 데이터를 계속해서 받는다.
			_localManager[localData.LocalData.LocalID].RealTimeLocalData =
				localData.LocalData;
            
			foreach (var obj in _localManager)
			{
				Context.Parent.Tell(new UpdateMonitor(obj.Value.RealTimeLocalData));
			}
		}
		public void Handle(UnsubscribeMonitorLocal localID)
		{
			if (localID.ID == _localCount)
			{
				_localCount--;
			}

			_localManager[localID.ID].IsTerminated = true;
			_deletedLocalIDStock.Push(localID.ID);

			Context.Parent.Tell(new UnsubscribeMonitorLocal(localID.ID));
		}

		public void Handle(UpdateRecipe recipeData)
		{
			FileInfo fi = new FileInfo(@"../../Recipe/Recipe.txt");
			if (fi.Exists)
			{
				StreamReader RecipeRead = File.OpenText(@"../../Recipe/Recipe.txt");
				string compareRecipe = RecipeRead.ReadToEnd();
				RecipeRead.Close();
				if (recipe != recipeData.Recipe)
				{
					recipe = recipeData.Recipe;
					Console.WriteLine("Replace the recipe.");
					StreamWriter sw = new StreamWriter(@"../../Recipe/Recipe.txt");
					sw.Write(recipe);

					for(int i=0;i<_localCount;i++)
					{
						if(!_localManager[i].IsTerminated)
						{
							_logger.Info("Send updated recipes");
							_localManager[i].MyLocalActor.Tell(new RestartMonitor(recipe));
						}
					}

					sw.Close(); 
				}
				else
				{
					Console.WriteLine("The recipe is the same.");
				}
			}
			Sender.Tell(new RecipeUpdated());
		}
	}
}
