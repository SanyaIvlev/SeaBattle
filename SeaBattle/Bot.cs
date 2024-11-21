namespace SeaBattle;

public class Bot
{
    public int ShipCellsDestroyed = 0;
    
    public Field BattleField = new();
    
    private List<(int x, int y)> _checkedShips = new List<(int x, int y)>();
    
    private Random _random = new();

    public void Logic(ref readonly Field playerField)
    {
        (int x, int y) shotCellPosition = GetUncheckedCell(playerField);
        
        ref Cell shotCell = ref playerField.GetCell(shotCellPosition.x, shotCellPosition.y);
        
        Shoot(ref shotCell);
        
        _checkedShips.Add(shotCellPosition);
    }
    
    private (int x, int y) GetUncheckedCell(ref readonly Field playerField)
    {
        (int x, int y) = playerField.GetRandomCell();
        
        while (!IsUncheckedCell(x, y))
        {
            (x, y) = playerField.GetRandomCell();
        }
        
        return (x, y);
    }

    private bool IsUncheckedCell(int x, int y)
    {
        foreach (var ship in _checkedShips)
        {
            if (x == ship.x && y == ship.y)
            {
                return false;
            }
        }

        return true;
    }

    private void Shoot(ref Cell shotCell)
    {
        if (shotCell.hasShip)
        {
            ShipCellsDestroyed++;
        }
        
        shotCell.hasShot = true;
    }
}