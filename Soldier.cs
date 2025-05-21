// Soldier.cs - Base class for all soldier types
using SplashKitSDK;

namespace SoldierRushGame
{
    public class Soldier : GameEntity
    {
        public int Damage { get; protected set; }               // Damage dealt by the soldier
        public int HP { get; protected set; } = 10;              // Current health points
        public int MaxHP { get; protected set; } = 10;           // Maximum health points
        public bool IsDead => HP <= 0;                           // Check if soldier is dead

        protected List<Bitmap> _runShootFrames = new List<Bitmap>(); // Animation frames for shooting/running
        protected int _frameIndex = 0;                            // Current frame in animation
        protected int _animationTimer = 0;                        // Timer to control animation speed
        protected int _animationSpeed = 5;                        // Speed of frame change

        // Constructor to initialize soldier with lane, damage, and X position
        public Soldier(Lane lane, int damage, float x)
        {
            Lane = lane;
            Damage = damage;
            HP = 10;
            MaxHP = 10;
            X = x;
            Y = GetLaneY(lane);
        }

        // Update animation frame based on timer
        public override void Update()
        {
            _animationTimer++;
            if (_animationTimer >= _animationSpeed)
            {
                _frameIndex = (_frameIndex + 1) % _runShootFrames.Count;
                _animationTimer = 0;
            }
        }

        // Draw soldier with animation and health bar
        public override void Draw()
        {
            float y = GetLaneY(Lane);
            if (_runShootFrames.Count > 0)
            {
                Bitmap currentFrame = _runShootFrames[_frameIndex];
                SplashKit.DrawBitmap(currentFrame, X, y);
            }
            DrawHealthBar(y);
        }

        // Create a bullet instance when shooting
        public Bullet Shoot() => new Bullet(Lane, Y, X + 40, Damage);

        // Upgrade weapon increases damage
        public void UpgradeWeapon() => Damage += 1;

        // Take damage and reduce HP, ensuring it doesn't go below zero
        public void TakeDamage(int dmg) => HP = Math.Max(0, HP - dmg);

        // Heal soldier and restore HP, capped at MaxHP
        public void Heal(int amount) => HP = Math.Min(MaxHP, HP + amount);

        // Draw health bar above soldier
        private void DrawHealthBar(float baseY)
        {
            float barWidth = 40;
            float barHeight = 6;
            float hpRatio = (float)HP / MaxHP;
            float x = X + 64 - (barWidth / 2);
            float y = baseY - 10;

            SplashKit.FillRectangle(Color.Black, x - 1, y - 1, barWidth + 2, barHeight + 2);
            SplashKit.FillRectangle(Color.DarkGray, x, y, barWidth, barHeight);

            Color hpColor = hpRatio > 0.5f ? Color.Green : (hpRatio > 0.2f ? Color.Orange : Color.Red);
            SplashKit.FillRectangle(hpColor, x, y, barWidth * hpRatio, barHeight);
        }

        // Get the Y coordinate for a given lane
        protected float GetLaneY(Lane lane) =>
            lane == Lane.Top ? 315 :
            lane == Lane.Bottom ? 450 : 500;
    }
}