// Soldier2.cs - Advanced soldier with higher damage and unique animation set
using SplashKitSDK;

namespace SoldierRushGame
{
    public class Soldier2 : Soldier
    {
        // Constructor: creates Soldier2 with increased damage (2) and loads specific animation frames
        public Soldier2(Lane lane, float x = 50) : base(lane, 2, x)
        {
            // Load animation frames from the soldier2 asset folder
            for (int i = 30; i <= 40; i++)
            {
                string frameNum = i.ToString("D4");
                string path = $"assets/soldiers2/run_shoot/{frameNum}.png";
                _runShootFrames.Add(new Bitmap($"soldier2_{frameNum}", path));
            }
        }
    }
}