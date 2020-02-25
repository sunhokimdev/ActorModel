using Akka.Actor;
using System.Collections.ObjectModel;
using static TeamLibrary.ClientActorMessage;
using TeamLibrary;

namespace TeamClient
{
    public class EventViewModel : ModuleVM
    {
        public ObservableCollection<ComboBoxStructure> ViewComboBoxTexts{ get; set; }
        public ObservableCollection<TimerTextItem> TimerItemTemplate { get; set; }
        public ComboBoxStructure ViewComboBoxText
        {
            #region ViewComboBoxHandle
            get
            {
                return _viewComboBoexText;
            }
            set
            {
                _viewComboBoexText = value;

                ChangedViewType viewType = ChangedViewType.E_NONE_VIEW;

                if (_viewComboBoexText.Text == "AllView")
                    viewType = ChangedViewType.E_TOTAL_VIEW;
                else if (_viewComboBoexText.Text == "CPUView")
                    viewType = ChangedViewType.E_CPU_VIEW;
                else if (_viewComboBoexText.Text == "MemoryView")
                    viewType = ChangedViewType.E_MEMORY_VIEW;
                else if (_viewComboBoexText.Text == "DiskView")
                    viewType = ChangedViewType.E_DISK_VIEW;
                else if (_viewComboBoexText.Text == "NetworkView")
                    viewType = ChangedViewType.E_NETWORK_VIEW;

                if (_actorWpfSystem.ServerActor != null)
                {
                    // 콤보 박스가 변경되었으면 서버로 변경되었다는 메시지를 전달한다.
                    _actorWpfSystem.ServerActor.Tell(new ClientChangeViewType(
                        _actorWpfSystem.AssignedClientID, viewType));

                    _actorWpfSystem.UIActor.Tell(
                        new ServerActorMessage.MessageOfClearWaveData(int.MaxValue));                    
                }
            }
            #endregion
        }
        ComboBoxStructure _viewComboBoexText;
        string _controlButtonText;
        bool _isWaveStart = false;  // 파형 그릴 수 있는 상태인지 확인

        public string StartAndPauseContent
        {
            get
            {
                return _controlButtonText ?? (_controlButtonText = "Add");
            }
            set
            {
                _controlButtonText = value;
                RaisePropertyChanged("StartAndPauseContent");   // 바인딩 된 이름을 넣어야 한다.
            }
        }
        public EventViewModel(IActorWpfSystem actorWpfSystem, string name) : base(actorWpfSystem, name)
        {
            _controlButtonText = "Pause";

            StartAndPauseViewCommand = new DelegateCommand(OnStartAndPauseView);

            ViewComboBoxTexts = new ObservableCollection<ComboBoxStructure>()
            {
                new ComboBoxStructure(){ID=0,Text=""},
                new ComboBoxStructure(){ID=1,Text="AllView"},
                new ComboBoxStructure(){ID=2,Text="CPUView"},
                new ComboBoxStructure(){ID=3,Text="MemoryView"},
                new ComboBoxStructure(){ID=4,Text="DiskView"},
                new ComboBoxStructure(){ID=5,Text="NetworkView"}
            };
            ViewComboBoxText = ViewComboBoxTexts[2];

            TimerItemTemplate = new ObservableCollection<TimerTextItem>();
        }
        private DelegateCommand _startAndPauseViewCommand;
        public DelegateCommand StartAndPauseViewCommand
        {
            get
            {
                return _startAndPauseViewCommand;
            }
            set
            {
                _startAndPauseViewCommand = value;
                RaisePropertyChanged("StartAndPauseViewCommand");
            }
        }
        // 중지, 시작 이벤트
        public void OnStartAndPauseView()
        {
            _isWaveStart = !_isWaveStart;

            if (_isWaveStart)
                StartAndPauseContent = "Start";
            else
                StartAndPauseContent = "Pause";

            _actorWpfSystem.UIActor.Tell(new MonitorStartAndStop
                (_isWaveStart));
        }
        public void OnAddTimertext(string timerText, int xPos)
        {
            TimerItemTemplate.Clear();
            TimerItemTemplate.Add(new TimerTextItem(timerText, xPos));
        }
        public void OnAllDeleteTimerText()
        {
            TimerItemTemplate.Clear();
        }
    }
}
