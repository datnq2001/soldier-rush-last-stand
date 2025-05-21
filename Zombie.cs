// Zombies.cs - Abstract base class for all zombie types
using SplashKitSDK;

namespace SoldierRushGame
{
    public abstract class Zombie : GameEntity
    {
        public int HP { get; protected set; }                  // Current health points
        public int MaxHP { get; protected set; }               // Maximum health points

        public bool IsAlive => !_isDead;                       // Returns true if zombie is alive
        public bool IsTargetable => !_isDying && !_isDead && HP > 0; // Can be targeted by attacks
        public bool IsBlocking => !_isDead && !_isDying;       // Blocks bullets if not dying or dead
        public bool IsBoss { get; protected set; }             // True if this zombie is a boss

        protected bool _isDying = false;                       // Is currently dying animation
        protected bool _isDead = false;                        // Is completely dead and inactive

        public bool IsDying => _isDying;                       // Public getter for dying state
        public bool IsDead => _isDead;                         // Public getter for dead state

        protected int _attackTimer = 0;                        // Timer to manage attack cooldown

        // Returns true if the zombie is ready to attack
        public virtual bool CanAttack => _attackTimer >= 60;

        // Reset the attack timer to start cooldown again
        public virtual void ResetAttackTimer()
        {
            _attackTimer = 0;
        }

        // Increment internal timers and update zombie status
        public override void Update()
        {
            _attackTimer++;
        }

        // Must be implemented by specific zombie type to render animation
        public abstract override void Draw();

        // Must be implemented to define how zombie takes damage
        public abstract void TakeDamage(int dmg);
    }
}
