using System.Windows;

namespace SS.Surface.Weapons
{
    public class MachineGun : BaseWeapon
    {
        public override int RateOfFire
        {
            get { return 100; }
        }

        public override int BulletSpeed
        {
            get { return 20; }
        }

        public override int Damage
        {
            get { return 9; }
        }

        public override int ProjectileLife
        {
            get { return 3000; }
        }

        public override void Shoot(int owner, Point origin, double orientation)
        {
            DoShot(owner, origin, orientation);
            Reload();
        }
    }
}
