using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SS.Surface.XNA.Weapons;

namespace SS.Surface.XNA.Classes
{
    public class User
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public Vector2 Position { get; set; }
        public float Orientation { get; set; }
        public IWeapon CurrentWeapon { get; set; }
        public Texture2D Sprite { get; set; }
        public bool Moving { get; set; }

        private Vector2 _acceleration;
        public Vector2 Acceleration
        {
            get
            {
                return _acceleration.Length() < 0.05 ? Vector2.Zero : _acceleration;
            }
            set
            {
                _acceleration = value;
                if (_acceleration.Length() > MaxAccel)
                {
                    _acceleration.Normalize();
                    _acceleration *= MaxAccel;
                }
            }
        }
        #endregion

        private const float Friction = 0.85f;
        private const float MaxAccel = 5;

        public User()
        {
            CurrentWeapon = new Pistol();
            PubNubUpdate();
        }

        private void PubNubUpdate()
        {
            PubNubObservable.Publish("client_channel_" + Id, new { message = "update", hp = HP });
        }

        public void Update(GameTime gameTime)
        {
            if (!Moving)
                Acceleration *= Friction;

            Position += Acceleration;
        }
        
        public void Shoot()
        {
            CurrentWeapon.Shoot(Id, Position, Orientation);
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;

            if (HP < 0) HP = 0;

            PubNubUpdate();
        }
    }
}