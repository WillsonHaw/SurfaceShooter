using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using SS.Surface.Classes;

namespace SS.Surface.ViewModels
{
    public class SurfaceWindowViewModel
    {
        private Dispatcher _dispatcher;

        public SurfaceWindowViewModel()
        {
            Users = new ObservableCollection<User>();
            Messages = new ObservableCollection<string>();

            _dispatcher = Dispatcher.CurrentDispatcher;
            new PubNubObservable(Constants.SUBSCRIBE_KEY, Constants.SURFACE_CHANNEL)
              .Subscribe(ProcessMessage);
        }

        private void ProcessMessage(string sender)
        {
            var data = JArray.Parse(sender);

            if (data.Any())
            {
                foreach (JToken token in data)
                {
                    var jObj = JObject.Parse(token.ToString());
                    var message = jObj["message"].ToString();
                    var id = int.Parse(jObj["id"].ToString());
                    var user = Users.FirstOrDefault(x => x.Id == id);

                    switch (message)
                    {
                        case "connected":
                            var name = jObj["charName"].ToString();
                            CreateUser(id, name);
                            break;
                        case "disconnected":
                            RemoveUser(id);
                            break;
                        case "uu":
                            if (user != null)
                                user.RemoveAction(user.Forward);
                            break;
                        case "ud":
                            if (user != null)
                                user.AddAction(user.Forward);
                            break;
                        case "du":
                            if (user != null)
                                user.RemoveAction(user.Back);
                            break;
                        case "dd":
                            if (user != null)
                                user.AddAction(user.Back);
                            break;
                        case "lu":
                            if (user != null)
                                user.RemoveAction(user.Left);
                            break;
                        case "ld":
                            if (user != null)
                                user.AddAction(user.Left);
                            break;
                        case "ru":
                            if (user != null)
                                user.RemoveAction(user.Right);
                            break;
                        case "rd":
                            if (user != null)
                                user.AddAction(user.Right);
                            break;
                        case "fd":
                            if (user != null)
                                user.AddAction(user.Shoot);
                            break;
                        case "fu":
                            if (user != null)
                                user.RemoveAction(user.Shoot);
                            break;
                    }
                }
            }
        }

        private void RemoveUser(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user != null)
            {
                if (Dispatcher.CurrentDispatcher != _dispatcher)
                {
                    _dispatcher.BeginInvoke(
                       new Action(() => Users.Remove(user)),
                       DispatcherPriority.Send);
                }
                else
                {
                    Users.Remove(user);
                }
            }
        }

        private void CreateUser(int id, string name)
        {
            if (Users.All(x => x.Id != id))
            {
                var seed = new Random();
                var position = new Point(seed.Next(960), seed.Next(1080));
                var orientation = seed.NextDouble() * 360;
                var newUser = new User
                {
                    Id = id,
                    Name = name,
                    HP = 100,
                    Position = position,
                    Orientation = orientation
                };

                if (Dispatcher.CurrentDispatcher != _dispatcher)
                {
                     _dispatcher.BeginInvoke(
                        new Action(() => Users.Add(newUser)),
                        DispatcherPriority.Send);
                }
                else
                {
                    Users.Add(newUser);
                }
                PubNubObservable.Publish("client_channel_" + id, new { message = "update", hp = newUser.HP });
            }
        }

        public void UserHit(User user, ProjectileViewModel projVm)
        {
            user.TakeDamage(projVm.Damage);
        }

        #region Properties

        public ObservableCollection<User> Users { get; set; } 
        public ObservableCollection<string> Messages { get; set; }
        public string GameURL { get { return "http://warm-samurai-5129.herokuapp.com/"; } }
        
        #endregion
    }
}
