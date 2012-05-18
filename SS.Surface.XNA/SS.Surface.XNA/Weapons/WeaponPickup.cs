using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS.Surface.XNA.Weapons
{
    public class WeaponPickup
    {
        public IWeapon Weapon { get; set; }
        public Vector2 Position { get; set; }
        public float Orientation { get; set; }
        public Texture2D Sprite { get; set; }
    }
}
