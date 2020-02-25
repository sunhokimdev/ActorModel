using Akka.Actor;

namespace TeamLibrary
{
    public sealed class LocalActorMessage
    {
		public sealed class SubscribeMonitorLocal
		{
			public string LocalName { get; private set; }
			public IActorRef ActorOfLocal { get; private set; }

			public SubscribeMonitorLocal(string localName, IActorRef localActor)
			{
				LocalName = localName;
				ActorOfLocal = localActor;
			}
		}
		public sealed class UnsubscribeMonitorLocal
		{
			public UnsubscribeMonitorLocal(int localID)
			{
				ID = localID;
			}
			public int ID { get; private set; }
		}
		public sealed class UpdateMonitor
		{
			public UpdateMonitor(LocalData localDatas)
			{
				LocalData = localDatas;
			}
			public LocalData LocalData { get; private set; }
		}
		
	}
}
