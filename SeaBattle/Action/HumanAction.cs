namespace SeaBattle;
public class HumanAction : IAction
{
    private (int x, int y) _delta;

    public (int x, int y) GetPosition()
        => _delta;

    public void ProcessAction()
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
    }
}