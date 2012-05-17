using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SS.Surface.XNA.Classes;

namespace SS.Surface.XNA.Managers
{
    public class ProjectileManager : GameComponent
    {
        private static List<Projectile> _projectiles;
        private static Texture2D _defaultSprite;

        public ProjectileManager(Game game) : base(game)
        {
            _projectiles = new List<Projectile>();
            _defaultSprite = game.Content.Load<Texture2D>("Sprites/projectile");
        }

        public static void Create(int owner, int damage, Vector2 position, float velocity, float orientation, int life)
        {
            lock (_projectiles)
            {
                _projectiles.Add(new Projectile
                    {
                        Owner = owner,
                        Damage = damage,
                        Position = position,
                        Velocity = velocity,
                        Orientation = orientation,
                        Life = life,
                        Sprite = _defaultSprite
                    });
            }
        }

        public override void Update(GameTime gameTime)
        {
            List<Projectile> deadProjectiles = new List<Projectile>();
            
            lock (_projectiles)
            {
                foreach (var projectile in _projectiles)
                {
                    projectile.Update(gameTime);

                    if (!projectile.IsAlive()) deadProjectiles.Add(projectile);
                }
            }

            foreach (var projectile in deadProjectiles)
            {
                Remove(projectile);
            }
        }

        private void Remove(Projectile projectile)
        {
            if (projectile != null)
            {
                lock (_projectiles)
                {
                    _projectiles.Remove(projectile);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            lock (_projectiles)
            {
                _projectiles.ForEach(p => spriteBatch.Draw(p.Sprite, p.Position, null, Color.White));
            }
        }
    }
}
