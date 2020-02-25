using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamClient
{
    public class MainModuleActor : ModuleActor
    {
        public static Props Props(ModuleVM mvm)
        {
            return Akka.Actor.Props.Create(() => new MainModuleActor(mvm));
        }
        public MainModuleActor(ModuleVM mvm) : base(mvm)
        {

        }
    }
}
