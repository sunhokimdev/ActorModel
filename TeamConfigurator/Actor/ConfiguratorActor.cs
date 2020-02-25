using Akka.Actor;
using Akka.Event;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using static TeamLibrary.Message.ConfiguratorActorMessage;
using static TeamLibrary.ServerActorMessage;

namespace TeamConfigurator.Actor
{
	public class ConfiguratorActor : ReceiveActor
	{
		private readonly ILoggingAdapter _logger;
		private string serverPath;
		private readonly StreamReader _recipeFileStreamReader;
		private FileStream recipeFileStream;
		
		public static Props Props(AppSettingsReader appSettings, string[] args)
		{
			return Akka.Actor.Props.Create(() => new ConfiguratorActor(appSettings, args));
		}

		public ConfiguratorActor(AppSettingsReader appSettings, string[] args)
		{
			_logger = Context.GetLogger();
			string serverActorSystemName = (string)appSettings.GetValue("ServerActorSystemName", typeof(string));
			string serverIP = (string)appSettings.GetValue("ServerIP", typeof(string));
			int serverPort = (int)appSettings.GetValue("ServerPort", typeof(int));
			serverPath = "akka.tcp://" + serverActorSystemName + "@" + serverIP + ":" + serverPort + "/user/ServerActor/LocalServer";
			recipeFileStream = new FileStream(@"../../Recipe/Recipe.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			_recipeFileStreamReader = new StreamReader(recipeFileStream, Encoding.UTF8);
			string recipe = _recipeFileStreamReader.ReadToEnd();


			if (args.Length == 0)
			{
				_logger.Info("Recipe Information : " + recipe);
			}
			else if (string.Compare(args[0], "-update", true) == 0)
			{
				Context.SetReceiveTimeout(TimeSpan.FromMilliseconds((int)appSettings.GetValue("MessageTimeOut", typeof(int))));
				_logger.Info("Send the recipe to the server.");
				Context.ActorSelection(serverPath).Tell(new UpdateRecipe(recipe));
				_logger.Info("Recipe Information : " + recipe);
			}
			else
			{
				throw new NotSupportedException("args is incrrect!");
			}

			Receive<ReceiveTimeout>(value => Handle(value));
			Receive<RecipeUpdated>(_ => Handle());
		}
		private void Handle(ReceiveTimeout timeoutInfo)
		{
			_logger.Info(timeoutInfo.ToString());
			_logger.Info("Recipe failed to transfer");
		}

		public void Handle()
		{
			_logger.Info("Recipe successed to transfer");
			Context.SetReceiveTimeout(null);
		}
	}
}
