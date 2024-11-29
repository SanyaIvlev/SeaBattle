using System.Net;

namespace SeaBattle;

public class Player
{
    public int ShipCellsDestroyed = 0;
    
    public bool IsEndedTurn { get; private set; }

    public Field BattleField = new();

    public bool IsHuman;
    public (int x, int y) ActionPosition
    {
        get
        {
            if(IsHuman) 
                return _humanAction.GetCurrentPosition();
            
            return _botAction.GetPosition();
        }
    } 
    private HumanAction _humanAction;
    private BotAction _botAction;

    public Player(bool isHuman)
    {
        if (isHuman)
        {
            _humanAction = new HumanAction();
            _botAction = null;
        }
        else
        {
            _botAction = new BotAction();
            _humanAction = null;
        }
        
        IsHuman = isHuman;
    }

    public void ProcessInput()
    {
        if(IsHuman) 
            _humanAction.ProcessInput();
        else 
            _botAction.ProcessAction();
    }
    
    public void Logic(Field enemyField)
    {
        IsEndedTurn = false;

        if (IsHuman)
        {
            if (_humanAction.Input == ' ')
            {
                (int x, int y) currentPosition = _humanAction.GetCurrentPosition();
                Cell enemyCell = enemyField.GetCell(currentPosition.x, currentPosition.y);

                if (enemyCell.hasShot)
                {
                    return;
                }
            }
            else
            {
                (int x, int y) = _humanAction.GetNextPosition();
                
                TryToMoveCursor(x, y);
                return;
            }
        }
        else
        {
            var botPosition = _botAction.GetPosition();
            Cell botShootingCell = enemyField.GetCell(botPosition.x, botPosition.y);

            if (botShootingCell.hasShot)
            {
                return;
            }
            
            
        }
        
        Thread.Sleep(500);
        
        ref Cell shotCell = ref enemyField.GetCell(ActionPosition.x, ActionPosition.y);
        
        Shoot(ref shotCell);

        if (!shotCell.hasShip)
        {
            IsEndedTurn = true;
        }
    }

    private void TryToMoveCursor(int x, int y)
    {
        if (x < 0 || x >= Field.Width || y < 0 || y >= Field.Height)
            return;
        
        _humanAction.Position = (x,y);
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