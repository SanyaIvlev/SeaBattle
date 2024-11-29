namespace SeaBattle;

public class BotAction
{
    public (int x, int y) Position { private get; set; }

    private Random _random = new Random();

    public (int x, int y) GetPosition()
        => Position;
    
    public void ProcessAction()
    {
        (int x, int y) randomShotPosition = GetRandomPosition();

        Position = randomShotPosition;
    }

    private (int x, int y) GetRandomPosition()
    {
        int x = _random.Next(Field.Width);
        int y = _random.Next(Field.Height);
        
        return (x, y);
    }
}