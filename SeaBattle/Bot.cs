namespace SeaBattle;

public class Bot
{
    public int ShipCellsDestroyed = 0;
    
    public Field BattleField = new();
    
    private List<(int x, int y)> _checkedShips = new List<(int x, int y)>();
    
    private Random _random = new();

    public void Logic(ref readonly Field playerField)
    {
        (int x, int y) shotCell = GetUncheckedCell(playerField);
        Shot(shotCell, playerField);
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

    private void Shot((int x, int y) cell, ref readonly Field playerField)
    {
        Cell shotCell = playerField.Cells[cell.y, cell.x];
        
        if (shotCell.hasShip)
        {
            ShipCellsDestroyed++;
        }
        
        playerField.Cells[cell.y, cell.x].hasShot = true;
        
        _checkedShips.Add(cell);
    }
}