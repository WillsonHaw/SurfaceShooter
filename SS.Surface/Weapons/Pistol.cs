using System.Windows;

namespace SS.Surface.Weapons
{
	public class Pistol : BaseWeapon
	{
	    public override int RateOfFire
	    {
	        get { return 400; }
	    }

	    public override int BulletSpeed
	    {
	        get { return 15; }
	    }

	    public override int Damage
	    {
            get { return 7; }
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
