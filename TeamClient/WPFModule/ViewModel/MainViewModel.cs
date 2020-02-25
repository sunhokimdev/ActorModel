using Akka.Actor;
using System;
using System.Collections.ObjectModel;
using TeamLibrary;

namespace TeamClient
{
    public class MainViewModel : VirtualModuleBase
    {
        readonly IActorWpfSystem _actorWpfSystem;
        ObservableCollection<ModuleVM> _modules;
        public ObservableCollection<ModuleVM> Modules { get { return _modules; } }
        public MainViewModel(IActorWpfSystem actorWpfSystem)
        {
            _actorWpfSystem = actorWpfSystem;
            _modules = new ObservableCollection<ModuleVM>();
        }
        public void AddModule(ModuleVM vm)
        {
            _modules.Add(vm);
        }
        // 윈도우가 종료되었을 때
        public void WindowClosing()
        {
            if (_actorWpfSystem.ServerActor != null)
            {
                _actorWpfSystem.ServerActor.Tell(new ClientActorMessage.UnsubscribeMonitorClient
                (_actorWpfSystem.AssignedClientID));

                Console.WriteLine("프로그램 종료합니다");
            }
        }
    }
}
