using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;

using static TeamLibrary.ClientActorMessage;
using static TeamLibrary.LocalActorMessage;
using static TeamLibrary.ServerActorMessage;

namespace TeamServer
{
	public class ClientServerActor : ReceiveActor
	{
		int _clientsCount = 0;      // 클라이언트들의 갯수를 저장
		Dictionary<int, ClientManagementData> _managerOfClients;   // 클라이언트들을 관리하는 변수
		Stack<int> _deletedClientIDStock;        // 삭제된 클라이언트 ID를 저장 -> HashTable이 거대해지는 것을 방지        

		public ClientServerActor()
		{
			_managerOfClients = new Dictionary<int, ClientManagementData>();
			_deletedClientIDStock = new Stack<int>();            

            Receive<SubscribeMonitorClient>(_ => Handle(_));
            Receive<UnsubscribeMonitorClient>(_ => Handle(_));
			Receive<DeadLetter>(_ => Handle(_));
			Receive<ClientChangeViewType>(_ => Handle(_));
			// 클라이언트에서 리스트뷰를 업데이트 하고 싶을 때 받는 메시지 이벤트
			Receive<MessageOfListViewUpdateLocalList>(_ => Handle(_));
            // 로컬 ID를 리턴 받는다.
			Receive<MessageOfReturnLocalID>(_ => Handle(_));
            // 로컬 데이터를 계속해서 갱신
			Receive<UpdateMonitor>(_ => Handle(_));
            // 리스트뷰의 로컬들을 갱신하고자 할 때 사용
			Receive<UpdateViewLocal>(_ => Handle(_));
            // 로컬들을 삭제할 때 사용
			Receive<UnsubscribeMonitorLocal>(_ => Handle(_));
		}
		static public Props Props()
		{
			return Akka.Actor.Props.Create(() => new ClientServerActor());
		}
		public void Handle(SubscribeMonitorClient clientID)
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

            if (_managerOfClients.ContainsKey(assignedClientID))
            {
                _managerOfClients[assignedClientID].IsTerminated = false;
                _managerOfClients[assignedClientID].AssignedClientActor = clientID.ClientActor;
            }                
            else
            {
                _managerOfClients[assignedClientID] = new ClientManagementData()
                {
                    LocalIDMap = new Dictionary<int, bool>(),
                    AssignedClientActor = clientID.ClientActor,
                    ViewType = ChangedViewType.E_NONE_VIEW,
                    IsTerminated = false,
                };
            }

			// 연결이 완료되었으므로 할당된 아이디를 넘겨준다.
			clientID.ClientActor.Tell(new MonitorClientSubscribed(assignedClientID));

			// Client에서 모든 Local들의 정보를 얻어온다.
			Context.Parent.Tell(new MessageOfRequestLocalIDs(assignedClientID)
			{});

			Console.WriteLine(string.Format("ID {0} 클라이언트 접속 됐습니다", assignedClientID));            
        }
		// 클라이언트의 접속을 종료할 때 사용한다.
		public void Handle(UnsubscribeMonitorClient clientID)
		{
			_deletedClientIDStock.Push(clientID.ClientID);
			_managerOfClients[clientID.ClientID].IsTerminated = true;
            _managerOfClients[clientID.ClientID].LocalIDMap.Clear();
            _managerOfClients[clientID.ClientID].ViewType = ChangedViewType.E_NONE_VIEW;            

            Console.WriteLine(string.Format("ID {0}클라이언트 접속을 종료합니다", clientID.ClientID));
		}
		public void Handle(DeadLetter terminateServerMsg)
		{            
			_managerOfClients.Clear();
			_clientsCount = 0;
			_deletedClientIDStock.Clear();
		}
		// Client의 View형식을 바꿔준다.
		public void Handle(ClientChangeViewType clientViewType)
		{
			Console.WriteLine(string.Format("Client ID {0} : 파형 View의 속성이 변경되었습니다.",
				clientViewType.ClientID));

			_managerOfClients[clientViewType.ClientID].ViewType = clientViewType.ViewType;
		}
		// 리스트에 로컬들을 넣기위한 작업
		public void Handle(MessageOfListViewUpdateLocalList updatedLocalID)
		{
            foreach(var client in _managerOfClients)
            {
                client.Value.LocalIDMap[updatedLocalID.AssignedLocalID] = false;
                client.Value.AssignedClientActor.Tell(new MonitorSearched(
                    updatedLocalID.AssignedLocalID,
                    updatedLocalID.AssignedLocalName));                
            }
		}
		public void Handle(MessageOfReturnLocalID returnedLocalID)
		{
            // 해당 클라이언트 ID에서 요청한 LocalList를 불러온다.
            _managerOfClients[returnedLocalID.AssignedClientID].AssignedClientActor.
                Tell(new MonitorSearched(
                    returnedLocalID.ReturnLocalID,
                    returnedLocalID.ReturnLocalName));                
		}
		// 해당 데이터를 클라이언트에게 보내준다.
		public void Handle(UpdateMonitor localData)
		{
			int sendData = 0;

            for(int i=0;i<_clientsCount;i++)
            {
                if (!_managerOfClients[i].IsTerminated &&
                    _managerOfClients[i].LocalIDMap.ContainsKey(localData.LocalData.LocalID))
                {
                    if(_managerOfClients[i].LocalIDMap[localData.LocalData.LocalID])
                    {
                        // 해당 클라이언트의 viewType을 확인한다.
                        if (_managerOfClients[i].ViewType == ChangedViewType.E_NONE_VIEW)
                            continue;
                        else if (_managerOfClients[i].ViewType == ChangedViewType.E_CPU_VIEW)
                            sendData = localData.LocalData.CPUValue;
                        else if (_managerOfClients[i].ViewType == ChangedViewType.E_MEMORY_VIEW)
                            sendData = localData.LocalData.MEMValue;
                        else if (_managerOfClients[i].ViewType == ChangedViewType.E_DISK_VIEW)
                            sendData = localData.LocalData.DiskValue;
                        else if (_managerOfClients[i].ViewType == ChangedViewType.E_NETWORK_VIEW)
                            sendData = localData.LocalData.NetworkValue;
                        else
                            sendData = localData.LocalData.TotalValue;

                        _managerOfClients[i].AssignedClientActor.Tell(new MonitorUpdating(
                            localData.LocalData.LocalID, sendData));                            
                    }
                }
            }
		}
		public void Handle(UpdateViewLocal localViewInfo)
		{
			// Local의 데이터를 볼 수 있다면
			if (localViewInfo.IsLocalView)
			{
				_managerOfClients[localViewInfo.ClientID].
					LocalIDMap[localViewInfo.LocalID] = true;

				Console.WriteLine(string.Format("{1} Client에 {0} Local 데이터 리스트뷰 추가",
					localViewInfo.LocalID,
					localViewInfo.ClientID));
			}
			else
			{
				_managerOfClients[localViewInfo.ClientID].
					LocalIDMap[localViewInfo.LocalID] = false;

				Console.WriteLine(string.Format("{1} Client에 {0} Local 데이터 리스트뷰 제거",
				localViewInfo.LocalID,
				localViewInfo.ClientID));
			}
		}
		// 해당 로컬 아이디를 제거한다.
		public void Handle(UnsubscribeMonitorLocal local)
		{
			foreach (var obj in _managerOfClients)
			{
				obj.Value.LocalIDMap[local.ID] = false;
				obj.Value.AssignedClientActor.Tell(local);
			}
		}
	}
}
