// Bullet.cs - Represents a projectile fired by a soldier
using SplashKitSDK;

namespace SoldierRushGame
{
    public class Bullet : GameEntity
    {
        public int Damage { get; private set; }           // Damage this bullet deals
        public bool IsActive { get; private set; }        // Determines whether the bullet is still in play
        private Bitmap _sprite;                           // Bullet sprite based on damage

        // Constructor: initializes bullet position, damage, and selects sprite based on power
        public Bullet(Lane lane, float y, float x, int damage)
        {
            Lane = lane;
            Y = y;
            X = x;
            Damage = damage;
            IsActive = true;

            // Load different colored bullet sprites depending on damage value
            if (damage >= 3)
                _sprite = new Bitmap("bullet_red", "assets/bullets/bullet_red.png");
            else if (damage >= 2)
                _sprite = new Bitmap("bullet_orange", "assets/bullets/bullet_orange.png");
            else
                _sprite = new Bitmap("bullet_blue", "assets/bullets/bullet_blue.png");
        }

        // Update bullet position, deactivate if off-screen
        public override void Update()
        {
            X += 8; // Move rightward
            if (X > 850) IsActive = false; // Remove bullet when it leaves screen
        }

        // Draw the bullet with slight offset based on lane
        public override void Draw()
        {
            float y = GetLaneY(Lane) + 55; // Adjust Y to align bullet with gun
            SplashKit.DrawBitmap(_sprite, X + 60, y); // Slight X offset for aesthetic
        }

        // Get Y-coordinate based on lane
        private float GetLaneY(Lane lane)
        {
            switch (lane)
            {
                case Lane.Top: return 315;
                case Lane.Bottom: return 450;
                default: return 500;
            }
        }

        // Deactivate bullet manually (e.g., after hitting a zombie)
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
