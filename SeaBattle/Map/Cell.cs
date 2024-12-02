namespace SeaBattle;

public struct Cell
{
    public bool hasShip;
    public bool hasShot;

    public void Shoot()
    {
        hasShot = true;
    }
}