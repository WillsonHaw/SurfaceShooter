using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS.Surface.XNA.Classes
{
    public class Projectile
    {
        public int Owner { get; set; }
        public int Damage { get; set; }
        public Vector2 Position { get; set; }
        public float Velocity { get; set; }
        public float Orientation { get; set; }
        public Texture2D Sprite { get; set; }
        public int Life { get; set; }

        private TimeSpan _spawnTime = TimeSpan.Zero;

        public void Update(GameTime gameTime)
        {
            _spawnTime += gameTime.ElapsedGameTime;

            var x = Math.Cos(Orientation) * Velocity + Position.X;
            var y = Math.Sin(Orientation) * Velocity + Position.Y;
            Position = new Vector2((float)x, (float)y);
        }

        public bool IsAlive()
        {
            return _spawnTime.TotalMilliseconds < Life;
        }
    }
}
