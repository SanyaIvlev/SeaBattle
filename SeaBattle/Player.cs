namespace SeaBattle;

public class Player
{
    public int ShipCellsDestroyed = 0;
    
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
    
    public void Logic()
    {
        _shotPosition.FindShotPositionWithin(Field.Width, Field.Height);
        
        var currentShotPosition = _shotPosition.GetShotPosition();
        ref Cell currentCell = ref BattleField.GetCell(currentShotPosition.x, currentShotPosition.y); 
        
        while (currentCell.hasShot)
        {
            currentShotPosition = _shotPosition.GetShotPosition();
            currentCell = ref BattleField.GetCell(currentShotPosition.x, currentShotPosition.y);
        }
        
        Shoot(ref currentCell);
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