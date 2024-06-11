public class Projectile : SpaceObject
{
    public float Speed { get; private set; }
    public bool IsActive { get; set; }

    public Projectile(float x, float y, float speed, string bitmapPath): base(x, y, bitmapPath)
    {
        Speed = speed;
        IsActive = true;
    }

    public override void Update()
    {
        Y += Speed; 
        if (Y < 0) IsActive = false; 
    }
}
