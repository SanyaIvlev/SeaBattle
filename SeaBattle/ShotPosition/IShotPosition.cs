namespace SeaBattle;

public interface IShotPosition
{
    void FindShotPositionWithin(int width, int height);
    (int x, int y) GetShotPosition();
}