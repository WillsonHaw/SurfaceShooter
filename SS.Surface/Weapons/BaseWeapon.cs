using System;
using System.Timers;
using System.Windows;

namespace SS.Surface.Weapons
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

        public void DoShot(int owner, Point origin, double orientation)
        {
            if (!_cooldown) {
                var rads = (Math.PI / 180) * (orientation - 90);
                var x = Math.Cos(rads) * BulletSpeed;
                var y = Math.Sin(rads) * BulletSpeed;
                SurfaceWindow1.AddProjectile(owner, origin, new Vector(x, y), Damage, ProjectileLife);
            }
        }

        public abstract void Shoot(int owner, Point origin, double orientation);
    }
}
