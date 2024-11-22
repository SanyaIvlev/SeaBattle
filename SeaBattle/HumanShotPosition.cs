namespace SeaBattle;

public class HumanShotPosition : IShotPosition
{
    private (int x, int y) _position;

    public (int x, int y) GetShotPosition()
        => _position;
    
    
    public void FindShotPositionWithin(int width, int height)
    {
        int x = -1, y = -1;
        
        while (x is -1 || y is -1)
        {
            var input = Console.ReadLine();
            
            if (input.Length < 2)
            {
                continue;
            }
            
            char xLetter = input[0];
            x = ConvertXToInt(xLetter, width);
            
            char yLetter = input[1];
            y = ConvertYToInt(yLetter, height);
        }
        
        _position = (x, y);
    }

    private int ConvertXToInt(char x, int width)
    {
        int endLetter = width + 'A';

        if (x < 'A' && x > (char)endLetter)
            return -1;

        return x - 'A';

    }

    private int ConvertYToInt(char y, int height)
    {
        int endLetter = height + '0' - 1;

        if (y < '0' && y > (char)endLetter)
            return -1;

        return y - '0';
    }
}