using System.Net;

namespace SeaBattle;

public class Player
{
    public int ShipCellsDestroyed = 0;
    
    public bool IsEndedTurn { get; private set; }

    public Field BattleField = new();

    public bool IsHuman;
    public (int x, int y) Position;

    private IAction _action;

    public Player(bool isHuman)
    {
        if (isHuman)
        {
            _action = new HumanAction();
        }
        else
        {
            _action = new BotAction();
        }
        
        IsHuman = isHuman;
    }

    public void ProcessInput()
    {
        _action.ProcessAction();
    }
    
    public void Logic(Field enemyField)
    {
        IsEndedTurn = false;

        var actionPosition = _action.GetPosition();

        if (IsHuman && actionPosition != (0, 0))
        {
            TryToMoveCursor(Position.x + actionPosition.x, Position.y + actionPosition.y);
            return;
        }
        
        if(!IsHuman)
            Position = actionPosition;
        
                
        Thread.Sleep(500);
        
        ref Cell shotCell = ref enemyField.GetCell(Position.x, Position.y);
        
        TryShoot(ref shotCell);

        if (!shotCell.hasShip)
        {
            IsEndedTurn = true;
        }
    }

    private void TryToMoveCursor(int x, int y)
    {
        if (x < 0 || x >= Field.Width || y < 0 || y >= Field.Height)
            return;

        Position = (x, y);
    }

    private void TryShoot(ref Cell shotCell)
    {
        if (shotCell.hasShot)
        {
            return;
        }
        
        if (shotCell.hasShip)
        {
            ShipCellsDestroyed++;
        }
        
        shotCell.hasShot = true;
    }
}