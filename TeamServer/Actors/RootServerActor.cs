using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamLibrary;
using static TeamLibrary.LocalActorMessage;
using static TeamLibrary.ServerActorMessage;

namespace TeamServer
{
	public class ServerActor : ReceiveActor
	{
		#region Messages
		public sealed class Start { }

		public sealed class Stop
		{
			public string From { get; private set; }
			public IReadOnlyList<string> Reasons { get; private set; }

			public Stop(string from, List<string> reasons)
			{
				From = from;
				Reasons = reasons;
			}
		}
		#endregion

		private readonly ILoggingAdapter _logger = Context.GetLogger();				

		public static Props Props()
		{
			return Akka.Actor.Props.Create(() => new ServerActor());
		}
		protected override void PostStop()
		{
			base.PostStop();			
		}

		private void RegisterMessageHandlers()
		{
			Receive<Start>(_ => Handle(_));
		}

		private void Handle(Start msg)
		{
			_logger.Info("Hello! MireroSystem.");
		}
		public ServerActor()
		{			
			Context.ActorOf(ClientServerActor.Props(), "ClientServer");
			Context.ActorOf(LocalServerActor.Props(), "LocalServer");
			
			Receive<MessageOfListViewUpdateLocalList>(_ => Handle(_));
			Receive<MessageOfRequestLocalIDs>(_ => Handle(_));
			Receive<MessageOfReturnLocalID>(_ => Handle(_));
			Receive<UpdateMonitor>(_ => Handle(_));
			Receive<UnsubscribeMonitorLocal>(_ => Handle(_));
		}

		public void Handle(MessageOfListViewUpdateLocalList updatedLocal)
		{
			Context.Child("ClientServer").Tell(updatedLocal);
		}
		public void Handle(MessageOfRequestLocalIDs requestLocalIDs)
		{
			Context.Child("LocalServer").Tell(requestLocalIDs);
		}
		public void Handle(MessageOfReturnLocalID returnLocalID)
		{
			Context.Child("ClientServer").Tell(returnLocalID);
		}
		public void Handle(UpdateMonitor dataOfLocal)
		{
			Context.Child("ClientServer").Tell(dataOfLocal);
		}
		public void Handle(UnsubscribeMonitorLocal deleteLocal)
		{
			Context.Child("ClientServer").Tell(deleteLocal);
		}
	}
}
