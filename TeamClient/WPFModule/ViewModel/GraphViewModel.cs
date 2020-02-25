using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Timers;
using TeamLibrary;
using System.Runtime.InteropServices;
using static TeamLibrary.ClientActorMessage;
using System.Threading.Tasks;

namespace TeamClient
{
    public class GraphViewModel : ModuleVM
    {
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int n, int m);

        ObservableCollection<ListViewItem> _listItems;
        ObservableCollection<GraphLine> _listLine;
        Dictionary<int, PointDictionary> _localDataMap;

        public string LocalCount
        {
            get
            {                
                return _localCount;
            }
            set
            {
                _localCount = value;
                RaisePropertyChanged("LocalCount");
            }
        }

        const int _initXPosition = 70;
        double _xIndex = 5;
        string _localCount; // 로컬의 갯수
        bool _isViewData = true;

        Timer _timer;
        int _prevTimerCount = 0;
        public GraphViewModel(IActorWpfSystem actorWpfSystem, string name) : base(actorWpfSystem, name)
        {
            _listItems = new ObservableCollection<ListViewItem>();
            _listLine = new ObservableCollection<GraphLine>();
            _localDataMap = new Dictionary<int, PointDictionary>();
            _timer = new Timer();            
        }
        public ObservableCollection<ListViewItem> ListItems
        {
            get { return _listItems; }
        }
        public ObservableCollection<GraphLine> ListLine
        {
            get { return _listLine; }
        }        
        // 데이터를 볼 수 있는 상태인지 확인
        public bool IsViewData {
            get
            {
                return _isViewData;
            }
            set
            {
                _isViewData = value;
            }
        }
        public void DrawLine(int wavedata, int localID)
        {
            DateTime now = DateTime.Now.ToLocalTime();
            TimeSpan span = (now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            int nowTime = (int)span.TotalSeconds;
            
            // 그리기작업과 Event 콤보박스에서 데이터 뷰를 잘 선택했을 때 그려준다.
            if (_isViewData)     // 그릴 수 있는 상태라면 파형을 그려준다.
            {
                // 파형 데이터라면 
                Point point = new Point(_localDataMap[localID].Point.X +
                    _xIndex, 350 - (wavedata + 30));                

                if (nowTime - _prevTimerCount >= 10)
                {
                    _prevTimerCount = nowTime;

                    _listLine.Add(new GraphLine
                    {
                        LocalID = int.MinValue,
                        From = new Point(point.X, 0),
                        To = new Point(point.X, 350),
                        StrokeColor = new SolidColorBrush(Color.FromRgb(255, 0, 255)),
                    });

                    _actorWpfSystem.UIActor.Tell(
                        new AddTimerTextMessage(DateTime.Now.ToString("HH시mm분ss초"), (int)point.X - 70), null);
                }

                // 파형 데이터가 50이 넘어가면 비프음을 내준다.
                if(wavedata > 50)
                {
                    _listLine.Add(new GraphLine
                    {
                        LocalID = localID,
                        From = _localDataMap[localID].Point,
                        To = point,
                        StrokeColor = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                    });
                    Task.Run((Action)delegate
                    {
                        Beep(3000, 1000);
                    });                    
                }
                // 안 넘어가면 그냥 파형을 그려준다.
                else
                {
                    _listLine.Add(new GraphLine
                    {
                        LocalID = localID,
                        From = _localDataMap[localID].Point,
                        To = point,
                        StrokeColor = _localDataMap[localID].ForegroundColor,
                    });                    
                }

                _localDataMap[localID].Point = point;

                if (point.X >= 550)
                {
                    ClearWaveData(localID, false);
                    ClearWaveData(int.MinValue, true);
                    _actorWpfSystem.UIActor.Tell(new DeleteAllTimerTextMessage(),null);
                }               
            }
        }
        public void AddLocalData(MonitorSearched item)
        {
            var random = new Random(DateTime.Now.Millisecond);

            Brush brush = new SolidColorBrush(
                Color.FromRgb

                ((byte)random.Next(1, 100),
                (byte)random.Next(1, 100),
                (byte)random.Next(1, 100)));

            ListViewItem listItem =
                new ListViewItem(
                item.AssignedLocalID,
                item.AssigendLocalName,
                brush,
                _actorWpfSystem,
                true);

            _listItems.Add(listItem);
            
            if(_localDataMap.Count == 0)
            {
                _localDataMap[item.AssignedLocalID] = new PointDictionary()
                {
                    Point = new Point(_initXPosition, 350),
                    ForegroundColor = brush,
                };

                DateTime now = DateTime.Now.ToLocalTime();
                TimeSpan span = (now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
                _prevTimerCount = (int)span.TotalSeconds;
            }
            else
            {
                foreach(var value in _localDataMap)
                {
                    if(value.Key != item.AssignedLocalID)
                    {
                        _localDataMap[item.AssignedLocalID] = new PointDictionary()
                        {
                            Point = new Point(value.Value.Point.X, 350),
                            ForegroundColor = brush,
                        };
                        break;
                    }                    
                }
            }

            _actorWpfSystem.ServerActor.Tell(new ClientChangeViewType(
                        _actorWpfSystem.AssignedClientID, ChangedViewType.E_CPU_VIEW),null);            
            LocalCount = _listItems.Count.ToString();
        }
        public void DeleteLocalData(LocalActorMessage.UnsubscribeMonitorLocal local)
        {
            foreach (var obj in _listItems)
            {
                if (obj.LocalID == local.ID)
                {
                    _listItems.Remove(obj);
                    break;
                }
            }

            LocalCount = _listItems.Count.ToString();
        }
        public void ClearWaveData(int localID, bool isrestart = true)
        {
            if (localID == int.MaxValue)
            {
                _listLine.Clear();

                foreach (KeyValuePair<int, PointDictionary> localMapData in _localDataMap)
                {
                    PointDictionary point = localMapData.Value;
                    point.Point = new Point(_initXPosition, 350);
                }
            }
            else if(localID == int.MinValue)
            {
                for(int i=0;i<_listLine.Count;i++)
                {
                    if(_listLine[i].LocalID == localID)
                    {
                        _listLine.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _listLine.Count; i++)
                {
                    if (_listLine[i].LocalID == localID)
                    {
                        _listLine.RemoveAt(i);
                        i--;
                    }
                }

                if (isrestart)
                    _localDataMap[localID].Point = new Point(_initXPosition, 350);
                else
                {
                    double pointY = _localDataMap[localID].Point.Y;
                    _localDataMap[localID].Point = new Point(_initXPosition, pointY);
                }
            }
        }        
    }
}
