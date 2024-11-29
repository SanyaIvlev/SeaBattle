namespace SeaBattle;

public class Player
{
    public int ShipCellsDestroyed = 0;
    
    public bool IsEndedTurn { get; private set; }

    public Field BattleField = new();
    
    public event Action OnCursorPositionChanged; 

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

    public void ProcessInput(Field enemyField)
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
            HandleHumanAction(enemyField);
        }
        else
        {
            HandleBotAction(enemyField);
        }
        
        ref Cell shotCell = ref enemyField.GetCell(ActionPosition.x, ActionPosition.y);
        
        Shoot(ref shotCell);

        if (!shotCell.hasShip)
        {
            IsEndedTurn = true;
        }
    }

    private void HandleHumanAction(Field enemyField)
    {
        (int x, int y) newPosition = _humanAction.GetNextPosition();
        TryToMoveCursor(newPosition);

        (int x, int y) currentPosition = _humanAction.GetCurrentPosition();
        ref Cell enemyCell = ref enemyField.GetCell(currentPosition.x, currentPosition.y);

        bool canShoot = !enemyCell.hasShot;
        
        while (_humanAction.Input != ' ' || !canShoot)
        {
            _humanAction.ProcessInput();
            
            newPosition = _humanAction.GetNextPosition();
            TryToMoveCursor(newPosition);
            
            currentPosition = _humanAction.GetCurrentPosition();
            enemyCell = ref enemyField.GetCell(currentPosition.x, currentPosition.y);
            
            canShoot = !enemyCell.hasShot;
        }
    }
    
    private void TryToMoveCursor((int x, int y) currentPosition)
    {
        var (x, y) = currentPosition;

        if (x < 0 || x >= Field.Width || y < 0 || y >= Field.Height)
            return;
        
        _humanAction.Position = (x,y);

        OnCursorPositionChanged?.Invoke();
    }
    
    private void HandleBotAction(Field enemyField)
    {
        (int x,int y) currentPosition = _botAction.GetPosition();
        ref Cell enemyCell = ref enemyField.GetCell(currentPosition.x, currentPosition.y);

        bool canShoot = !enemyCell.hasShot;

        while (!canShoot)
        {
            _botAction.ProcessAction();
            
            currentPosition = _botAction.GetPosition();
            enemyCell = ref enemyField.GetCell(currentPosition.x, currentPosition.y);

            canShoot = !enemyCell.hasShot;
        }
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