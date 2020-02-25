using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Threading;
using TeamLibrary;
using static TeamLibrary.ClientActorMessage;

namespace TeamClient
{
    public class UIActor : ReceiveActor
    {
        GraphViewModel _viewModel;
        EventViewModel _eventModel;

        static public Props Props(IActorWpfSystem actorWpfSystem, MainViewModel mainModel)
        {
            return Akka.Actor.Props.Create(() => new UIActor(actorWpfSystem, mainModel));
        }
        public class AddModuleMessage
        {
            public IActorRef Actor { get; set; }
            public ModuleVM ViewModel { get; set; }
            public AddModuleMessage(IActorRef actor, ModuleVM viewModel)
            {
                Actor = actor;
                ViewModel = viewModel;
            }
        }

        List<IActorRef> _moduleActors;      // 액터 모듈을 저장하는 자료구조        
        readonly IActorWpfSystem _actorWpfSystem;       // 액터 시스템저장
        readonly MainViewModel _mainView;   // Main Module

        public UIActor(IActorWpfSystem actorWpfSystem, MainViewModel mainView)
        {
            _actorWpfSystem = actorWpfSystem;
            _mainView = mainView;
            _moduleActors = new List<IActorRef>();

            // 연결이 성공했음을 알려준다.
            Receive<MonitorClientSubscribed>(_ => Handle(_));
            // Local들을 리스트 뷰에 저장한다.
            Receive<MonitorSearched>(_ => Handle(_));
            // Local의 데이터를 Client상에서 받게 한다.
            Receive<MonitorUpdating>(_ => Handle(_));
            // 파형 그릴 때 발생되는 메시지
            Receive<ServerActorMessage.MessageOfClearWaveData>(_ => Handle(_));
            // 로컬에서 연결을 종료할 때 발생되는 메시지
            Receive<LocalActorMessage.UnsubscribeMonitorLocal>(_ => Handle(_));
            // Start And Stop 버튼 처리 
            Receive<MonitorStartAndStop>(_ => Handle(_));
            // 서버접속이 계속 안될경우 발생되는 이벤트
            Receive<DeadLetter>(_ => Handle(_));
            // 서버가 터질 경우 발생되는 이벤트
            Receive<Terminated>(_ => Handle(_));
            // Timer Text 추가할 때 발생하는 이벤트
            Receive<AddTimerTextMessage>(_ => Handle(_));
            // 타이머 텍스트 제거할 때 발생하는 이벤트
            Receive<DeleteAllTimerTextMessage>(_ => Handle(_));
        }
        // 서버가 종료 되었을 때 발생되는 이벤트
        private void Handle(Terminated terminatedMsg)
        {
            if (terminatedMsg.ActorRef.Equals(_actorWpfSystem.ServerActor))
            {
                Context.Unwatch(_actorWpfSystem.ServerActor);
            }

            MainApplication.Current.Dispatcher.Invoke((Action)delegate
            {
                Console.WriteLine("데이터들을 모두 지웁니다.");
                _viewModel.ListItems.Clear();
                _viewModel.ListLine.Clear();
            });
        }
        void RetryToSubscribeClient()
        {
            Thread.Sleep(MainApplication._configureInfo.RetryServerConnectTime * 1000);
            _actorWpfSystem.ServerActor.Tell(new SubscribeMonitorClient(_actorWpfSystem.UIActor));            
        }
        public void Handle(DeadLetter letter)
        {
            Thread.Sleep(MainApplication._configureInfo.RetryServerConnectTime * 1000);
            try
            {
                _actorWpfSystem.ServerActor = _actorWpfSystem.ActorSystem.ActorSelection
                            (MainApplication._configureInfo.GetConfigure()).
                            ResolveOne(TimeSpan.Zero).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            RetryToSubscribeClient();
        }
        // 파형을 볼 수 있는 형태인지 확인하는 작업
        public void Handle(MonitorStartAndStop check)
        {
            MainApplication.Current.Dispatcher.Invoke((Action)delegate
            {
                _viewModel.IsViewData = check.IsStart;
            });
        }
        public void Handle(MonitorClientSubscribed successMsg)
        {
            _actorWpfSystem.AssignedClientID = successMsg.ClientID; // 할당된 ID를 부여한다.
            Console.WriteLine(string.Format("할당 받은 ID : {0}", _actorWpfSystem.AssignedClientID));

            Context.Watch(_actorWpfSystem.ServerActor);
        }
        public void Handle(MonitorSearched localList)
        {
            Console.WriteLine("추가된 로컬 ID : " + localList.AssignedLocalID);
            MainApplication.Current.Dispatcher.Invoke((Action)delegate
            {
                _viewModel.AddLocalData(localList);
            });
        }
        // 파형 추가하는 메서드
        public void Handle(MonitorUpdating localWaveData)
        {
            MainApplication.Current.Dispatcher.Invoke((Action)delegate
            {
                _viewModel.DrawLine(localWaveData.WaveData,
                    localWaveData.AssignedLocalID);
            });
        }
        // 로컬 리스트를 지운다.
        public void Handle(LocalActorMessage.UnsubscribeMonitorLocal localID)
        {
            MainApplication.Current.Dispatcher.Invoke((Action)delegate
            {
                _viewModel.DeleteLocalData(localID);
                _viewModel.ClearWaveData(localID.ID, false);
            });
        }
        // 모든 파형 제거
        public void Handle(ServerActorMessage.MessageOfClearWaveData clearWave)
        {
            MainApplication.Current.Dispatcher.Invoke((Action)delegate
            {
                _viewModel.ClearWaveData(clearWave.LocalID);
            });
        }
        public void Handle(AddTimerTextMessage addTimer)
        {
            MainApplication.Current.Dispatcher.Invoke((Action)delegate
            {
                _eventModel.OnAddTimertext(addTimer.TimerText, addTimer.XPosition);
            });
        }
        public void Handle(DeleteAllTimerTextMessage allDeleteTimer)
        {
            MainApplication.Current.Dispatcher.Invoke((Action)delegate
            {
                _eventModel.OnAllDeleteTimerText();
            });
        }
        protected override void PreStart()
        {
            _viewModel = new GraphViewModel(_actorWpfSystem, "ViewModel");
            var viewModel = Context.System.ActorOf(MainModuleActor.Props(_viewModel), "ViewModule");
            _viewModel.Actor = viewModel;

            Handle(new AddModuleMessage(viewModel, _viewModel));

            _eventModel = new EventViewModel
                (_actorWpfSystem, "EventModel");
            var viewModel2 = Context.System.ActorOf(MainModuleActor.Props(_eventModel), "EventModule");
            _eventModel.Actor = viewModel2;

            Handle(new AddModuleMessage(viewModel2, _eventModel));
        }
        void Handle(AddModuleMessage x)
        {
            _moduleActors.Add(x.Actor);
            _mainView.AddModule(x.ViewModel);
        }
    }
}
