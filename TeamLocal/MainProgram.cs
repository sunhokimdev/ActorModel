using System;
using System.Threading;
using System.Diagnostics;
using Akka.Actor;
using TeamLibrary;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;
using Akka.Event;
using System.Configuration;

namespace TeamLocal.Script
{

	class MainProgram
    {
        static void Main(string[] args)
        {
			AppSettingsReader appSettings = new AppSettingsReader();
			string localActorName = (string)appSettings.GetValue("LocalActorSystemName", typeof(string));
			var system = ActorSystem.Create(localActorName);
			IActorRef localActor = system.ActorOf(LocalActor.Props(appSettings, args), ActorPaths.LocalActor.Name);
			system.EventStream.Subscribe(localActor, typeof(DeadLetter));

			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				eventArgs.Cancel = true;
				localActor.Tell(PoisonPill.Instance);
				system.Terminate();
			};

			system.WhenTerminated.Wait();
		}        
    }
}
