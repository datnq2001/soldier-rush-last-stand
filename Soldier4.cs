//Soldier4.cs
using SplashKitSDK;

namespace SoldierRushGame
{
    public class Soldier4 : Soldier
    {
        public Soldier4(Lane lane, float x = 50) : base(lane, 2, x)
        {
            for (int i = 30; i <= 40; i++)
            {
                string frameNum = i.ToString("D4");
                string path = $"assets/soldiers4/run_shoot/{frameNum}.png";
                _runShootFrames.Add(new Bitmap($"soldier4_{frameNum}", path));
            }
        }
    }
}