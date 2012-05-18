using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SS.Surface.XNA.Classes;
using SS.Surface.XNA.Weapons;

namespace SS.Surface.XNA.Managers
{
    public class CollisionManager :GameComponent
    {
        private readonly UserManager _userManager;
        private readonly ProjectileManager _projectileManager;
        private readonly WeaponsManager _weaponsManager;

        public CollisionManager(Game game, UserManager userManager, ProjectileManager projectileManager, WeaponsManager weaponsManager)
            : base(game)
        {
            _userManager = userManager;
            _projectileManager = projectileManager;
            _weaponsManager = weaponsManager;
        }

        public override void Update(GameTime gameTime)
        {
            var removedProjectiles = new Dictionary<User, Projectile>();
            var removedWeapons = new Dictionary<User, WeaponPickup>();

            lock (_userManager.Users)
            {
                foreach (var user in _userManager.Users)
                {
                    var box = new Rectangle(
                        (int)(user.Position.X - user.Sprite.Width / 2f),
                        (int)(user.Position.Y - user.Sprite.Height / 2f),
                        user.Sprite.Width,
                        user.Sprite.Height);


                    //Check projectile collision
                    lock (_projectileManager.Projectiles)
                    {
                        foreach (var projectile in _projectileManager.Projectiles)
                        {
                            if ((box.X < projectile.Position.X && projectile.Position.X < box.X + box.Width) &&
                                (box.Y < projectile.Position.Y && projectile.Position.Y < box.Y + box.Height) &&
                                projectile.Owner != user.Id)
                            {
                                removedProjectiles.Add(user, projectile);
                            }
                        }
                    }

                    //Check weapon pickup
                    lock (_weaponsManager.SpawnedWeapons)
                    {
                        foreach (var weapon in _weaponsManager.SpawnedWeapons)
                        {
                            if ((box.X < weapon.Position.X && weapon.Position.X < box.X + box.Width) &&
                                (box.Y < weapon.Position.Y && weapon.Position.Y < box.Y + box.Height))
                            {
                                removedWeapons.Add(user, weapon);
                            }
                        }
                    }
                }
            }

            foreach (var kvp in removedProjectiles)
            {
                var user = kvp.Key;
                var projectile = kvp.Value;

                user.TakeDamage(projectile.Damage);
                _projectileManager.Remove(projectile);

                if (user.HP <= 0)
                    _userManager.Remove(user.Id);
            }

            foreach (var kvp in removedWeapons)
            {
                var user = kvp.Key;
                var weapon = kvp.Value;

                user.CurrentWeapon = weapon.Weapon;
                _weaponsManager.Remove(weapon);
            }
        }
    }
}
