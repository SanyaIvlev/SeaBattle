namespace SeaBattle;

public class BotShotPosition : IShotPosition
{
    private (int x, int y) _position;
    
    private Random _random = new Random();

    public (int x, int y) GetShotPosition()
        => _position;
    
    public void FindShotPositionWithin(int width, int height)
    {
        (int x, int y) randomShotPosition = GetRandomPosition(width, height);

        _position = randomShotPosition;
    }

    private (int x, int y) GetRandomPosition(int width, int height)
    {
        int x = _random.Next(width);
        int y = _random.Next(height);
        
        return (x, y);
    }
}