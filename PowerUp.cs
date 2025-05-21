// PowerUp.cs - Defines collectible power-ups with effects like healing, upgrading, or adding soldiers
using SplashKitSDK;
using System.Collections.Generic;

namespace SoldierRushGame
{
    // Enumeration for the types of power-ups available
    public enum PowerUpType
    {
        Plus1,           // Adds a new soldier
        UpgradeWeapon,   // Increases weapon damage for all soldiers
        Heal             // Restores HP to all soldiers
    }

    public class PowerUp : GameEntity
    {
        public PowerUpType Type { get; private set; }     // Type of this power-up
        private Bitmap _sprite;                            // Visual representation of the power-up
        private const float SoldierSpacing = 64f;          // Horizontal spacing for spawning new soldiers

        private int _stage; // The current game stage, used to determine which soldier to spawn

        // Constructor: sets lane, type, and loads the corresponding sprite
        public PowerUp(Lane lane, PowerUpType type, int stage = 1)
        {
            Lane = lane;
            Type = type;
            _stage = stage;

            X = SplashKit.ScreenWidth() + 75; // Start slightly off-screen
            Y = GetLaneY(lane);

            _sprite = new Bitmap(type.ToString(), $"assets/powerups/{type.ToString().ToLower()}.png");
        }

        // Move the power-up leftward each frame
        public override void Update()
        {
            X -= 1.5f;
        }

        // Draw the power-up sprite
        public override void Draw()
        {
            SplashKit.DrawBitmap(_sprite, X, Y);
        }

        // Apply the power-up effect to the list of soldiers
        public void Apply(List<Soldier> soldiers)
        {
            switch (Type)
            {
                case PowerUpType.Plus1:
                    AddSoldierInFront(soldiers); // Add new soldier based on stage
                    break;

                case PowerUpType.UpgradeWeapon:
                    foreach (var s in soldiers)
                        s.UpgradeWeapon(); // Increase damage
                    break;

                case PowerUpType.Heal:
                    foreach (var s in soldiers)
                        s.Heal(5); // Heal by 5 HP
                    break;
            }
        }

        // Add a soldier ahead of the last soldier in the list based on stage
        private void AddSoldierInFront(List<Soldier> soldiers)
        {
            float startX = soldiers.Count > 0 ? soldiers[^1].X + SoldierSpacing : 50;
            Soldier newSoldier;

            if (_stage == 10)
                newSoldier = new Soldier2(Lane, startX);
            else if (_stage == 15)
                newSoldier = new Soldier3(Lane, startX);
            else if (_stage == 20)
                newSoldier = new Soldier4(Lane, startX);
            else
                return; // Do not add soldier if stage is not 10, 15, or 20

            soldiers.Add(newSoldier);
        }

        // Get the vertical Y coordinate for the lane
        private float GetLaneY(Lane lane)
        {
            return lane switch
            {
                Lane.Top => 315,
                Lane.Bottom => 450,
                _ => 500
            };
        }
    }
}
