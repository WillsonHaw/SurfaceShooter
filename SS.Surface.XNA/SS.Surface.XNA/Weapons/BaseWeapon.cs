using System.Timers;
using Microsoft.Xna.Framework;
using SS.Surface.XNA.Managers;

namespace SS.Surface.XNA.Weapons
{
    public abstract class BaseWeapon : IWeapon
    {
        public abstract int RateOfFire { get; }
        public abstract int BulletSpeed { get; }
        public abstract int Damage { get; }
        public abstract int ProjectileLife { get; }

        private Timer _shotTimer;
        private bool _cooldown;

        protected BaseWeapon()
        {
            _cooldown = false;
            _shotTimer = new Timer();
            _shotTimer.Enabled = false;
            _shotTimer.Elapsed += (sender, args) =>
                                  {
                                      _cooldown = false;
                                      _shotTimer.Stop();
                                  };
        }
        
        public void Reload()
        {
            _cooldown = true;
            if (_shotTimer != null)
            {
                if (_shotTimer.Interval < RateOfFire || _shotTimer.Interval > RateOfFire)
                    _shotTimer.Interval = RateOfFire;
                _shotTimer.Start();
            }
        }

        public void DoShot(int owner, Vector2 origin, float orientation)
        {
            if (!_cooldown) {
                ProjectileManager.Create(owner, Damage, origin, BulletSpeed, orientation, ProjectileLife);
            }
        }

        public abstract void Shoot(int owner, Vector2 origin, float orientation);
    }
}
