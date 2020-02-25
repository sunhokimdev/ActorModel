using Akka.Actor;

namespace TeamClient
{
    public interface IActorWpfSystem
    {
        ActorSystem ActorSystem { get; }
        IActorRef UIActor { get; }
        IActorRef ServerActor { get; set; } // 서버 Actor       
        int AssignedClientID { get; set; } // 할당된 클라이언트 ID
    }
}
