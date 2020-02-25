using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace TeamLibrary
{
    // 클라이언트 콤보 박스 데이터 형식
    public sealed class ClientActorMessage
    {
        public enum ChangedViewType
        {
            E_NONE_VIEW = 2001,
            E_TOTAL_VIEW,
            E_CPU_VIEW,
            E_MEMORY_VIEW,
            E_DISK_VIEW,
            E_NETWORK_VIEW
        }
        public sealed class SubscribeMonitorClient
        {
            public IActorRef ClientActor { get; private set; }
            public SubscribeMonitorClient(IActorRef clientActor)
            {                
                ClientActor = clientActor;
            }
        }
        public sealed class MonitorClientSubscribed
        {
            public int ClientID { get; private set; }
            public MonitorClientSubscribed(int clientID)
            {
                ClientID = clientID;
            }
        }
        public sealed class UnsubscribeMonitorClient
        {
            public int ClientID { get; private set; }
            public UnsubscribeMonitorClient(int clientID)
            {
                ClientID = clientID;
            }
        }
        public sealed class MonitorUpdating
        {
            public int AssignedLocalID { get; private set; }
            public int WaveData { get; private set; }
            public MonitorUpdating(int localID, int waveData)
            {
                AssignedLocalID = localID;
                WaveData = waveData;
            }
        }
        // 로컬들을 리스트 뷰에 추가하는 메시지
        public sealed class MonitorSearched
        {
            public int AssignedLocalID { get; private set; }
            public string AssigendLocalName { get; private set; }
            public MonitorSearched(int localID, string localName)
            {
                AssignedLocalID = localID;
                AssigendLocalName = localName;
            }
        }
        public sealed class ClientChangeViewType
        {
            public ChangedViewType ViewType { get; private set; }
            public int ClientID { get; private set; }
            public ClientChangeViewType(int clientID, ChangedViewType viewType)
            {
                ClientID = clientID;
                ViewType = viewType;
            }
        }
        public sealed class MonitorStartAndStop
        {
            public bool IsStart { get; private set; }
            public MonitorStartAndStop(bool isStart)
            {
                IsStart = isStart;
            }
        }
        public sealed class UpdateViewLocal
        {
            public bool IsLocalView { get; private set; }
            public int LocalID { get; private set; }
            public int ClientID { get; private set; }
            public UpdateViewLocal(int localID, int clientID, bool isLocalView)
            {
                LocalID = localID;
                ClientID = clientID;
                IsLocalView = isLocalView;
            }
        }
        public sealed class AddTimerTextMessage
        {
            public string TimerText { get; private set; }
            public int XPosition { get; private set; }
            public AddTimerTextMessage(string timerText, int xPos)
            {
                TimerText = timerText;
                XPosition = xPos;
            }
        }
        public sealed class DeleteAllTimerTextMessage
        {

        }
    }
}
