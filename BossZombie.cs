// BossZombie.cs - Defines a powerful enemy with custom animations and behavior
using SplashKitSDK;
using System.Collections.Generic;

namespace SoldierRushGame
{
    public class BossZombie : Zombie
    {
        private List<Bitmap> _walkFrames = new List<Bitmap>();   // Frames for walking animation
        private List<Bitmap> _dieFrames = new List<Bitmap>();    // Frames for dying animation
        private List<Bitmap> _attackFrames = new List<Bitmap>(); // Frames for attacking animation

        private int _frameIndex = 0;       // Current animation frame index
        private int _animationTimer = 0;   // Timer to control frame changes
        private int _animationSpeed = 6;   // Speed of animation

        private bool _isAttacking = false; // Flag for attacking state

        // Constructor: initializes boss zombie with HP and loads all animation frames
        public BossZombie(Lane lane, int hp)
        {
            Lane = lane;
            HP = hp;
            MaxHP = hp;
            IsBoss = true;
            X = SplashKit.ScreenWidth() - 50;
            Y = GetLaneY(lane);

            // Load walk animation frames
            for (int i = 1; i <= 15; i++)
            {
                string frameNum = i.ToString("D4");
                string path = $"assets/zombies/boss_walk/{frameNum}.png";
                _walkFrames.Add(new Bitmap($"boss_walk_{frameNum}", path));
            }

            // Load die animation frames
            for (int i = 130; i <= 150; i++)
            {
                string frameNum = i.ToString("D4");
                string path = $"assets/zombies/boss_die/{frameNum}.png";
                _dieFrames.Add(new Bitmap($"boss_die_{frameNum}", path));
            }

            // Load attack animation frames
            for (int i = 460; i <= 470; i++)
            {
                string frameNum = i.ToString("D4");
                string path = $"assets/zombies/boss_attack/{frameNum}.png";
                _attackFrames.Add(new Bitmap($"boss_attack_{frameNum}", path));
            }
        }

        // Updates zombie state based on whether it's dying, attacking, or walking
        public override void Update()
        {
            if (_isDead) return; // Do nothing if already dead

            base.Update(); // Increments _attackTimer

            if (_isDying)
            {
                _animationTimer++;
                if (_animationTimer >= _animationSpeed)
                {
                    _frameIndex++;
                    _animationTimer = 0;
                    if (_frameIndex >= _dieFrames.Count)
                        _isDead = true; // Mark as dead after death animation
                }
                return; // Skip rest of update if dying
            }

            if (_isAttacking)
            {
                // Handle attack animation progression
                _animationTimer++;
                if (_animationTimer >= _animationSpeed)
                {
                    _frameIndex++;
                    _animationTimer = 0;
                    if (_frameIndex >= _attackFrames.Count)
                    {
                        _isAttacking = false;
                        _frameIndex = 0;
                    }
                }
            }
            else
            {
                // Move left and animate walking
                X -= 1.0f;
                _animationTimer++;
                if (_animationTimer >= _animationSpeed)
                {
                    _frameIndex = (_frameIndex + 1) % _walkFrames.Count;
                    _animationTimer = 0;
                }
            }
        }

        // Draw the zombie based on its current state
        public override void Draw()
        {
            if (_isDead) return;

            if (_isDying)
            {
                if (_frameIndex < _dieFrames.Count)
                    SplashKit.DrawBitmap(_dieFrames[_frameIndex], X, Y);
            }
            else
            {
                DrawHealthBar();

                if (_isAttacking)
                    SplashKit.DrawBitmap(_attackFrames[Math.Min(_frameIndex, _attackFrames.Count - 1)], X, Y);
                else
                    SplashKit.DrawBitmap(_walkFrames[_frameIndex], X, Y);
            }
        }

        // Start attack animation and reset attack timer
        public void TriggerAttack()
        {
            if (_isDying || _isDead) return;

            _isAttacking = true;
            _frameIndex = 0;
            _animationTimer = 0;
            ResetAttackTimer();
        }

        // Apply damage to boss zombie
        public override void TakeDamage(int dmg)
        {
            if (_isDying || _isDead) return;

            HP -= dmg;
            if (HP <= 0)
            {
                HP = 0;
                _isDying = true;
                _frameIndex = 0;
                _animationTimer = 0;
            }
        }

        // Return Y-coordinate based on current lane
        private float GetLaneY(Lane lane)
        {
            return lane switch
            {
                Lane.Top => 315,
                Lane.Bottom => 450,
                _ => 500,
            };
        }

        // Draw health bar above boss zombie
        private void DrawHealthBar()
        {
            float barWidth = 60;
            float barHeight = 8;
            float hpRatio = (float)HP / MaxHP;
            float x = X;
            float y = Y - 12;

            SplashKit.FillRectangle(Color.Black, x - 1, y - 1, barWidth + 2, barHeight + 2);
            SplashKit.FillRectangle(Color.DarkGray, x, y, barWidth, barHeight);

            Color hpColor = hpRatio > 0.5f ? Color.Green : (hpRatio > 0.2f ? Color.Orange : Color.Red);
            SplashKit.FillRectangle(hpColor, x, y, barWidth * hpRatio, barHeight);
        }
    }
}
