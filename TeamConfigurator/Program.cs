using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamConfigurator.Actor;

namespace TeamConfigurator
{
	class Program
	{
		static void Main(string[] args)
		{
			AppSettingsReader appSettings = new AppSettingsReader();
			string configuratorSystemName = (string)appSettings.GetValue("ConfiguratorSystemName", typeof(string));
			var system = ActorSystem.Create("configuratorActorName");
			IActorRef configuratorActor = system.ActorOf(ConfiguratorActor.Props(appSettings, args), "ConfiguratorActor");
			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				eventArgs.Cancel = true;
				configuratorActor.Tell(PoisonPill.Instance);
				system.Terminate();
			};

			system.WhenTerminated.Wait();
		}
	}
}
