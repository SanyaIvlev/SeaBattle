namespace SeaBattle;

public class Game
{
    private Player _player = new(true);
    private Player _bot = new(false);

    private Player _currentPlayer;
    private Player _currentOpponent;
    
    private Render _render;

    public Game()
    {
        _currentPlayer = _player;
        _currentOpponent = _bot;
        
        _render = new(_player, _bot);
    }

    public void Run()
    {
        GenerateMap();
        DrawMap();

        while (!IsGameEnded())
        {
            GetInput(); 
            Logic();
            DrawMap();
            Wait();
        }
        
        Console.WriteLine("Game ended!");
    }

    private void GenerateMap()
    {
        Field playerField = _player.BattleField;
        playerField.GenerateField();
        
        Field botField = _bot.BattleField;
        botField.GenerateField();
    }

    private void DrawMap()
    {
        _render.DrawMap();
    }
    
    private bool IsGameEnded()
        => _player.ShipCellsDestroyed == Field.ShipsCount || _bot.ShipCellsDestroyed == Field.ShipsCount; 
    
    private void GetInput()
    {
        _currentPlayer.ProcessInput(ref _currentOpponent.BattleField);
    }
    
    private void Logic()
    {
        _currentPlayer.Logic(_currentOpponent.BattleField);
        if (_currentPlayer.IsEndedTurn)
        {
            SwitchPlayer();
        }
    }

    private void SwitchPlayer()
    {
        (_currentPlayer, _currentOpponent) = (_currentOpponent, _currentPlayer);
    }
    
    private void Wait()
    {
        Thread.Sleep(500);
    }

}