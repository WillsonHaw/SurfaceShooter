using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SS.Surface.XNA.Classes;

namespace SS.Surface.XNA.Managers
{
    public class UserManager : GameComponent
    {
        private List<User> _users = new List<User>();
        private Texture2D _hpBackground;
        private Texture2D _hpFill;

        private const int HPBarWidth = 60;
        private const int HPBarHeight = 20;
        private const int HPBarPadding = 2;

        public UserManager(Game game) : base(game)
        {
            _hpBackground = new Texture2D(game.GraphicsDevice, HPBarWidth, HPBarHeight);
            _hpFill = new Texture2D(game.GraphicsDevice, HPBarWidth - HPBarPadding * 2, HPBarHeight - HPBarPadding * 2);
        }

        public User Get(int id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }

        public void Create(int id, string name, Texture2D sprite)
        {
            lock (_users)
            {
                if (_users.All(x => x.Id != id))
                {
                    var seed = new Random();
                    var position = new Vector2(seed.Next(960), seed.Next(1080));
                    var orientation = (float)(seed.NextDouble() * 360);
                    var newUser = new User
                                      {
                                          Id = id,
                                          Name = name,
                                          HP = 100,
                                          Position = position,
                                          Orientation = orientation,
                                          Sprite = sprite
                                      };

                    _users.Add(newUser);
                }
            }
        }

        public void Remove(int id)
        {
            var user = _users.FirstOrDefault(x => x.Id == id);

            if (user != null)
            {
                lock (_users)
                {
                    _users.Remove(user);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            lock (_users)
            {
                _users.ForEach(u => u.Update(gameTime));
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            foreach (var user in _users)
            {
                var rotation = user.Orientation + MathHelper.PiOver2;
                var rotationOrigin = new Vector2(user.Sprite.Width / 2f, user.Sprite.Height / 2f);

                var stringSize = font.MeasureString(user.Name);
                var textPosition = new Vector2(
                    user.Position.X - (stringSize.X / 2f),
                    user.Position.Y + (user.Sprite.Height / 2f));
                var hpPosition = new Vector2(
                    user.Position.X - HPBarWidth / 2,
                    user.Position.Y + HPBarHeight + (user.Sprite.Height / 2f));
                var hpValuePosition = new Vector2(
                    hpPosition.X + HPBarPadding,
                    hpPosition.Y + HPBarPadding);

                spriteBatch.Draw(user.Sprite, user.Position, null, Color.White, rotation, rotationOrigin, 1.0f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, user.Name, textPosition, Color.White);
                spriteBatch.Draw(_hpBackground, hpPosition, Color.DarkSlateBlue);
                spriteBatch.Draw(_hpFill, hpValuePosition, Color.MediumSpringGreen);
            }
        }
    }
}
