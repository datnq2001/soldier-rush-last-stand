// Zombie1.cs - Standard zombie enemy with walk, attack, and die animations
using SplashKitSDK;
using System.Collections.Generic;

namespace SoldierRushGame
{
    public class Zombie1 : Zombie
    {
        private List<Bitmap> _walkFrames = new List<Bitmap>();   // Walk animation frames
        private List<Bitmap> _dieFrames = new List<Bitmap>();    // Die animation frames
        private List<Bitmap> _attackFrames = new List<Bitmap>(); // Attack animation frames

        private int _frameIndex = 0;       // Current animation frame index
        private int _animationTimer = 0;   // Timer to control animation speed
        private int _animationSpeed = 5;   // Speed of animation updates

        private bool _isAttacking = false; // Flag to indicate if zombie is attacking

        // Constructor initializes zombie state, position, and loads animation frames
        public Zombie1(Lane lane, int hp)
        {
            Lane = lane;
            HP = hp;
            MaxHP = hp;
            IsBoss = false;
            X = SplashKit.ScreenWidth() - 50; // Start from right side of screen
            Y = GetLaneY(lane);

            // Load walk animation frames
            for (int i = 1; i <= 15; i++)
            {
                string frameNum = i.ToString("D4");
                string filePath = $"assets/zombies/walk/{frameNum}.png";
                _walkFrames.Add(new Bitmap($"zombie_walk_{frameNum}", filePath));
            }

            // Load die animation frames
            for (int i = 130; i <= 150; i++)
            {
                string frameNum = i.ToString("D4");
                string filePath = $"assets/zombies/die/{frameNum}.png";
                _dieFrames.Add(new Bitmap($"zombie_die_{frameNum}", filePath));
            }

            // Load attack animation frames
            for (int i = 460; i <= 470; i++)
            {
                string frameNum = i.ToString("D4");
                string filePath = $"assets/zombies/attack/{frameNum}.png";
                _attackFrames.Add(new Bitmap($"zombie_attack_{frameNum}", filePath));
            }
        }

        // Update zombie state (movement, animation, attack, dying)
        public override void Update()
        {
            if (_isDead) return; // Skip update if already dead

            base.Update(); // Increment attack timer

            if (_isDying)
            {
                // Play dying animation frame by frame
                _animationTimer++;
                if (_animationTimer >= _animationSpeed)
                {
                    _frameIndex++;
                    _animationTimer = 0;
                    if (_frameIndex >= _dieFrames.Count)
                        _isDead = true;
                }
                return;
            }

            if (_isAttacking)
            {
                // Play attack animation
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
                // Move zombie forward and play walk animation
                X -= 1.5f;
                _animationTimer++;
                if (_animationTimer >= _animationSpeed)
                {
                    _frameIndex = (_frameIndex + 1) % _walkFrames.Count;
                    _animationTimer = 0;
                }
            }
        }

        // Draw zombie on screen based on current state
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

        // Trigger attack animation sequence
        public void TriggerAttack()
        {
            if (_isDying || _isDead) return;

            _isAttacking = true;
            _frameIndex = 0;
            _animationTimer = 0;
            ResetAttackTimer();
        }

        // Apply damage to the zombie
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

        // Get vertical position based on lane
        private float GetLaneY(Lane lane)
        {
            return lane switch
            {
                Lane.Top => 315,
                Lane.Bottom => 450,
                _ => 500,
            };
        }

        // Draw zombie health bar above its head
        private void DrawHealthBar()
        {
            float barWidth = 40;
            float barHeight = 6;
            float hpRatio = (float)HP / MaxHP;
            float x = X + 64 - (barWidth / 2);
            float y = Y - 10;

            SplashKit.FillRectangle(Color.Black, x - 1, y - 1, barWidth + 2, barHeight + 2);
            SplashKit.FillRectangle(Color.DarkGray, x, y, barWidth, barHeight);

            Color hpColor = hpRatio > 0.5f ? Color.Green : (hpRatio > 0.2f ? Color.Orange : Color.Red);
            SplashKit.FillRectangle(hpColor, x, y, barWidth * hpRatio, barHeight);
        }
    }
}
