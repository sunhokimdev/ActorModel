using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamClient
{
    class ActorWpfSystem : IActorWpfSystem
    {
        public int AssignedClientID { get; set; }
        public ActorSystem ActorSystem { get; set; }
        public IActorRef UIActor { get; set; }
        public IActorRef ServerActor { get; set; }
        public ActorWpfSystem(string actorName)
        {
            ActorSystem = ActorSystem.Create(actorName);
        }
    }
}
