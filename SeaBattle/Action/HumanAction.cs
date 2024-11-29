namespace SeaBattle;
public class HumanAction
{
    public char Input { get; private set; }

    public (int x, int y) Position { private get; set; }
    private (int x, int y) _delta;

    public (int x, int y) GetCurrentPosition()
        => (Position.x, Position.y);

    public (int x, int y) GetNextPosition()
    {
        int x = Position.x + _delta.x;
        int y = Position.y + _delta.y;

        return (x, y);
    }

    public void ProcessInput()
    {
        var input = Console.ReadKey();
        var key = input.KeyChar;
        
        _delta = (0, 0);

        _delta = key switch
        {
            'W' or 'w' => (0, -1),
            'A' or 'a' => (-1, 0),
            'S' or 's' => (0, 1),
            'D' or 'd' => (1, 0),
            _ => (0, 0)
        };
        
        Input = key;

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
}