using CellClass;
using FieldClass;

namespace PlayerClass;

public class Player
{
    public Field BattleField = new();

    public bool IsPlayerTurn = true;
    
    private (int x, int y) _position;
    private (int x, int y) _delta;

    private int _shipCellsDestroyed = 0;

    public void Logic()
    {
        int newX = _position.x + _delta.x;
        int newY = _position.y + _delta.y;
        
        TryToMove(newX, newY);
    }
    
    public void ProcessInput()
    {
        var input = Console.ReadKey();
        var key = input.KeyChar;
        
        ProcessDirection(key);
        ProcessShot(key);
    }

    public void DrawField()
    {
        for (int i = 0; i < Field.Height; i++)
        {
            for (int j = 0; j < Field.Width; j++)
            {
                
            }
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
    
    private void ProcessShot(char input)
    {
        if (input == ' ' && !BattleField.Cells[_position.y, _position.x].hasShot)
        {
            Shot();
        }
    }

    private void Shot()
    {
        Cell shotCell = BattleField.Cells[_position.y, _position.x];
        
        if (shotCell.hasShip)
        {
            _shipCellsDestroyed++;
        }
        
        shotCell.hasShot = true;
        IsPlayerTurn = false;

    }
    
    
    private void TryToMove(int newX, int newY)
    {
        if (newX >= 0 && newY >= 0 && newX < Field.Width && newY < Field.Height)
        {
            _position.x = newX;
            _position.y = newY;
        }
    }
}