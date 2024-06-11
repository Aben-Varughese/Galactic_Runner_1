using SplashKitSDK;

namespace Galactic_Runner
{
    public class Program
    {
        public static void Main()
        {
            Window window = new Window("Galactic Runner", 800, 600);
            Bitmap backgroundImage = new Bitmap("background", "background.png");
            float backgroundY = 0;

            Font font = SplashKit.LoadFont("Nasalization", "/Users/abenbinuvarughese/Desktop/nasalization-free/nasalization-rg.otf");
            string title = "Galactic Runner";
            double titleWidth = SplashKit.TextWidth(title, font, 60);
            Spaceship spaceship = new Spaceship(window, 350, 350, 100, 100, 10, "spaceship.png");
            List<Enemy> enemies = new List<Enemy>();

            bool isPlaying = false;

            Button playButton = new Button("Play", 350, 250, 100, 50, () => { isPlaying = true; enemies.Clear();});
            Button quitButton = new Button("Quit", 350, 350, 100, 50, () => window.Close());

            do
            {
                SplashKit.ProcessEvents();
                SplashKit.ClearScreen();

                backgroundY += 10; 
                if (backgroundY >= backgroundImage.Height)
                {
                    backgroundY = 0;
                }
                window.DrawBitmap(backgroundImage, 0, backgroundY - backgroundImage.Height);
                window.DrawBitmap(backgroundImage, 0, backgroundY);

                double titleX = (window.Width - titleWidth) / 2;
                SplashKit.DrawText(title, Color.White, font, 60, titleX, 100);

                playButton.Draw();
                quitButton.Draw();

                playButton.Clicked(); 
                quitButton.Clicked();

                if (isPlaying)
                {
                    GameLoop gameLoop = new GameLoop(window, backgroundImage, font, spaceship);
                    gameLoop.StartLevel(1);
                    gameLoop.ResetGame();
                    isPlaying = false;
                }

                SplashKit.RefreshScreen();
                SplashKit.Delay(100);
            } while (!window.CloseRequested);
        }
    }
}
