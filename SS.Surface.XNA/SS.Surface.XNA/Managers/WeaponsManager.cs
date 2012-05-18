using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SS.Surface.XNA.Weapons;

namespace SS.Surface.XNA.Managers
{
    public class WeaponsManager : GameComponent
    {
        private const int _trySpawnInterval = 10000; //In Milliseconds
        private static Random _seed;

        private Game _game;
        private long _lastTick;
        private Dictionary<IWeapon, Texture2D> _weapons;

        public List<WeaponPickup> SpawnedWeapons { get; private set; }
        
        public WeaponsManager(Game game) : base(game)
        {
            _game = game;
            _seed = new Random();
        }

        public override void Initialize()
        {
            var weapons = (from t in Assembly.GetExecutingAssembly().GetTypes()
                           where t.IsClass && !t.IsAbstract && t.Namespace == "SS.Surface.XNA.Weapons" && t.Name != "Pistol" && t.BaseType.Name == "BaseWeapon"
                           select t).ToList();

            _weapons = new Dictionary<IWeapon, Texture2D>();
            SpawnedWeapons = new List<WeaponPickup>();

            foreach (var weapon in weapons)
            {
                _weapons.Add((IWeapon)Activator.CreateInstance(null, weapon.FullName).Unwrap(),
                    _game.Content.Load<Texture2D>("Sprites/Weapons/" + weapon.Name));
            }

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _lastTick += gameTime.ElapsedGameTime.Ticks;

            if (_lastTick > _trySpawnInterval)
                SpawnWeapon();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            lock (SpawnedWeapons)
            {
                foreach (var weapon in SpawnedWeapons)
                {
                    spriteBatch.Draw(weapon.Sprite, weapon.Position, null, Color.White, weapon.Orientation, new Vector2(
                        weapon.Sprite.Width / 2.0f, weapon.Sprite.Height / 2.0f), 1f, SpriteEffects.None, 0);
                }
            }
        }

        public void Remove(WeaponPickup weapon)
        {
            lock (SpawnedWeapons)
            {
                if (SpawnedWeapons.Contains(weapon))
                    SpawnedWeapons.Remove(weapon);
            }
        }

        private void SpawnWeapon()
        {
            foreach (var weapon in _weapons.Where(weapon => (float)_seed.NextDouble() * 100 < weapon.Key.ChanceToSpawn))
            {
                lock (SpawnedWeapons)
                {
                    SpawnedWeapons.Add(new WeaponPickup
                                            {
                                                Orientation = (float)_seed.NextDouble() * MathHelper.TwoPi,
                                                Position = new Vector2(
                                                    (float)_seed.NextDouble() * 1920f,
                                                    (float)_seed.NextDouble() * 1080f),
                                                Weapon = weapon.Key,
                                                Sprite = weapon.Value
                                            });
                }
            }
        }
    }
}
