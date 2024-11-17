using FieldClass;

namespace SeaBattle;

public class Enemy
{
    public Field BattleField = new();
    
    private (int x, int y)[] _checkedShips = new (int, int)[0];
    private int _shipCellsDestroyed = 0;
    
    private Random _random = new();

    public void Logic()
    {
        (int x, int y) shotCell = GetUncheckedCell();
        Shot(shotCell);
    }s
    
    private (int x, int y) GetUncheckedCell()
    {
        (int x, int y) = BattleField.GetRandomCell();
        
        while (!IsUncheckedCell(x, y))
        {
            (x, y) = BattleField.GetRandomCell();
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

    private void Shot((int x, int y) cell)
    {
        if (BattleField.Cells[cell.y, cell.x].hasShip)
        {
            _shipCellsDestroyed++;
        }
        
        BattleField.Cells[cell.y, cell.x].hasShot = true;
    }
}