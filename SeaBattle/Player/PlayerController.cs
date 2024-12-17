using System.Text;

namespace SeaBattle;

public class PlayerController
{
    public int DecksDestroyed;
    
    public bool IsEndedTurn { get; private set; }

    public Field BattleField = new();

    public bool IsHuman;
    public (int x, int y) Position;
    
    private IAction _action;

    public PlayerController(bool isHuman)
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
        _action.Process();
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
        
        Cell shotCell = enemyField.GetCell(Position.x, Position.y);
        
        TryShoot(enemyField, shotCell);

        if (!shotCell.hasShip && !shotCell.hasShot)
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

    private void TryShoot(Field enemyField, Cell shotCell)
    {
        if (shotCell.hasShot)
        {
            return;
        }
        
        if (shotCell.hasShip)
        {
            DecksDestroyed++;
        }
    
        enemyField.Shoot(Position.x, Position.y);
    }
}