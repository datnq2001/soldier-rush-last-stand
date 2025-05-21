//GameManager.cs
using SplashKitSDK;
using System;
using System.Collections.Generic;

namespace SoldierRushGame
{
    public class GameManager
    {   
        private List<Soldier> _soldiers;  // List of soldiers
        private List<Zombie> _zombies;   // List of zombies
        private List<PowerUp> _powerUps;   // List of power-ups
        private List<Bullet> _bullets;   // List of bullets

        // Game state variables
        private int _score = 0;
        private int _stage = 1;
        private int _playerHP = 3;

        // Timers for controlling zombie spawn, power-up appearance, and firing rate
        private int _spawnTimer = 0;
        private int _powerUpTimer = 0;
        private int _fireRateTimer = 0;

        // Cooldowns for power-ups
        private int _plus1Cooldown = 1200;
        private int _upgradeCooldown = 1200;

        // Flags for power-up spawning
        private bool _plus1GivenStage10 = false;
        private bool _plus1GivenStage15 = false;
        private bool _plus1GivenStage20 = false;

        private float _scrollX = 0;   // Background scrolling position
        private const float ScrollSpeed = 1.5f;   // Speed of background scrolling

        // Random number generator
        // to spawn zombies and power-ups
        // and to determine the lane of the power-up
        // and the lane of the zombie
        // and the lane of the soldier
        // and the lane of the bullet
        private Random _rng = new Random();
        private Bitmap _background = new Bitmap("background_tile", "assets/ui/background.png");

        private bool _gameWon = false; // Flag for game win condition

        public bool GameOver => _playerHP <= 0;  // Flag for game over condition

        // Constructor
        public GameManager()
        {
            _soldiers = new List<Soldier>();
            _zombies = new List<Zombie>();
            _powerUps = new List<PowerUp>();
            _bullets = new List<Bullet>();

            _soldiers.Add(new Soldier1(Lane.Top));
            SpawnZombieWave();
        }

        public void Update()
        {   
            // Check for game over or win conditions
            if (GameOver || _gameWon) return;

            _spawnTimer++;   // Increment spawn timer
            _powerUpTimer++;   // Increment power-up timer
            _fireRateTimer++;    // Increment fire rate timer
            _plus1Cooldown++;   // Increment plus1 cooldown
            _upgradeCooldown++;  // Increment upgrade cooldown

            // Check for stage increase
            if (_score % 300 == 0 && _score > 0)
            {
                _stage++;
                // Check for game win condition
                // If player reaches Stage 31, trigger win condition
                if (_stage > 30)
                {
                    _gameWon = true;
                    return;
                }
            }

            // Spawn power-ups based on stage 
            if (_stage == 10 && !_plus1GivenStage10)
            {
                _powerUps.Add(new PowerUp((Lane)_rng.Next(0, 2), PowerUpType.Plus1, _stage));
                _plus1GivenStage10 = true;
            }

            if (_stage == 15 && !_plus1GivenStage15)
            {
                _powerUps.Add(new PowerUp((Lane)_rng.Next(0, 2), PowerUpType.Plus1, _stage));
                _plus1GivenStage15 = true;
            }

            if (_stage == 20 && !_plus1GivenStage20)
            {
                _powerUps.Add(new PowerUp((Lane)_rng.Next(0, 2), PowerUpType.Plus1, _stage));
                _plus1GivenStage20 = true;
            }

            // Spawn zombies based on stage
            // The spawn interval decreases as the stage increases
            // The maximum number of zombies increases as the stage increases
            int spawnInterval = Math.Max(120, 300 - (_stage * 10));
            int maxZombies = (_stage <= 20) ? 8 : (_stage <= 30 ? 14 : 0);

            if (_spawnTimer > spawnInterval && _zombies.Count < maxZombies && !_gameWon)
            {
                SpawnZombieWave();
                _spawnTimer = 0;
            }

            // Spawn power-ups based on stage
            if (_powerUpTimer > 600)
            {
                SpawnRandomPowerUp();
                _powerUpTimer = 0;
            }

            // Fire bullets based on fire rate
            // The fire rate increases as the stage increases
            if (_fireRateTimer > 20)
            {
                foreach (var s in _soldiers)
                    _bullets.Add(s.Shoot());
                _fireRateTimer = 0;
            }

            // Update all game entities
            foreach (var s in _soldiers) s.Update();
            foreach (var z in _zombies) z.Update();
            foreach (var p in _powerUps) p.Update();
            foreach (var b in _bullets) b.Update();

            // Handle bullet-zombie collisions: if hit, apply damage and deactivate bullet
            HashSet<Zombie> damagedZombies = new HashSet<Zombie>();
            foreach (var bullet in _bullets)
            {
                foreach (var zombie in _zombies)
                {
                    if (!zombie.IsAlive || damagedZombies.Contains(zombie)) continue;

                    if (bullet.IsActive && zombie.IsBlocking && bullet.Lane == zombie.Lane)
                    {
                        if (Math.Abs(bullet.X - zombie.X) < 20)
                        {
                            zombie.TakeDamage(bullet.Damage);
                            damagedZombies.Add(zombie);
                            bullet.Deactivate();
                            break;
                        }
                    }
                }
            }

            // Check for collisions between zombies and soldiers
            // If a zombie hits a soldier, the soldier takes damage
            // and the zombie is deactivated
            foreach (var z in _zombies)
            {
                if (z.X < -50 && z.IsAlive) 
                {
                    _playerHP--;  // Decrease player HP if zombie goes off-screen
                    z.TakeDamage(9999);  // Deactivate zombie
                }
                
                foreach (var s in _soldiers)
                {
                    // Check if the zombie is in the same lane as the soldier
                    // and if the zombie is within attack range
                    // If so, the soldier takes damage
                    // and the zombie attacks
                    if (z.Lane == s.Lane && z.X <= s.X + 64 && z.X >= s.X + 48 && !z.IsDead && !z.IsDying) 
                    {
                        if (z.CanAttack)
                        {
                            int damage = (z is BossZombie) ? 3 : 2;
                            s.TakeDamage(damage);

                            if (z is Zombie1 z1) z1.TriggerAttack();
                            else if (z is BossZombie boss) boss.TriggerAttack();

                            z.ResetAttackTimer();
                        }
                    }
                }
            }

            // Check for collisions between power-ups and soldiers
            // If a soldier picks up a power-up, the power-up is applied
            // and the power-up is removed from the game
            // The power-up can be a heal, upgrade, or plus1
            for (int i = _powerUps.Count - 1; i >= 0; i--)
            {
                PowerUp p = _powerUps[i];
                foreach (var s in _soldiers)
                {
                    if (p.Lane == s.Lane && Math.Abs(p.X - s.X) < 30)
                    {
                        p.Apply(_soldiers);
                        _powerUps.RemoveAt(i);
                        break;
                    }
                }
            }

            
            _zombies.RemoveAll(z => !z.IsAlive); // Remove dead zombies
            _bullets.RemoveAll(b => !b.IsActive); // Remove inactive bullets
            _soldiers.RemoveAll(s => s.IsDead); // Remove dead soldiers

            // Check if all soldiers are dead
            if (_soldiers.Count == 0) _playerHP = 0;

            _score++; // Increment score
            // Scroll the background
            // The background scrolls to the left
            // The scroll speed is constant 
            _scrollX += ScrollSpeed;
            if (_scrollX >= _background.Width)
                _scrollX -= _background.Width;
        }

        // Draw the game entities
        public void Draw()
        {
            SplashKit.DrawBitmap(_background, -_scrollX, 0);
            SplashKit.DrawBitmap(_background, _background.Width - _scrollX, 0);

            foreach (var s in _soldiers) s.Draw();
            foreach (var z in _zombies) z.Draw();
            foreach (var p in _powerUps) p.Draw();
            foreach (var b in _bullets) b.Draw();

            DrawHUD();
        }

        // Draw the HUD (Heads-Up Display)
        // The HUD displays the player's HP, score, and stage
        // If the game is over, it displays "GAME OVER"
        // If the player wins, it displays "YOU WIN!"
        private void DrawHUD()
        {
            SplashKit.DrawText($"HP: {_playerHP}", Color.White, "Arial", 20, 20, 20);
            SplashKit.DrawText($"Score: {_score}", Color.White, "Arial", 20, 20, 50);
            SplashKit.DrawText($"Stage: {_stage}", Color.White, "Arial", 20, 20, 80);

            if (GameOver)
            {
                SplashKit.DrawText("GAME OVER", Color.Red, "Arial", 32, 300, 300);
            }
            else if (_gameWon)
            {
                SplashKit.DrawText("YOU WIN!", Color.LimeGreen, "Arial", 32, 300, 300);
            }
        }

        // Spawn a wave of zombies
        private void SpawnZombieWave()
        {
            int hp = 3 + (_stage / 5);    // Base HP for zombies
            int bossChance = 0;   // Chance to spawn a boss zombie
            if (_stage >= 5) bossChance = 10;
            if (_stage >= 10) bossChance = 20;
            if (_stage >= 15) bossChance = 30;
            if (_stage >= 20) bossChance = 40;

            // The number of zombies increases as the stage increases
            int zombieCount = 1 + (_stage / 5);

            float startX = SplashKit.ScreenWidth() + 30; // Starting X position for zombies

            // Spawn zombies in random lanes
            for (int i = 0; i < zombieCount; i++)
            {
                Lane currentLane = (Lane)_rng.Next(0, 2);
                float spawnX = startX + i * 40;

                Zombie zombie;
                // Randomly decide whether to spawn a boss zombie
                if (_rng.Next(100) < bossChance) 
                {
                    int bossHP = 20 + (_stage * 2);
                    zombie = new BossZombie(currentLane, bossHP);
                }
                else
                {
                    zombie = new Zombie1(currentLane, hp);
                }
                
                zombie.X = spawnX;
                _zombies.Add(zombie);
            }
        }

        // Spawn a random power-up
        private void SpawnRandomPowerUp()
        {
            List<PowerUpType> available = new List<PowerUpType>(); // List of available power-ups
            
            //Conditions to add power-ups to the available list
            if (_upgradeCooldown >= 1200)
                available.Add(PowerUpType.UpgradeWeapon); 

            if (_plus1Cooldown >= 1200 && (_stage == 10 || _stage == 15 || _stage == 20))
                available.Add(PowerUpType.Plus1);

            if (_stage >= 12 && _soldiers.Count > 0)
                available.Add(PowerUpType.Heal);

            if (available.Count == 0) return;

            // Randomly select a power-up type from the available list
            // and a random lane (0 or 1)
            PowerUpType type = available[_rng.Next(available.Count)];
            Lane lane = (Lane)_rng.Next(0, 2);

            // Spawn the power-up
            if (type == PowerUpType.Plus1)
                _powerUps.Add(new PowerUp(lane, type, _stage));
            else
                _powerUps.Add(new PowerUp(lane, type));

            if (type == PowerUpType.Plus1) _plus1Cooldown = 0;
            if (type == PowerUpType.UpgradeWeapon) _upgradeCooldown = 0;
        }
  
        public int Score => _score;
        public bool GameWon => _gameWon;

        public int Stage => _stage;

        public void MoveSoldiersUp()
        {
            foreach (var s in _soldiers)
                if (s.Lane > Lane.Top) s.Lane--; // Move soldier up
        }

        public void MoveSoldiersDown()
        {
            foreach (var s in _soldiers)
                if (s.Lane < Lane.Bottom) s.Lane++; // Move soldier down
        }
    }
}
