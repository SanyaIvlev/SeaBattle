namespace SeaBattle;

public class BotAction : IAction
{
    public (int x, int y) Position;
    
    private Random _random = new Random();

    public (int x, int y) GetPosition()
        => Position;
    
    public void ProcessAction()
    {
        int x = _random.Next(Field.Width);
        int y = _random.Next(Field.Height);
        
        Position = (x, y);
    }
}