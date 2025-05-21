// LambGroupEffect.cs - Simple animated effect that cycles visibility between three lamb images
using SplashKitSDK;
using System;
using System.Collections.Generic;

namespace SoldierRushGame
{
    public class LambGroupEffect
    {
        private List<Bitmap> _lambs;                        // List of lamb image frames
        private List<(float x, float y)> _positions;        // Screen positions for each lamb
        private int _invisibleIndex = 0;                    // Index of the lamb that is hidden
        private int _timer = 0;                             // Timer to control animation speed

        // Constructor: load lamb bitmaps and define their positions
        public LambGroupEffect()
        {
            _lambs = new List<Bitmap>()
            {
                new Bitmap("lamb-1", "assets/effects/lamb-1.png"),
                new Bitmap("lamb-2", "assets/effects/lamb-2.png"),
                new Bitmap("lamb-3", "assets/effects/lamb-3.png")
            };

            // Set screen positions for lambs, spaced apart horizontally
            _positions = new List<(float x, float y)>
            {
                (240, 480),
                (462, 480),
                (684, 480)
            };
        }

        // Update the effect: cycle the invisible lamb every 40 frames
        public void Update()
        {
            _timer++;
            if (_timer > 40) // Switch the invisible lamb index periodically
            {
                _invisibleIndex = (_invisibleIndex + 1) % 3;
                _timer = 0;
            }
        }

        // Draw lambs, hiding one at a time to create a blinking effect
        public void Draw()
        {
            for (int i = 0; i < 3; i++)
            {
                if (i != _invisibleIndex)
                    SplashKit.DrawBitmap(_lambs[i], _positions[i].x, _positions[i].y);
            }
        }
    }
}
