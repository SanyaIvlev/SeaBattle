namespace SeaBattle;
public class HumanShotPosition : IShotPosition
{
    private (int x, int y) _position;
    private (int x, int y) _delta;

    public (int x, int y) GetShotPosition()
        => _position;
    
    
    public void FindShotPositionWithin(int width, int height)
    {
        var input = Console.ReadKey();
        var key = input.KeyChar;
        
        while (key != ' ')
        {
            input = Console.ReadKey();
            key = input.KeyChar;
            
            ProcessDirection(key);
            
            int newX = _position.x + _delta.x;
            int newY = _position.y + _delta.y;
            
            TryChangePosition(newX, newY, width, height);
        }
    }
    
    private void ProcessDirection(char input)
    {
        _delta = (0, 0);

        _delta = input switch
        {
            'W' or 'w' => (0, -1),
            'A' or 'a' => (-1, 0),
            'S' or 's' => (0, 1),
            'D' or 'd' => (1, 0),
            _ => (0, 0)
        };
    }

    private void TryChangePosition(int newX, int newY, int width, int height)
    {
        if (newX >= 0 && newY >= 0 && newX < width && newY < height)
        {
            _position.x = newX;
            _position.y = newY;
        }
    }
}