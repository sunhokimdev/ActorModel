using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLibrary
{
	public sealed class ServerActorMessage
	{
		public sealed class MessageOfListViewUpdateLocalList
		{
			public MessageOfListViewUpdateLocalList(int _AssignedLocalID, string _AssignedLocalName)
			{
				AssignedLocalID = _AssignedLocalID;
				AssignedLocalName = _AssignedLocalName;
			}
			public int AssignedLocalID { get; private set; }
			public string AssignedLocalName { get; private set; }
		}
		public sealed class MessageOfReturnLocalID
		{
			public MessageOfReturnLocalID(int _AssignedLocalID, int _ReturnLocalID, string _ReturnLocalName)
			{
				AssignedClientID = _AssignedLocalID;
				ReturnLocalID = _ReturnLocalID;
				ReturnLocalName = _ReturnLocalName;
			}
			public int AssignedClientID { get; private set; }
			public int ReturnLocalID { get; private set; }
			public string ReturnLocalName { get; private set; }
		}
		public sealed class MessageOfConnectSuccess
		{
			public int AssignedID { get; private set; }
			public MessageOfConnectSuccess(int assignedID)
			{
				AssignedID = assignedID;
			}
		}
		// 서버 종료시 발생되는 메시지
		public sealed class MessageOfTerminatedServer
		{
		}

		// 로컬 아이디들을 요청하는 메시지
		public sealed class MessageOfRequestLocalIDs
		{
			public MessageOfRequestLocalIDs(int _AssignedClientID)
			{
				AssignedClientID = _AssignedClientID;
			}
			public int AssignedClientID { get; private set; }
		}

		// 클라이언트로 보내는 로컬 데이터
		public sealed class MessageOfSendLocalDataToClient
		{
			// 데이터를 요청한 클라이언트 ID
			public int RequestedClientID { get; private set; }
			// 데이터를 보내는 Local ID
			public int SendLocalID { get; private set; }
			// Local Data
			public int WaveData { get; private set; }

			public MessageOfSendLocalDataToClient(
				int requestClientID,
				int sendLocalID,
				int waveData)
			{
				RequestedClientID = requestClientID;
				SendLocalID = sendLocalID;
				WaveData = waveData;
			}
		}

		// 파형 데이터를 모두 제거하라는 메시지 - LocalID가 intMax라면 모두 지워준다.
		public sealed class MessageOfClearWaveData
		{
			public int LocalID { get; private set; }
			public MessageOfClearWaveData(int localID)
			{
				LocalID = localID;
			}
		}
		public sealed class MonitorLocalSubscribed
		{
			public int LocalID { get; private set; }
			public MonitorLocalSubscribed(int localID, string recipe)
			{
				LocalID = localID;
				Recipe = recipe;
			}
			public string Recipe { get; private set; }
		}
		public sealed class RecipeUpdated { }

		public sealed class RestartMonitor
		{
			public RestartMonitor(string recipe)
			{
				Recipe = recipe;
			}
			public string Recipe { get; private set; }
		}

	}
}
