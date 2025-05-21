//Program.cs
using SplashKitSDK;
using System;
using System.Collections.Generic;

namespace SoldierRushGame
{
    // Enum to represent the different game states
    public enum GameState
    {
        StartMenu,
        Playing,
        Paused,
        GameOver,
        GameWon,
        HowToPlay
    }

    public class Program
    {   
        // Static variables to manage game state and resources
        static bool _victorySoundPlayed = false;   // Flag to check if victory sound has been played
        static bool _bgMusicPlaying = false;    // Flag to check if background music is playing
        static bool _isMuted = false;       // Flag to check if the game is muted

        static SoundEffect _bgMusic;  // Background music sound effect
        static List<FireworkEffect> _fireworks = new List<FireworkEffect>();    // List of firework effects
        static LambGroupEffect _lambGroup;  // Lamb group effect for victory screen

        static Bitmap _startMenuBg;   // Background image for the start menu
        static Bitmap _startButton;   // Start button image
        static Bitmap _startButtonHover;    // Start button hover image
        static float _startBtnX = 250;    // X position of the start button
        static float _startBtnY = 430;   // Y position of the start button

        public static void Main()
        {   
            // Initialize the game window
            Window gameWindow = new Window("Soldier Rush: Last Stand", 1024, 683);

            // Load the background music and play it
            _bgMusic = new SoundEffect("bg_music", "assets/audio/background_music.mp3");
            _bgMusic.Play();
            _bgMusicPlaying = true;
            
            // Load the start menu background and button images
            _startMenuBg = new Bitmap("start_menu_bg", "assets/ui/start_menu_bg.png");
            _startButton = new Bitmap("start_button", "assets/ui/start_button.png");
            _startButtonHover = new Bitmap("start_button_hover", "assets/ui/start_button_hover.png");

            // Set the initial game state
            // Initialize the game manager
            GameState state = GameState.StartMenu;
            GameManager game = new GameManager();

            // Main game loop
            while (!gameWindow.CloseRequested)
            {
                SplashKit.ProcessEvents();
                SplashKit.ClearScreen(Color.Black);

                switch (state)
                {   
                    // Handle the different game states
                    case GameState.StartMenu:
                        DrawStartMenu();
                        _victorySoundPlayed = false;  // Reset the victory sound flag

                        // Check if the background music is playing and play it if not muted
                        if (!_bgMusicPlaying && !_isMuted)
                        {
                            _bgMusic.Play();
                            _bgMusicPlaying = true;
                        }

                        // Check if the start button is clicke
                        // and start the game if clicked
                        // Check if the How To Play button is clicked
                        // and show the How To Play screen if clicked
                        if (SplashKit.MouseClicked(MouseButton.LeftButton))
                        {
                            float mx = SplashKit.MouseX();
                            float my = SplashKit.MouseY();

                            if (mx >= _startBtnX && mx <= _startBtnX + _startButton.Width &&
                                my >= _startBtnY && my <= _startBtnY + _startButton.Height)
                            {
                                game = new GameManager();
                                state = GameState.Playing;
                            }

                            if (mx >= 920 && mx <= 1020 && my >= 20 && my <= 50)
                            {
                                state = GameState.HowToPlay;
                            }
                        }
                        break;

                    // Show the How To Play screen
                    case GameState.HowToPlay:
                        DrawHowToPlay();
                        if (SplashKit.KeyTyped(KeyCode.EscapeKey))
                        {
                            state = GameState.StartMenu;
                        }
                        break;

                    // Handle the game playing state
                    case GameState.Playing:
                        // Check for user input to move soldiers or pause the game
                        if (SplashKit.KeyTyped(KeyCode.UpKey)) game.MoveSoldiersUp();
                        if (SplashKit.KeyTyped(KeyCode.DownKey)) game.MoveSoldiersDown();
                        if (SplashKit.KeyTyped(KeyCode.EscapeKey)) state = GameState.Paused;

                        game.Update();
                        game.Draw();

                        // Draw the pause button
                        SplashKit.FillRectangle(Color.LightGray, 940, 20, 60, 30);
                        SplashKit.DrawText("Menu", Color.Black, "Arial", 14, 950, 27);

                        // Check if the pause button is clicked
                        if (SplashKit.MouseClicked(MouseButton.LeftButton))
                        {
                            float mx = SplashKit.MouseX();
                            float my = SplashKit.MouseY();
                            if (mx >= 940 && mx <= 1000 && my >= 20 && my <= 50)
                            {
                                state = GameState.Paused;
                            }
                        }
                        
                        // Check if the game is over or won
                        // and update the game state accordingly
                        if (game.GameOver) state = GameState.GameOver;
                        else if (game.GameWon) state = GameState.GameWon;
                        break;

                    // Handle the paused state
                    case GameState.Paused:
                        game.Draw();
                        DrawPauseMenu();
                        // Check for user input to resume the game or close the game
                        if (SplashKit.MouseClicked(MouseButton.LeftButton))
                        {
                            float mx = SplashKit.MouseX();
                            float my = SplashKit.MouseY();
                            // Check if the resume button is clicked
                            if (mx >= 300 && mx <= 420 && my >= 200 && my <= 240)
                                state = GameState.Playing;
                            // Check if the replay button is clicked
                            else if (mx >= 300 && mx <= 420 && my >= 260 && my <= 300)
                            {
                                game = new GameManager();
                                state = GameState.Playing;
                            }
                            // Check if the mute/unmute button is clicked
                            else if (mx >= 300 && mx <= 420 && my >= 320 && my <= 360)
                            {
                                _isMuted = !_isMuted;
                                if (_isMuted)
                                {
                                    _bgMusic.Stop();
                                    _bgMusicPlaying = false;
                                }
                                else if (!_bgMusicPlaying)
                                {
                                    _bgMusic.Play();
                                    _bgMusicPlaying = true;
                                }
                            }
                            // Check if the close button is clicked
                            else if (mx >= 300 && mx <= 420 && my >= 380 && my <= 420)
                                state = GameState.Playing;
                        }
                        break;

                    // Handle the game over state
                    case GameState.GameOver:
                        DrawGameOverScreen(game);
                        if (SplashKit.KeyTyped(KeyCode.SpaceKey))
                        {
                            state = GameState.StartMenu; // Return to the start menu
                        }
                        break;

                    // Handle the game won state
                    case GameState.GameWon:
                        if (!_victorySoundPlayed)
                        {
                            _bgMusic.Stop();        // Stop the background music
                            _bgMusicPlaying = false;    // Set the flag to false
                            // Play the victory sound
                            SoundEffect victorySound = new SoundEffect("victory", "assets/audio/victory_sound.wav");
                            victorySound.Play();

                            _victorySoundPlayed = true;
                            // Initialize the fireworks and lamb group effects
                            _fireworks.Clear();
                            _fireworks.Add(new FireworkEffect("pink", 200, 120));
                            _fireworks.Add(new FireworkEffect("purple", 450, 160));
                            _fireworks.Add(new FireworkEffect("yellow", 700, 240));

                            _lambGroup = new LambGroupEffect(); // Initialize the lamb group effect
                        }

                        // Draw the fireworks and lamb group effects
                        foreach (var fw in _fireworks)
                        {
                            fw.Update();
                            fw.Draw();
                        }

                        // Draw the victory screen
                        DrawVictoryScreen(game);

                        _lambGroup.Update();
                        _lambGroup.Draw();

                        // Check if the replay button is clicked
                        // Check if the exit button is clicked
                        // and handle the respective actions
                        if (SplashKit.MouseClicked(MouseButton.LeftButton))
                        {
                            float mx = SplashKit.MouseX();
                            float my = SplashKit.MouseY();

                            if (mx >= 370 && mx <= 490 && my >= 400 && my <= 440)
                            {
                                game = new GameManager();
                                _victorySoundPlayed = false;

                                _bgMusic.Stop();
                                if (!_isMuted)
                                {
                                    _bgMusic.Play();
                                    _bgMusicPlaying = true;
                                }

                                state = GameState.Playing;
                            }

                            if (mx >= 530 && mx <= 650 && my >= 400 && my <= 440)
                            {
                                gameWindow.Close();
                            }
                        }
                        break;
                }

                SplashKit.RefreshScreen(60);
            }

            gameWindow.Close();
        }

        // Draw the start menu screen
        private static void DrawStartMenu()
        {
            SplashKit.DrawBitmap(_startMenuBg, 0, 0);

            SplashKit.DrawText("SOLDIER RUSH", Color.Yellow, "Arial", 64, 280, 50);
            SplashKit.DrawText("LAST STAND", Color.White, "Arial", 48, 320, 120);

            string[] lines = {
                "You are the last guardian of a peaceful village hidden in the woods.",
                "When night falls, an eerie mist rises and brings with it the undead.",
                "You must stand your ground, soldier, and defend the innocent from the encroaching horror.",
                "Fight valiantly through 31 relentless stages.",
                "Only then will your courage be legend.",
                "Survive. Protect. Endure.",
                "",
                "Click the Start button below to begin your mission."
            };

            float lineY = 200;
            foreach (string line in lines)
            {
                SplashKit.DrawText(line, Color.White, "Arial", 18, 100, lineY);
                lineY += 30;
            }

            float mx = SplashKit.MouseX();
            float my = SplashKit.MouseY();

            bool isHover = (mx >= _startBtnX && mx <= _startBtnX + _startButton.Width &&
                            my >= _startBtnY && my <= _startBtnY + _startButton.Height);

            if (isHover)
                SplashKit.DrawBitmap(_startButtonHover, _startBtnX, _startBtnY);
            else
                SplashKit.DrawBitmap(_startButton, _startBtnX, _startBtnY);

            SplashKit.FillRectangle(Color.SkyBlue, 920, 20, 100, 30);
            SplashKit.DrawText("HOW TO PLAY", Color.Black, "Arial", 14, 925, 28);
        }

        // Draw the How To Play screen
        private static void DrawHowToPlay()
        {
            SplashKit.FillRectangle(Color.Black, 0, 0, 1024, 683);
            SplashKit.DrawText("HOW TO PLAY", Color.White, "Arial", 36, 400, 40);

            string[] lines = {
                "Controls:",
                "- Use UP / DOWN arrows to move soldiers between lanes.",
                "- Soldiers shoot automatically.",
                "- Press ESC to open the pause menu.",
                "",
                "Power-ups:",
                "- Plus1: Adds a new soldier (Soldier2 at Stage 10, Soldier3 at 15, Soldier4 at 20).",
                "- UpgradeWeapon: Boosts weapon power of all soldiers.",
                "- Heal: Heals all soldiers by 5 HP.",
                "",
                "Survive to Stage 31 to win. Good luck!"
            };

            float y = 120;
            foreach (string line in lines)
            {
                SplashKit.DrawText(line, Color.White, "Arial", 20, 100, y);
                y += 30;
            }

            SplashKit.DrawText("Press ESC to return", Color.LightGray, "Arial", 20, 420, 640);
        }

        // Draw the game over screen
        private static void DrawGameOverScreen(GameManager game)
        {
            SplashKit.DrawText("GAME OVER", Color.Red, "Arial", 40, 250, 180);
            SplashKit.DrawText($"Final Score: {game.Score}", Color.White, "Arial", 24, 280, 250);
            SplashKit.DrawText($"Stage Reached: {game.Stage}", Color.White, "Arial", 24, 270, 290);
            SplashKit.DrawText("Press SPACE to return to main menu", Color.Yellow, "Arial", 20, 190, 360);
        }

        // Draw the victory screen
        private static void DrawVictoryScreen(GameManager game)
        {
            SplashKit.DrawText("YOU WIN!", Color.LimeGreen, "Arial", 64, 320, 180);
            SplashKit.DrawText($"Final Score: {game.Score}", Color.White, "Arial", 28, 380, 270);
            SplashKit.DrawText($"Stage Reached: {game.Stage}", Color.White, "Arial", 28, 380, 310);

            SplashKit.FillRectangle(Color.LightGreen, 370, 400, 120, 40);
            SplashKit.DrawText("REPLAY", Color.Black, "Arial", 20, 395, 410);

            SplashKit.FillRectangle(Color.IndianRed, 530, 400, 120, 40);
            SplashKit.DrawText("EXIT", Color.White, "Arial", 20, 565, 410);
        }

        // Draw the pause menu
        private static void DrawPauseMenu()
        {
            SplashKit.FillRectangle(Color.DarkSlateGray, 250, 150, 300, 300);
            SplashKit.DrawText("PAUSED", Color.White, "Arial", 28, 330, 160);

            SplashKit.FillRectangle(Color.LightGreen, 300, 200, 120, 40);
            SplashKit.DrawText("Resume", Color.Black, "Arial", 20, 325, 210);

            SplashKit.FillRectangle(Color.Orange, 300, 260, 120, 40);
            SplashKit.DrawText("Replay", Color.Black, "Arial", 20, 330, 270);

            SplashKit.FillRectangle(Color.LightBlue, 300, 320, 120, 40);
            SplashKit.DrawText(_isMuted ? "Unmute" : "Mute", Color.Black, "Arial", 20, 325, 330);

            SplashKit.FillRectangle(Color.LightGray, 300, 380, 120, 40);
            SplashKit.DrawText("Close", Color.Black, "Arial", 20, 335, 390);
        }
    }
}
