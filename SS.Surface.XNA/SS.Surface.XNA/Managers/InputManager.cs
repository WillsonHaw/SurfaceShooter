using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using SS.Surface.XNA.Classes;

namespace SS.Surface.XNA.Managers
{
    public class InputManager : GameComponent
    {
        private readonly Game _game;
        private readonly UserManager _userManager;
        private readonly ProjectileManager _projectileManager;

        private List<Action> _actionList;

        public InputManager(Game game, UserManager userManager, ProjectileManager projectileManager) : base(game)
        {
            _game = game;
            _userManager = userManager;
            _projectileManager = projectileManager;
            _actionList = new List<Action>();
        }

        public override void Initialize()
        {
            base.Initialize();

            //Setup PubNub
            new PubNubObservable(Constants.SUBSCRIBE_KEY, Constants.SURFACE_CHANNEL)
              .Subscribe(ProcessMessage);
        }

        private void AddAction(Action action)
        {
            lock (_actionList)
            {
                if (!_actionList.Contains(action))
                {
                    _actionList.Add(action);
                }
            }
        }

        private void RemoveAction(Action action)
        {
            lock (_actionList)
            {
                if (_actionList.Contains(action))
                    _actionList.Remove(action);
            }
        }

        private void DoActions()
        {
            lock (_actionList)
            {
                foreach (Action action in _actionList)
                {
                    action.BeginInvoke(null, null);
                }
            }
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
                    var vect = jObj["vector"];
                    var user = _userManager.Get(id);

                    switch (message)
                    {
                        case "connected":
                            var name = jObj["charName"].ToString();
                            _userManager.Create(id, name, _game.Content.Load<Texture2D>("Sprites/spaceship"));
                            break;
                        case "disconnected":
                            _userManager.Remove(id);
                            break;
                        case "move":
                            if (vect != null && user != null)
                            {
                                switch (vect.ToString())
                                {
                                    case "up":
                                        user.Moving = false;
                                        break;
                                    case "down":
                                        user.Moving = true;
                                        break;
                                    default:
                                        var orientation = Math.Atan2((float)vect["y"], (float)vect["x"]);
                                        user.Orientation = (float)orientation;
                                        user.Acceleration = new Vector2((float)vect["x"], (float)vect["y"]);
                                        break;
                                }
                            }
                            break;
                        case "fire":
                            if (vect != null && user != null)
                            {
                                switch (vect.ToString())
                                {
                                    case "up":
                                        RemoveAction(user.Shoot);
                                        break;
                                    case "down":
                                        AddAction(user.Shoot);
                                        break;
                                    default:
                                        var orientation = Math.Atan2((float)vect["y"], (float)vect["x"]);
                                        user.Orientation = (float)orientation;
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            DoActions();
        }
    }
}
