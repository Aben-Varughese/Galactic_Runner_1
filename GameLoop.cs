using Galactic_Runner;
using Galactic_Runner_1;
using SplashKitSDK;

public class GameLoop
{
    private Window _window;
    private Bitmap _backgroundImage;
    private Font _font;
    private Spaceship _spaceship;
    private List<Enemy> _enemies;
    private const int TopMargin = 50;
    private SplashKitSDK.Timer _spawnTimer;
    private bool _isPaused, _flashEffect, DisplayLevel;
    public Button pauseButton, unpauseButton, quitButton;
    public int _currentLevel, _enemiesDefeated, _spawnInterval, _score;
    private HighScore _highScore;
    private GameState _currentState;

    public GameLoop(Window window, Bitmap backgroundImage, Font font, Spaceship spaceship)
    {
        _window = window;
        _backgroundImage = backgroundImage;
        _font = font;
        _spaceship = spaceship;
        _enemies = new List<Enemy>();
        _spawnTimer = SplashKit.CreateTimer("enemySpawner");
        _isPaused = false;
        pauseButton = new Button("Pause", 10, 10, 80, 30, () => _currentState = GameState.Paused);
        unpauseButton = new Button("Resume", 10, 10, 80, 30, () => _currentState = GameState.Running);
        quitButton = new Button("Quit", 350, 400, 100, 50, () => _window.Close());
        _currentLevel = 1;
        _enemiesDefeated = 0;
        _spawnInterval = 3000;
        _score = 0;
        _highScore = new HighScore("HighScore.txt");
        _currentState = GameState.Running;
    }

    public void StartLevel(int level)
    {
        _currentLevel = level;
        _spawnInterval = 3000 - (level - 1) * 200;
        DisplayLevel = true;
        SplashKit.StartTimer(_spawnTimer);
        SplashKit.ResetTimer(_spawnTimer);
        Run();
    }

    public void ResetGame()
    {
        _spaceship.ResetLives();
        _spaceship.ResetPosition();
        _spaceship.ClearProjectiles();
        _currentLevel = 1;
        _enemies.Clear();
        StartLevel(_currentLevel);
    }

    public void Run()
    {
        float backgroundY = 0;
        do
        {
            SplashKit.ProcessEvents();
            SplashKit.ClearScreen();

            if (_flashEffect)
            {
                _window.Clear(Color.Red);
                _flashEffect = false;
            }
            else if (DisplayLevel)
            {
                DisplayLevelStartMessage(_currentLevel);
                DisplayLevel = false;
            }
            else
            {
                backgroundY += 10;
                if (backgroundY >= _backgroundImage.Height)
                {
                    backgroundY = 0;
                }
                _window.DrawBitmap(_backgroundImage, 0, backgroundY - _backgroundImage.Height);
                _window.DrawBitmap(_backgroundImage, 0, backgroundY);
            }

            DrawTopMargin();
            _spaceship.DrawLives(_window);

            switch (_currentState)
            {
                case GameState.Running:
                    pauseButton.Draw();
                    if (pauseButton.Clicked())
                    {
                        _currentState = GameState.Paused;
                    }
                    if (_currentState == GameState.GameOver)
                    {
                        DisplayGameOver();
                        SplashKit.Delay(3000);
                        break;
                    }
                    UpdateGame();
                    break;

                case GameState.Paused:
                    PauseGame();
                    unpauseButton.Draw();
                    quitButton.Draw();
                    if (unpauseButton.Clicked())
                    {
                        _currentState = GameState.Running;
                    }
                    if (quitButton.Clicked())
                    {
                        _window.Close();
                    }
                    break;

                case GameState.GameOver:
                    DisplayGameOver();
                    SplashKit.Delay(3000);
                    break;
            }

            SplashKit.RefreshScreen();
            SplashKit.Delay(100);
        } while (!_window.CloseRequested && _currentState != GameState.GameOver);
    }

    private void UpdateGame()
    {
        HandleEnemySpawning();

        for (int i = _enemies.Count - 1; i > 0; i--)
        {
            Enemy enemy = _enemies[i];

            enemy.Update();
            enemy.Draw();

            if (!enemy.IsActive || CheckCollisions(enemy, _spaceship))
            {
                _enemies.RemoveAt(i);
                _score += 50;
            }
        }

        _spaceship.Draw();
        _spaceship.Update();
        _spaceship.DrawProjectiles();
    }

    private void HandleEnemySpawning()
    {
        if (SplashKit.TimerTicks(_spawnTimer) > _spawnInterval && !_isPaused)
        {
            _enemies.Add(EnemyFactory.CreateEnemy(_currentLevel, _window.Width));
            SplashKit.ResetTimer(_spawnTimer);
        }
    }

    private bool CheckCollisions(Enemy enemy, Spaceship spaceship)
    {
        if (enemy.Y + enemy.Height >= _window.Height)
        {
            spaceship.LoseLife();
            _flashEffect = true;
            return true;
        }

        if (!spaceship.IsAlive())
        {
            //_gameOver = true;
            _currentState = GameState.GameOver;
        }

        if (enemy.CheckCollision(spaceship))
        {
            spaceship.LoseLife();
            _flashEffect = true;
            if (!spaceship.IsAlive())
            {
                //_gameOver = true;
                _currentState = GameState.GameOver;
            }
            enemy.Destroy();
            return true;
        }

        if (_enemiesDefeated >= 10 * _currentLevel)
        {
            StartLevel(_currentLevel + 1);
        }

        foreach (Projectile projectile in spaceship._projectiles)
        {
            if (projectile.IsActive && projectile.CheckCollision(enemy))
            {
                enemy.Destroy();
                projectile.IsActive = false;
                _enemiesDefeated++;
                return false;
            }
            
        }

        foreach (Projectile projectile in enemy._projectiles)
        {
            if (projectile.IsActive && projectile.CheckCollision(spaceship))
            {
                spaceship.LoseLife();
                _flashEffect = true;
                projectile.IsActive = false;
                if (!spaceship.IsAlive())
                {
                    //_gameOver = true;
                    _currentState = GameState.GameOver;
                }
            }
        }

        return false;
    }

    private void DisplayGameOver()
    {
        string title = "GAME OVER";
        double titleWidth = SplashKit.TextWidth(title, _font, 60);
        double titleX = (_window.Width - titleWidth) / 2;
        _window.DrawBitmap(_backgroundImage, 0, 0);
        SplashKit.DrawText(title, Color.Red, _font, 60, titleX, 250);

        string prompt = "Enter your name:";
        Rectangle inputRect = new Rectangle
        {
            X = 400,
            Y = 350,
            Width = 200,
            Height = 20
        };
        SplashKit.StartReadingText(_window, inputRect, "");

        while (SplashKit.ReadingText(_window) && !SplashKit.TextEntryCancelled(_window))
        {
            SplashKit.ProcessEvents();
            _window.Clear(Color.Black);
            _window.DrawBitmap(_backgroundImage, 0, 0);
            SplashKit.DrawText(title, Color.Red, _font, 60, titleX, 250);
            SplashKit.DrawText(prompt, Color.White, _font, 20, 200, 350);
            SplashKit.DrawText(SplashKit.TextInput(_window), Color.White, _font, 20, inputRect.X, inputRect.Y);
            SplashKit.RefreshScreen();
        }

        string playerName = SplashKit.TextInput(_window);
        SplashKit.EndReadingText(_window);

        bool isHighScore = _highScore.IsHighScore(_score);
        _highScore.AddScore(playerName, _score);
        if (isHighScore)
        {
            string text = "Congrats! You have achieved a new high score!";
            SplashKit.DrawText(text, Color.White, _font, 20, 150, 390);
            SplashKit.RefreshScreen();
            SplashKit.Delay(3000);
        }

        DisplayLeaderBoard();

        SplashKit.RefreshScreen();
        SplashKit.Delay(100);
    }

    private void DisplayLeaderBoard()
    {
        SplashKit.ClearScreen();
        _window.Clear(Color.Black);
        string title = "LEADER BOARD";
        double titleWidth = SplashKit.TextWidth(title, _font, 60);
        double titleX = (_window.Width - titleWidth) / 2;
        _window.DrawBitmap(_backgroundImage, 0, 0);
        SplashKit.DrawText(title, Color.White, _font, 60, titleX, 50);
        _highScore.DisplayHighScores(_window, _font);
        SplashKit.RefreshScreen();
        SplashKit.Delay(3000);
    }

    private void PauseGame()
    {
        string title = "PAUSED";
        double titleWidth = SplashKit.TextWidth(title, _font, 60);
        double titleX = (_window.Width - titleWidth) / 2;
        _window.DrawBitmap(_backgroundImage, 0, 0);
        SplashKit.DrawText(title, Color.Red, _font, 60, titleX, 250);
    }

    private void DrawTopMargin()
    {
        Color marginColor = Color.Black;
        SplashKit.FillRectangle(marginColor, 0, 0, _window.Width, TopMargin);
        string scoreText = $"Score: {_score}";
        double scoreWidth = SplashKit.TextWidth(scoreText, _font, 20);
        double scoreX = _window.Width - scoreWidth - 10;
        SplashKit.DrawText(scoreText, Color.White, _font, 20, scoreX, 10);
    }

    private void DisplayLevelStartMessage(int level)
    {
        string message = $"Level {level} Start!";
        double messageWidth = SplashKit.TextWidth(message, _font, 40);
        double messageX = (_window.Width - messageWidth) / 2;
        double messageY = (_window.Height - 40) / 2;

        SplashKit.ClearScreen();
        _window.DrawBitmap(_backgroundImage, 0, 0);
        SplashKit.DrawText(message, Color.White, _font, 40, messageX, messageY);
        SplashKit.RefreshScreen();
        SplashKit.Delay(2000); // Display the message for 2 seconds
    }

    public void TriggerFlashEffect()
    {
        _flashEffect = true;
    }
}
