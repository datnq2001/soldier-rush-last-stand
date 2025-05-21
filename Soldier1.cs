// Soldier1.cs - Basic soldier class with default damage and animation
using SplashKitSDK;

namespace SoldierRushGame
{
    public class Soldier1 : Soldier
    {
        // Constructor: initializes a Soldier1 with default damage (1) and loads animation frames
        public Soldier1(Lane lane, float x = 50) : base(lane, 1, x)
        {
            // Load run-and-shoot animation frames from asset folder
            for (int i = 30; i <= 40; i++)
            {
                string frameNum = i.ToString("D4");
                string path = $"assets/soldiers/run_shoot/{frameNum}.png";
                _runShootFrames.Add(new Bitmap($"soldier1_{frameNum}", path));
            }
        }
    }
}