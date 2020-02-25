using Akka.Actor;
using System.Windows;
using System;
using TeamLibrary;
using System.Threading.Tasks;
using System.Threading;
using Akka.Event;

namespace TeamClient
{
    public partial class MainApplication : Application
    {
        internal static MainViewModel _mainView;
        internal static ConfigureInfo _configureInfo;

        protected override void OnStartup(StartupEventArgs e)
        {
            _configureInfo = new ConfigureInfo();
            var actorWpfSystem = new ActorWpfSystem(_configureInfo.ClientActorName);
            _mainView = new MainViewModel(actorWpfSystem);

            var uiactor = actorWpfSystem.ActorSystem.ActorOf(UIActor.Props(actorWpfSystem, _mainView), "uiactor");
            actorWpfSystem.UIActor = uiactor;

            actorWpfSystem.ActorSystem.EventStream.Subscribe(uiactor, typeof(DeadLetter));

            Task.Run(() =>
            {
                while (actorWpfSystem.ServerActor == null)
                {
                    Thread.Sleep(_configureInfo.RetryServerConnectTime * 1000);

                    try
                    {
                        actorWpfSystem.ServerActor =
                        actorWpfSystem.ActorSystem.ActorSelection
                        (_configureInfo.GetConfigure()).ResolveOne(TimeSpan.Zero).Result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                actorWpfSystem.ServerActor.Tell(new ClientActorMessage.
                    SubscribeMonitorClient(actorWpfSystem.UIActor));

            });

            base.OnStartup(e);
        }
    }
}
