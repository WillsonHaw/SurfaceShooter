using Microsoft.Xna.Framework;

namespace SS.Surface.XNA.Weapons
{
    public class Spreader : BaseWeapon
    {
        public override int RateOfFire
        {
            get { return 300; }
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

        public override void Shoot(int owner, Vector2 origin, float orientation)
        {
            //DoShot(owner, origin, Vector2.Transform(orientation, ) orientation - 13);
            DoShot(owner, origin, orientation);
            //DoShot(owner, origin, orientation + 13);
            Reload();
        }
    }
}
