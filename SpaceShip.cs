using SplashKitSDK;

public class Spaceship : SpaceObject
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int Speed { get; set; }
    public Window Window { get; set; }
    public int lives { get; private set; }
    private const int TopMargin = 50;
    public List<Projectile> _projectiles;

    public Spaceship(Window window, int x, int y, int width, int height, int speed, string bitmapPath): base(x, y, bitmapPath)
    {
        Width = width;
        Height = height;
        Speed = speed;
        Window = window;
        _projectiles = new List<Projectile>();
        lives = 3;
    }

    public override void Update()
    {
        if (SplashKit.KeyDown(KeyCode.LeftKey))
        {
            Move(-1, 0);
        }
        if (SplashKit.KeyDown(KeyCode.RightKey))
        {
            Move(1, 0);
        }
        if (SplashKit.KeyDown(KeyCode.UpKey))
        {
            Move(0, -1);
        }
        if (SplashKit.KeyDown(KeyCode.DownKey))
        {
            Move(0, 1);
        }
        if (SplashKit.KeyTyped(KeyCode.SpaceKey))
        {
            Shoot();
        }
        UpdateProjectiles();
    }

    private void Shoot()
    {
        _projectiles.Add(new Projectile(X + Width / 2 - 5, Y, -10, "projectile.png"));
    }

    private void UpdateProjectiles()
    {
        foreach (Projectile projectile in _projectiles)
        {
            projectile.Update();
            if (projectile.Y <= TopMargin)
            {
                projectile.IsActive = false;
            }
        }
        _projectiles.RemoveAll(p => !p.IsActive);
    }

    public void DrawProjectiles()
    {
        foreach (Projectile projectile in _projectiles)
        {
            if (projectile.Y > TopMargin)
            {
                projectile.Draw();
            }
        }
    }

    public void Move(int directionX, int directionY)
    {
        int newX = (int)X + Speed * directionX;
        int newY = (int)Y + Speed * directionY;

        
        if (newX >= 0 && newX <= Window.Width - Width)
        {
            X = newX;
        }

        if (newY >= 50 && newY <= Window.Height - Height)
        {
            Y = newY;
        }
    }

    public void LoseLife()
    {
        if (lives > 0)
        {
            lives--;
        }
    }

    public bool IsAlive()
    {
        return lives > 0;
    }

    public void DrawLives(Window window)
    {
        Bitmap heart = new Bitmap("heart","heart.png");
        for (int i = 0; i < lives; i++)
        {
            window.DrawBitmap(heart, 300 + i * 70, 5);
        }
    }

    public void ResetLives()
    {
        lives = 3;
    }

    public void ResetPosition()
    {
        this.X = 350;
        this.Y = 350;
    }

    public void ClearProjectiles()
    {
        this._projectiles.Clear();
    }
}