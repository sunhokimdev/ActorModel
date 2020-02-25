using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamLibrary;
using Akka.Actor;
using static TeamLibrary.ClientActorMessage;

namespace TeamServer
{
    public class ClientManagementData
    {
        public Dictionary<int, bool> LocalIDMap { get; set; } // 해당 Local ID가 보여질 수 있는 상태인지 확인하는 데이터 구조
        public IActorRef AssignedClientActor { get; set; }
        public ChangedViewType ViewType { get; set; }
        public bool IsTerminated { get; set; } // 파괴된 상태인지 확인 -> 클라이언트가
    }
}
