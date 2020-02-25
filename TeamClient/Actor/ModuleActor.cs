using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace TeamClient
{
    public class ModuleActor : ReceiveActor
    {
        protected readonly ModuleVM _mvm;
        public ModuleActor(ModuleVM mvm)
        {
            _mvm = mvm;
        }
    }
}
