using System.Collections.Generic;
using Akka.Actor;
using TeamLibrary;
using System;
using Akka.Event;
using System.Threading.Tasks;

namespace TeamServer
{
    public class ClientManagementData
    {       
        public Dictionary<int, bool> LocalIDMap { get; set; } // 해당 Local ID가 보여질 수 있는 상태인지 확인하는 데이터 구조
        public IActorRef AssignedClientActor { get; set; }
        public ChangedViewType ViewType { get; set; }
        public bool IsTerminated { get; set; } // 파괴된 상태인지 확인 -> 클라이언트가
    }
    public class ClientServer : ReceiveActor
    {
        int _clientsCount = 0;      // 클라이언트들의 갯수를 저장
        Dictionary<int, ClientManagementData> _managerOfClients;   // 클라이언트들을 관리하는 변수
        Stack<int> _deletedClientIDStock;        // 삭제된 클라이언트 ID를 저장 -> HashTable이 거대해지는 것을 방지

        public ClientServer()
        {
            _managerOfClients = new Dictionary<int, ClientManagementData>();
            _deletedClientIDStock = new Stack<int>();

            Receive<SubscribeMonitorClient>(clientID => 
            AddClient(clientID));

            Receive<UnsubscribeMonitorClient>(clientID => 
            DeleteClient(clientID));

            Receive<DeadLetter>(terminateServer => 
            TerminateRootServer(terminateServer));

            Receive<MessageOfChangedViewType>(changedViewAttribute =>
            ChangeViewTypeOfClient(changedViewAttribute));

            // 클라이언트에서 리스트뷰를 업데이트 하고 싶을 때 받는 메시지 이벤트
            Receive<MessageOfListViewUpdateLocalList>(updateLocalList =>
            UpdateLocalList(updateLocalList));

            Receive<MessageOfReturnLocalID>(returnedLocalID =>
            RequestedLocalIDAdd(returnedLocalID));

            Receive<UpdateMonitor>(receivedLocalData =>
            SendToClient_DataOfLocal(receivedLocalData));

            Receive<MessageOfUpdatedViewLocal>(viewUpdateInfo =>
            UpdateViewOfLocal(viewUpdateInfo));

            Receive<UnsubscribeMonitorLocal>(deleteLocal =>
            DeleteLocalID(deleteLocal));
        }
        static public Props Props()
        {
            return Akka.Actor.Props.Create(() => new ClientServer());
        }
        public void AddClient(SubscribeMonitorClient clientID)
        {            
            int assignedClientID = _clientsCount;

            if (_deletedClientIDStock.Count != 0)
            {
                assignedClientID = _deletedClientIDStock.Pop();
                
                if (_clientsCount == assignedClientID)
                    _clientsCount--;
            }                
            else
                _clientsCount++;

            _managerOfClients[assignedClientID] = new ClientManagementData()
            {
                LocalIDMap = new Dictionary<int,bool>(),
                AssignedClientActor = clientID.ActorOfClient,
                ViewType = ChangedViewType.E_NONE_VIEW,
                IsTerminated = false,
            };

            // 연결이 완료되었으므로 할당된 아이디를 넘겨준다.
            clientID.ActorOfClient.Tell(new MonitorClientSubscribed(assignedClientID));            

            // Client에서 모든 Local들의 정보를 얻어온다.
            Context.Parent.Tell(new MessageOfRequestLocalIDs()
            {
                AssignedClientID = assignedClientID,
            });

            Console.WriteLine(string.Format("Client ID {0} 접속 됐습니다",assignedClientID));
        }
        // 클라이언트의 접속을 종료할 때 사용한다.
        public void DeleteClient(UnsubscribeMonitorClient clientID)
        {
            _deletedClientIDStock.Push(clientID.ClientID);
            _managerOfClients[clientID.ClientID] = null;

            Console.WriteLine(string.Format("Client ID {0}클라이언트 접속을 종료합니다",clientID.ClientID));
        }
        public void TerminateRootServer(DeadLetter terminateServerMsg)
        {
            foreach(var client in _managerOfClients)
            {
                client.Value.AssignedClientActor.Tell(new MessageOfTerminatedServer());
            }

            _managerOfClients.Clear();
            _clientsCount = 0;
            _deletedClientIDStock.Clear();

            Console.WriteLine("Server를 종료합니다");
        }
        // Client의 View형식을 바꿔준다.
        public void ChangeViewTypeOfClient(MessageOfChangedViewType clientViewType)
        {
            Console.WriteLine(string.Format("Client ID {0} : 파형 View의 속성이 변경되었습니다.",
                clientViewType.ClientID));

            _managerOfClients[clientViewType.ClientID].ViewType = clientViewType.ViewType;
        }
        // 리스트에 로컬들을 넣기위한 작업
        public void UpdateLocalList(MessageOfListViewUpdateLocalList updatedLocalID)
        {            
            foreach(var client in _managerOfClients)
            {
                client.Value.LocalIDMap[updatedLocalID.AssignedLocalID] = false;
                client.Value.AssignedClientActor.Tell(new MonitorSearched
                {
                    AssignedLocalID = updatedLocalID.AssignedLocalID,
                    AssigendLocalName = updatedLocalID.AssignedLocalName,
                });
            }
        }
        public void RequestedLocalIDAdd(MessageOfReturnLocalID returnedLocalID)
        {
            // 해당 클라이언트 ID에서 요청한 LocalList를 불러온다.
            _managerOfClients[returnedLocalID.AssignedClientID].AssignedClientActor.
                Tell(new MessageOfAddLocal_To_ListView
                {
                    AssignedLocalID = returnedLocalID.ReturnLocalID,
                    AssigendLocalName = returnedLocalID.ReturnLocalName,
                });

            _managerOfClients[returnedLocalID.AssignedClientID].LocalIDMap
                [returnedLocalID.ReturnLocalID] = false;
        }
        // 해당 데이터를 클라이언트에게 보내준다.
        public void SendToClient_DataOfLocal(UpdateMonitor localData)
        {
            int sendData = 0;

            foreach(var client in _managerOfClients)
            {
                if (!client.Value.IsTerminated &&
                    client.Value.LocalIDMap.ContainsKey(localData.LocalData.LocalID))
                {
                    // 해당 클라이언트의 viewType을 확인한다.
                    if (client.Value.ViewType == ChangedViewType.E_NONE_VIEW)
                        continue;
                    else if (client.Value.ViewType == ChangedViewType.E_CPU_VIEW)
                        sendData = localData.LocalData.CPUValue;
                    else if (client.Value.ViewType == ChangedViewType.E_MEMORY_VIEW)
                        sendData = localData.LocalData.MEMValue;
                    else if (client.Value.ViewType == ChangedViewType.E_DISK_VIEW)
                        sendData = localData.LocalData.DiskValue;
                    else if (client.Value.ViewType == ChangedViewType.E_NETWORK_VIEW)
                        sendData = localData.LocalData.NetworkValue;
                    else
                        sendData = localData.LocalData.TotalValue;

                    client.Value.AssignedClientActor.Tell
                        (new MessageOfSendLocalData()
                        {
                            AssignedLocalID = localData.LocalData.LocalID,
                            WaveData = sendData,
                        });
                }
            }
        }

        public void UpdateViewOfLocal(MessageOfUpdatedViewLocal localViewInfo)
        {
            // Local의 데이터를 볼 수 있다면
            if(localViewInfo.IsLocalView)
            {
                if(_managerOfClients[localViewInfo.ClientID].LocalIDMap.
                    ContainsKey(localViewInfo.LocalID))
                {
                    _managerOfClients[localViewInfo.ClientID].
                    LocalIDMap[localViewInfo.LocalID] = true;

                    Console.WriteLine(string.Format("{1} Client에 {0} Local 데이터 리스트뷰 추가",
                    localViewInfo.LocalID,
                    localViewInfo.ClientID));
                }
            }
            else
            {
                if (_managerOfClients[localViewInfo.ClientID].LocalIDMap.
                    ContainsKey(localViewInfo.LocalID))
                {
                    _managerOfClients[localViewInfo.ClientID].
                    LocalIDMap[localViewInfo.LocalID] = false;

                    Console.WriteLine(string.Format("{1} Client에 {0} Local 데이터 리스트뷰 제거",
                    localViewInfo.LocalID,
                    localViewInfo.ClientID));
                }                             
            }
        }
        // 해당 로컬 아이디를 제거한다.
        public void DeleteLocalID(UnsubscribeMonitorLocal local)
        {
            foreach(var obj in _managerOfClients)
            {
                obj.Value.LocalIDMap[local.ID] = false;
                obj.Value.AssignedClientActor.Tell(local);
            }
        }
    }
}
