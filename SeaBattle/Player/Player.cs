using System.Runtime.InteropServices.JavaScript;

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
                return _actionForHuman.GetCurrentPosition();
            
            return _actionForBot.GetPosition();
        }
    } 
    private HumanAction _actionForHuman;
    private BotAction _actionForBot;

    public Player(bool isHuman)
    {
        if (isHuman)
        {
            _actionForHuman = new HumanAction();
            _actionForBot = null;
        }
        else
        {
            _actionForBot = new BotAction();
            _actionForHuman = null;
        }
        
        IsHuman = isHuman;
    }

    public void ProcessInput(ref Field enemyField)
    {
        if(IsHuman) 
            _actionForHuman.ProcessInput();
        else 
            _actionForBot.ProcessAction();
    }
    
    public void Logic(Field enemyField)
    {
        IsEndedTurn = false;

        if (IsHuman)
        {
            HandleHumanAction();
        }
        else
        {
            HandleBotAction();
        }
        
        ref Cell shotCell = ref enemyField.GetCell(ActionPosition.x, ActionPosition.y);
        
        Shoot(ref shotCell);

        if (!shotCell.hasShip)
        {
            IsEndedTurn = true;
        }
    }

    private void HandleHumanAction()
    {
        (int x, int y) newPosition = _actionForHuman.GetNextPosition();
        TryToMoveCursor(newPosition);
        
        while (_actionForHuman.Input != ' ')
        {
            _actionForHuman.ProcessInput();
            
            newPosition = _actionForHuman.GetNextPosition();
            TryToMoveCursor(newPosition);
        }
    }
    
    private void TryToMoveCursor((int x, int y) currentPosition)
    {
        ;
        var (x, y) = currentPosition;

        if (x < 0 || x >= Field.Width || y < 0 || y >= Field.Height)
            return;
        
        _actionForHuman.Position = (x,y);

        OnCursorPositionChanged?.Invoke();
    }
    
    private void HandleBotAction()
    {
        bool isActionResultValid = false;

        while (!isActionResultValid)
        {
            _actionForBot.ProcessAction();
            isActionResultValid = GetResultValidity();
        }
        
        (int x,int y) currentPosition = _actionForBot.GetPosition();
        _actionForBot.Position = currentPosition;
    }

    private bool GetResultValidity()
    {
        (int x,int y) currentPosition = _actionForBot.GetPosition();
        ref Cell currentCell = ref BattleField.GetCell(currentPosition.x, currentPosition.y);

        if (currentCell.hasShot)
            return false;

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