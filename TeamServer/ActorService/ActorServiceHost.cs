using Akka.Actor;
using Akka.Configuration;
using Topshelf;

namespace TeamServer.ActorService
{
    public sealed class ActorServiceHost : ServiceControl
    {
        private readonly Config _config;
        private readonly ActorServiceCommandLine _commandLine;
        private ActorSystem _system;

        public ActorServiceHost(Config config, ActorServiceCommandLine commandLine)
        {
            _config = config;
            _commandLine = commandLine;
        }

        public bool Start(HostControl hostControl)
        {
            _system = CreateActorSystem(_config);

#if DEBUG
            StartPetabridgeCmd();
#endif

            LogCommandLine();
            CreateTopLevelActors();
            return true;
        }

        private ActorSystem CreateActorSystem(Config config)
        {                        
            return ActorSystem.Create(
                config.GetString(HoconPaths.ActorSystemName),
                config);   
        }

        private void StartPetabridgeCmd()
        {
            //PetabridgeCmd
            //    .Get(_system)
            //    .Start();
        }

        private void LogCommandLine()
        {
            _commandLine.Log(_system.Log);
        }

        private void CreateTopLevelActors()
        {
            _system.ActorOf(ServerActor.Props(), ActorPaths.ServerActor.Name);
        }

        public bool Stop(HostControl hostControl)
        {
            _system.Terminate().Wait();            
            return true;
        }
    }
}

