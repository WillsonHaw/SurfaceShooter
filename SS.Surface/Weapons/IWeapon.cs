using System.Windows;

namespace SS.Surface.Weapons
{
    public interface IWeapon
    {
        int RateOfFire { get; }
        int BulletSpeed { get; }
        int Damage { get; }
        int ProjectileLife { get; }
        void Shoot(int owner, Point origin, double orientation);
    }
}
