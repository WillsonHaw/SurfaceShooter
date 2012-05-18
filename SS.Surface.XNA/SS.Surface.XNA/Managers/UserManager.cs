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
        public List<User> Users { get; private set; }

        private Texture2D _hpBackground;
        private Texture2D _hpFill;

        private const int HPBarWidth = 60;
        private const int HPBarHeight = 20;
        private const int HPBarPadding = 2;

        public UserManager(Game game) : base(game)
        {
            _hpBackground = new Texture2D(game.GraphicsDevice, 1, 1);
            _hpBackground.SetData(new[] { Color.White });
            _hpFill = new Texture2D(game.GraphicsDevice, 1, 1);
            _hpFill.SetData(new[] { Color.White });
        }

        public override void Initialize()
        {
            Users = new List<User>();
        }

        public User Get(int id)
        {
            return Users.FirstOrDefault(x => x.Id == id);
        }

        public void Create(int id, string name, Texture2D sprite)
        {
            lock (Users)
            {
                if (Users.All(x => x.Id != id))
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

                    Users.Add(newUser);
                }
            }
        }

        public void Remove(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user != null)
            {
                lock (Users)
                {
                    Users.Remove(user);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            lock (Users)
            {
                Users.ForEach(u => u.Update(gameTime));
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            foreach (var user in Users)
            {
                var rotation = user.Orientation + MathHelper.PiOver2;
                var rotationOrigin = new Vector2(user.Sprite.Width / 2f, user.Sprite.Height / 2f);

                var stringSize = font.MeasureString(user.Name);
                var textPosition = new Vector2(
                    user.Position.X - (stringSize.X / 2f),
                    user.Position.Y + (user.Sprite.Height / 2f));
                var hpPosition = new Rectangle(
                    (int)(user.Position.X - HPBarWidth / 2),
                    (int)(user.Position.Y + HPBarHeight + (user.Sprite.Height / 2f)),
                    HPBarWidth,
                    HPBarHeight);
                var hpValuePosition = new Rectangle(
                    hpPosition.X + HPBarPadding,
                    hpPosition.Y + HPBarPadding,
                    (int)((HPBarWidth - HPBarPadding * 2) * (user.HP / 100f)),
                    HPBarHeight - HPBarPadding * 2);

                spriteBatch.Draw(user.Sprite, user.Position, null, Color.White, rotation, rotationOrigin, 1.0f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, user.Name, textPosition, Color.White);
                spriteBatch.Draw(_hpBackground, hpPosition, Color.DarkSlateBlue);
                spriteBatch.Draw(_hpFill, hpValuePosition, Color.MediumSpringGreen);
            }
        }
    }
}
