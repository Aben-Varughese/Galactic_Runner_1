using Galactic_Runner_1;
using SplashKitSDK;

public abstract class SpaceObject : IDrawable, IUpdatable
{
    public float X { get; protected set; }
    public float Y { get; protected set; }
    public Bitmap Bitmap { get; private set; }

    protected SpaceObject(float x, float y, string bitmapPath)
    {
        X = x;
        Y = y;
        Bitmap = SplashKit.LoadBitmap(bitmapPath, bitmapPath);
    }

    public abstract void Update();

    public virtual void Draw()
    {
        if (Bitmap != null)
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
        }
    }

    public bool CheckCollision(SpaceObject other)
    {
        return SplashKit.BitmapCollision(Bitmap, X, Y, other.Bitmap, other.X, other.Y);
    }
}

