using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using TeamLibrary;
using Topshelf;

namespace TeamServer
{
    class Program
    {       
        static void Main(string[] args)
        {            
            var actorServiceConfigurator = new ActorServiceConfigurator();
            actorServiceConfigurator.Execute();
        }
    }
    public sealed class ServerActor : ReceiveActor
    {
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

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        public readonly ICancelable Schedule;

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new ServerActor());
        }
        public ServerActor()
        {
            Context.ActorOf(ClientServer.Props(), "ClientServer");
            Context.ActorOf(LocalServer.Props(), "LocalServer");
            Context.ActorOf(DataBaseServer.Props(), "DataBaseServer");

            Receive<MessageOfListViewUpdateLocalList>(updatedLocal =>
            LocalToClient_Send_CreatedLocalID(updatedLocal));

            Receive<MessageOfRequestLocalIDs>(requestedLocalID =>
            RequestAllLocalID(requestedLocalID));

            Receive<MessageOfReturnLocalID>(returnLocal =>
            ReturnAllLocalID(returnLocal));

            Receive<UpdateMonitor>(receivedLocalData =>
            SendToClient_DataOfLocal(receivedLocalData));

            Receive<UnsubscribeMonitorLocal>(deletedLocal =>
            DeleteLocal(deletedLocal));

            Schedule = Context
                .System
                .Scheduler
                .ScheduleTellRepeatedlyCancelable(
                    TimeSpan.Zero,              // The time period that has to pass before the first message is sent.
                    TimeSpan.FromSeconds(1),    // The interval, i.e. the time period that has to pass between messages are being sent.
                    Self,                       // The receiver.
                    new Start(),                // The message.
                    Self);                      // The sender.

            Receive<Start>(_ => Handle(_));
        }        
        public void LocalToClient_Send_CreatedLocalID
            (MessageOfListViewUpdateLocalList updatedLocal)
        {
            Context.Child("ClientServer").Tell(updatedLocal);
        }
        public void RequestAllLocalID(MessageOfRequestLocalIDs requestLocalIDs)
        {
            Context.Child("LocalServer").Tell(requestLocalIDs);
        }
        public void ReturnAllLocalID(MessageOfReturnLocalID returnLocalID)
        {
            Context.Child("ClientServer").Tell(returnLocalID);
        }
        public void SendToClient_DataOfLocal(UpdateMonitor dataOfLocal)
        {
            Context.Child("ClientServer").Tell(dataOfLocal);
        }
        public void DeleteLocal(UnsubscribeMonitorLocal deleteLocal)
        {
            Context.Child("ClientServer").Tell(deleteLocal);
        }
        private void Handle(Start msg)
        {
            
            //_logger.Info(Self.Path.ToString());
        }
    }		
}
