using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SS.Surface.XNA.Classes;

namespace SS.Surface.XNA.Managers
{
    public class ProjectileManager : GameComponent
    {
        private static ProjectileManager _singleton;
        public List<Projectile> Projectiles { get; private set; }
        private Texture2D _defaultSprite;

        public ProjectileManager(Game game) : base(game)
        {
            _defaultSprite = game.Content.Load<Texture2D>("Sprites/projectile");
            _singleton = this;
        }

        public override void Initialize()
        {
            Projectiles = new List<Projectile>();
        }

        public static void Create(int owner, int damage, Vector2 position, float velocity, float orientation, int life)
        {
            lock (_singleton.Projectiles)
            {
                _singleton.
                Projectiles.Add(new Projectile
                    {
                        Owner = owner,
                        Damage = damage,
                        Position = position,
                        Velocity = velocity,
                        Orientation = orientation,
                        Life = life,
                        Sprite = _singleton._defaultSprite
                    });
            }
        }

        public override void Update(GameTime gameTime)
        {
            List<Projectile> deadProjectiles = new List<Projectile>();
            
            lock (Projectiles)
            {
                foreach (var projectile in Projectiles)
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

        public void Remove(Projectile projectile)
        {
            if (projectile != null)
            {
                lock (Projectiles)
                {
                    Projectiles.Remove(projectile);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            lock (Projectiles)
            {
                Projectiles.ForEach(p => spriteBatch.Draw(p.Sprite, p.Position, null, Color.White));
            }
        }
    }
}
