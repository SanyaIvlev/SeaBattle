namespace SeaBattle;

public class Player
{
    public int ShipCellsDestroyed = 0;
    public bool IsEndedTurn { get; private set; }

    public Field BattleField = new();
    
    private IShotPosition _shotPosition;

    public Player(bool isHuman)
    {
        if (isHuman)
        {
            _shotPosition = new HumanShotPosition();
        }
        else
        {
            _shotPosition = new BotShotPosition();
        }
    }

    public void ProcessInput(ref Field enemyField)
    {
        _shotPosition.FindShotPositionWithin(Field.Width, Field.Height);
        
        var currentShotPosition = _shotPosition.GetShotPosition();
        ref Cell currentCell = ref enemyField.GetCell(currentShotPosition.x, currentShotPosition.y); 
        
        while (currentCell.hasShot)
        {
            _shotPosition.FindShotPositionWithin(Field.Width, Field.Height);
            
            currentShotPosition = _shotPosition.GetShotPosition();
            currentCell = ref enemyField.GetCell(currentShotPosition.x, currentShotPosition.y);
        }
        
    }
    
    public void Logic(ref Field enemyField)
    {
        IsEndedTurn = false;
        
        (int x, int y) shotPosition = _shotPosition.GetShotPosition();
        ref Cell shotCell = ref enemyField.GetCell(shotPosition.x, shotPosition.y);
        
        Shoot(ref shotCell);

        if (!shotCell.hasShip)
        {
            IsEndedTurn = true;
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