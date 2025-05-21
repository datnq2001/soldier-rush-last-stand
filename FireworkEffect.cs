// FireworkEffect.cs - Handles looping firework animations for victory screen effects
using SplashKitSDK;
using System.Collections.Generic;

namespace SoldierRushGame
{
    public class FireworkEffect
    {
        private List<Bitmap> _frames;         // List of animation frames for the firework
        private int _currentFrame = 0;        // Current frame index in animation
        private int _frameTimer = 0;          // Timer to control animation speed
        private float _x, _y;                 // Position of the firework on screen

        // Constructor: load all frames for the selected firework type (e.g., pink, yellow)
        public FireworkEffect(string type, float x, float y)
        {
            _x = x;
            _y = y;
            _frames = new List<Bitmap>();

            // Load 7 sequential frame images from the assets/effects folder
            for (int i = 1; i <= 7; i++)
            {
                _frames.Add(new Bitmap($"{type}_{i}", $"assets/effects/{type}/{i}.png"));
            }
        }

        // Update frame index every 5 ticks to loop animation
        public void Update()
        {
            _frameTimer++;
            if (_frameTimer > 5)
            {
                _currentFrame = (_currentFrame + 1) % _frames.Count;
                _frameTimer = 0;
            }
        }

        // Draw the current frame of the firework at the specified location
        public void Draw()
        {
            SplashKit.DrawBitmap(_frames[_currentFrame], _x, _y);
        }
    }
}