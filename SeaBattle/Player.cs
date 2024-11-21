namespace SeaBattle;

public class Player
{
    public int ShipCellsDestroyed = 0;
    
    public Field BattleField = new();

    public bool IsPlayerTurn = true;
    
    private (int x, int y) _position;
    private (int x, int y) _delta;

    
    public (int x, int y) Position => _position;
    
    public void Logic()
    {
        int newX = _position.x + _delta.x;
        int newY = _position.y + _delta.y;
        
        TryToMove(newX, newY);
    }
    
    public void ProcessInput(ref readonly Field enemyField)
    {
        var input = Console.ReadKey();
        var key = input.KeyChar;
        
        ProcessDirection(key);
        ProcessShot(key, enemyField);
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
    
    private void ProcessShot(char input, ref readonly Field enemyField)
    {
        ref Cell shotCell = ref enemyField.GetCell(_position.x, _position.y);
        
        if (input == ' ' && !shotCell.hasShot)
        {
            Shoot(ref shotCell);
        }
    }

    private void Shoot(ref Cell shotCell)
    {
        if (shotCell.hasShip)
        {
            ShipCellsDestroyed++;
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