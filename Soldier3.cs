//Soldier3.cs
using SplashKitSDK;

namespace SoldierRushGame
{
    public class Soldier3 : Soldier
    {
        public Soldier3(Lane lane, float x = 50) : base(lane, 2, x)
        {
            for (int i = 30; i <= 40; i++)
            {
                string frameNum = i.ToString("D4");
                string path = $"assets/soldiers3/run_shoot/{frameNum}.png";
                _runShootFrames.Add(new Bitmap($"soldier3_{frameNum}", path));
            }
        }
    }
}