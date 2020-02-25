using Akka.Actor;

namespace TeamClient
{
    public abstract class ModuleVM : VirtualModuleBase
    {
        public IActorRef Actor { get; set; }
        string _name;
        protected readonly IActorWpfSystem _actorWpfSystem;

        public ModuleVM(IActorWpfSystem actorWpfSystem, string name)
        {
            _actorWpfSystem = actorWpfSystem;
            _name = name;
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(_name);
                }
            }
        }
    }
}
