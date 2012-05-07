using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using System.Windows;

namespace SS.Surface.Classes
{
    public class User : INotifyPropertyChanged
    {
        #region Properties
        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private int _hp;
        public int HP
        {
            get { return _hp; }
            set
            {
                _hp = value;
                RaisePropertyChanged("HP");
            }
        }

        private Point _position;
        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RaisePropertyChanged("Position");
            }
        }

        private double _orientation;
        public double Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                RaisePropertyChanged("Orientation");
            }
        }
        #endregion

        private Timer _actionTimer;
        private List<Action> _actionList;

        public User()
        {
            _actionList = new List<Action>();
            _actionTimer = new Timer(16);
            _actionTimer.Elapsed += DoActions;
            _actionTimer.Start();
        }

        private void DoActions(object state, ElapsedEventArgs elapsedEventArgs)
        {
            if (_actionList.Count == 0)
            {
                _actionTimer.Stop();
            }
            else
            {
                foreach (Action action in _actionList)
                {
                    action.BeginInvoke(null, null);
                }
            }
        }

        public void AddAction(Action action)
        {
            if (!_actionList.Contains(action))
            {
                _actionList.Add(action);
            }

            if (!_actionTimer.Enabled)
                _actionTimer.Start();
        }

        public void RemoveAction(Action action)
        {
            if (_actionList.Contains(action))
                _actionList.Remove(action);

            if (_actionList.Count == 0)
                _actionTimer.Stop();
        }

        public void Shoot(int id)
        {

        }

        public void Right()
        {
            Orientation += 5.0;
        }

        public void Left()
        {
            Orientation -= 5.0;
        }

        public void Back()
        {
            Move(-5);
        }

        public void Forward()
        {
            Move(5);
        }

        private void Move(double speed)
        {
            var rads = (Math.PI / 180) * (Orientation - 90);
            var x = Math.Cos(rads) * speed + Position.X;
            var y = Math.Sin(rads) * speed + Position.Y;
            Position = new Point(x, y);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}