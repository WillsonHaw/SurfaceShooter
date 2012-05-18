using Microsoft.Xna.Framework;

namespace SS.Surface.XNA.Weapons
{
    public interface IWeapon
    {
        int RateOfFire { get; }
        int BulletSpeed { get; }
        int Damage { get; }
        int ProjectileLife { get; }
        float ChanceToSpawn { get; }
        void Shoot(int owner, Vector2 origin, float direction);
    }
}
