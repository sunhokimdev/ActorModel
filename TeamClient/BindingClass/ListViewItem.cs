using TeamLibrary;
using Akka.Actor;
using static TeamLibrary.ClientActorMessage;

namespace TeamClient
{
    public class ListViewItem
    {
        int _localID;
        string _localName;
        bool _isChecked;
        IActorWpfSystem _actorWpfSystem;
        System.Windows.Media.Brush _foregroundBrush;
        public ListViewItem(
            int id,
            string name,
            System.Windows.Media.Brush brush,
            IActorWpfSystem actor,
            bool isChecked = false)
        {
            _localID = id;
            _localName = name;
            _isChecked = isChecked;
            _actorWpfSystem = actor;
            _foregroundBrush = brush;

            if(isChecked)
            {
                _actorWpfSystem.ServerActor.Tell(new UpdateViewLocal(
                        _localID, _actorWpfSystem.AssignedClientID, isChecked));
            }
        }
        public string LocalName
        {
            get
            {
                return _localName;
            }
        }
        public int LocalID
        {
            get
            {
                return _localID;
            }
        }
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                // 변경이 CheckBox가 True -> False로 변경 됐다면                
                _isChecked = value;

                // 서버 Actor가 존재하고 Local의 View를 갱신하고 싶을 때 서버에 메시지를 던져준다
                if (_actorWpfSystem.ServerActor != null)
                {
                    _actorWpfSystem.ServerActor.Tell(new UpdateViewLocal(
                        _localID, _actorWpfSystem.AssignedClientID, _isChecked));                    
                }

                // 체크 되지 않았다면 해당 그래프 지워준다.
                if (!_isChecked)
                {
                    _actorWpfSystem.UIActor.Tell(
                        new ServerActorMessage.MessageOfClearWaveData(_localID));                    
                }
            }
        }
        public System.Windows.Media.Brush ForegroundColor
        {
            get
            {
                return _foregroundBrush;
            }
        }
    }
}
