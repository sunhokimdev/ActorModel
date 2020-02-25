using Akka.Actor;
using Akka.Event;
using Microsoft.VisualBasic.Devices;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TeamLibrary;
using static TeamLibrary.LocalActorMessage;
using static TeamLibrary.ServerActorMessage;

namespace TeamLocal.Script
{
	public sealed class LocalActor : ReceiveActor
	{
		#region Messages
		public class RetryToSubscribeMonitorLocal {}

		public class Monitoring {}
		#endregion

		private readonly ILoggingAdapter _logger;
		private int memCounter;
		private bool serverAccessState;
		private int _localID;
		private PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		private IActorRef server;
		private string serverPath;
		private ICancelable scheduler;
		private ICancelable nonScheduler;
		private string _localName;
		private string recipe;
		private int monitoringTime;
		AppSettingsReader appSettings;
		private bool recipeRead = false;

		public static Props Props(AppSettingsReader appSettings, string[] args)
		{
			return Akka.Actor.Props.Create(() => new LocalActor(appSettings, args));
		}

		public LocalActor(AppSettingsReader _appSettings, string[] args)
		{
			appSettings = _appSettings;
			_logger = Context.GetLogger();
			string serverActorSystemName = (string)appSettings.GetValue("ServerActorSystemName", typeof(string));
			string serverIP = (string)appSettings.GetValue("ServerIP", typeof(string));
			int serverPort = (int)appSettings.GetValue("ServerPort", typeof(int));
			serverPath = "akka.tcp://" + serverActorSystemName + "@" + serverIP + ":" + serverPort + "/user/ServerActor/LocalServer";

			if (args.Length == 0)
			{
				Console.Write("Local 이름을 입력하세요 : ");
				_localName = Console.ReadLine();
			}
			else if (string.Compare(args[0], "-auto", true) == 0)
			{
				_localName = DateTime.Now.ToString("yyyyMMddHHmmss");
				Console.WriteLine(_localName);
			}
			else
			{
				_logger.Info("Check the command.");
				throw new NotSupportedException("Check the command.");
			}

			Context.ActorSelection(serverPath).Tell(new SubscribeMonitorLocal(_localName, Self));

			Receive<DeadLetter>(_ => Handle(_));
			Receive<RetryToSubscribeMonitorLocal>(_ => Handle(_));
			Receive<MonitorLocalSubscribed>(_ => Handle(_));
			Receive<Terminated>(_ => Handle(_));
			Receive<Monitoring>(_ => Handle());
			Receive<RestartMonitor>(_ => Handle(_));
		}

		// 서버 접속이 안될 때(Dead Letter 메시지를 받을 때)
		private void Handle(DeadLetter data)
		{
			_logger.Info("The server is not connected");
			_logger.Info(data.ToString());
			if (File.Exists(@"../../Recipe/Recipe.txt"))
			{
				if (recipeRead == false)
				{
					_logger.Info("The recipe exists.");
					StreamReader RecipeRead;
					RecipeRead = File.OpenText(@"../../Recipe/Recipe.txt");
					string RecipeData = RecipeRead.ReadToEnd();
					monitoringTime = Convert.ToInt32(RecipeData);
					_logger.Info("Read the recipe.");
					nonScheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
						0,
						monitoringTime,
						Self,
						new Monitoring(),
						Self);
					recipeRead = true;
				}
				else
				{
					_logger.Info("Recipe information is available.");
				}
			}
			else
			{
				_logger.Info("The recipe does not exist.");
			}
			Thread.Sleep((int)appSettings.GetValue("RetryServerConnectTime", typeof(int)));
			Self.Tell(new RetryToSubscribeMonitorLocal());
			serverAccessState = false;
		}

		// 서버 재접속
		private void Handle(RetryToSubscribeMonitorLocal none)
		{
			_logger.Info("Attempt to reconnect the server");
			Context.ActorSelection(serverPath).Tell(new SubscribeMonitorLocal(_localName, Self));
		}

		// 서버에 로컬 등록
		private void Handle(MonitorLocalSubscribed succesSubscribe) // 
		{
			_logger.Info("Server connection succeeded.");

			server = Context.ActorSelection(serverPath)
								.ResolveOne(TimeSpan.Zero)
								.Result;
			Context.Watch(server);
			_localID = succesSubscribe.LocalID;
			recipe = succesSubscribe.Recipe;
			Console.WriteLine(recipe);
			monitoringTime = Convert.ToInt32(recipe);
			Console.WriteLine(monitoringTime);
			StreamWriter sw = new StreamWriter(@"../../Recipe/Recipe.txt");
			sw.Write(recipe);
			sw.Close();
			_logger.Info(" Start monitoring.");

			if (recipeRead == true)
			{
				nonScheduler.Cancel();
				recipeRead = false;
			}

			scheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
				0,
				monitoringTime,
				Self,
				new Monitoring(),
				Self);

			serverAccessState = true;
		}

		// 서버가 종료되었을 때
		private void Handle(Terminated terminated)
		{
			_logger.Info("The server has been shutdown.");
			if (terminated.ActorRef.Equals(server))
			{
				serverAccessState = false;
				Self.Tell(new RetryToSubscribeMonitorLocal());
				_logger.Info("Server monitoring end");
				Context.Unwatch(server);


			}
		}
		// 로컬 모니터링
		private void Handle()
		{
			ComputerInfo memoryInfo = new ComputerInfo();
			double totalMemory = ((double)memoryInfo.TotalPhysicalMemory / 1048576);
			double availableMemory = ((double)memoryInfo.AvailablePhysicalMemory / 1048576);
			memCounter = (int)(((totalMemory - availableMemory) / totalMemory) * 100);

			int cpuValue = (int)cpuCounter.NextValue();

			_logger.Info("CPU Usage : " + cpuValue + "%");
			_logger.Info("Memory Usage : " + memCounter + "%");
			LocalData localDatas = new LocalData(_localID, cpuValue, memCounter, 0, 0);

			if (serverAccessState == true)
			{
				_logger.Info(" Transfer data to the LocalServer.");
				server.Tell(new UpdateMonitor(localDatas));
			}
		}
		//로컬이 중단될 때
		protected override void PostStop()
		{
            if (nonScheduler != null)
            {
                nonScheduler.Cancel();
                nonScheduler = null;
            }
            if(scheduler != null)
            {
                scheduler.Cancel();
                scheduler = null;
            }
			
			base.PostStop();
			if (serverAccessState == true)
			{
				_logger.Info("Notifies the server of the shutdown.");
				server.Tell(new UnsubscribeMonitorLocal(_localID));
			}
			serverAccessState = false;
			_logger.Info("The Local terminate.");
		}

		private void Handle(RestartMonitor recipeInfo)
		{
			_logger.Info("Update the recipe.");
			scheduler.Cancel();
			recipe = recipeInfo.Recipe;
			monitoringTime = Convert.ToInt32(recipe);
			StreamWriter sw = new StreamWriter(@"../../Recipe/Recipe.txt");
			sw.Write(recipe);
			sw.Close();
			_logger.Info("Recipe saved");
			scheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
				0,
				monitoringTime,
				Self,
				new Monitoring(),
				Self);

		}
	}

}
