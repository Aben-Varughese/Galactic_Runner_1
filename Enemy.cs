using SplashKitSDK;

public class Enemy : SpaceObject
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Speed { get; private set; }
    private bool _isDestroyed;
    private Bitmap _destroyBitmap;
    private SplashKitSDK.Timer _destroyTimer;
    private SplashKitSDK.Timer shootTimer;
    public List<Projectile> _projectiles;
    public bool IsActive { get; set; }
    private int _minShootInterval;
    private int _maxShootInterval;

    public Enemy(int x, int y, int width, int height, int speed, string bitmapPath)
        : base(x, y, bitmapPath)
    {
        Width = width;
        Height = height;
        Speed = speed;
        _projectiles = new List<Projectile>();
        _isDestroyed = false;
        _destroyBitmap = new Bitmap("destroy", "destroy.png");
        _destroyTimer = SplashKit.CreateTimer("enemy_destroy");
        shootTimer = SplashKit.CreateTimer("enemy_shoot");
        IsActive = true;
        _minShootInterval = 1500; 
        _maxShootInterval = 2200; 
        SplashKit.StartTimer(_destroyTimer);
        SplashKit.StartTimer(shootTimer);
    }

    public override void Update()
    {
        if (!_isDestroyed)
        {
            Y += Speed;
            if (SplashKit.TimerTicks(shootTimer) % GetRandomShootInterval() == 0) 
            {
                Shoot();
                SplashKit.ResetTimer(shootTimer);
                SplashKit.StartTimer(shootTimer);
            }
        }
        else
        {
            if (SplashKit.TimerTicks(_destroyTimer) > 300) 
            {
                IsActive = false;
                SplashKit.StopTimer(_destroyTimer);
            }
        }

        UpdateProjectiles();
    }

    public void Destroy()
    {
        _isDestroyed = true;
        SplashKit.ResetTimer(_destroyTimer);
        SplashKit.StartTimer(_destroyTimer);

    }

    private int GetRandomShootInterval()
    {
        return _minShootInterval + SplashKit.Rnd(_maxShootInterval - _minShootInterval);
    }

    public override void Draw()
    {
        if (!_isDestroyed)
        {
            base.Draw();
            DrawProjectiles();
        }
        else
        {
            if (IsActive)
            {
                SplashKit.DrawBitmap(_destroyBitmap, X, Y);
            }
        }

    }

    private void Shoot()
    {
        _projectiles.Add(new Projectile(X + Width / 2 + 15, Y + Height, 15, "enemy_proj.png"));
    }

    private void UpdateProjectiles()
    {
        foreach (Projectile projectile in _projectiles)
        {
            projectile.Update();
        }
        _projectiles.RemoveAll(p => !p.IsActive);
    }

    public void DrawProjectiles()
    {
        foreach (Projectile projectile in _projectiles)
        {
            projectile.Draw();
        }
    }


}
