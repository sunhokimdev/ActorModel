using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamServer.ActorService
{
    public sealed class UserSupervisorStrategy : SupervisorStrategyConfigurator
    {
        public UserSupervisorStrategy()
        {
        }

        public override SupervisorStrategy Create()
        {
            return new OneForOneStrategy((ex) =>
            {
                OutOfMemoryException outOfMemory = new OutOfMemoryException();
                if (outOfMemory.GetType() == ex.GetType())
                {
                    GC.Collect();
                }

                return SupervisorStrategy.DefaultStrategy.Decider.Decide(ex);
            });
        }
    }
}
