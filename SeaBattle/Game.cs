namespace SeaBattle;

public class Game
{
    private Player _player = new();
    private Bot _bot = new();
    
    private Render _render = new();

    public void Run()
    {
        InitializeFields();
        
        _render.DrawMap(ref _player, ref _bot.BattleField.Cells);
        
        while (!IsGameEnded())
        {
            _player.IsPlayerTurn = true;
            
            while (_player.IsPlayerTurn)
            {
                _player.ProcessInput(ref _bot.BattleField);
                _player.Logic();
                _render.DrawMap(ref _player, ref _bot.BattleField.Cells);
            }
            
            _bot.Logic(ref _player.BattleField);
            
            _render.DrawMap(ref _player, ref _bot.BattleField.Cells);
        }

        if (_player.ShipCellsDestroyed == Field.ShipsCount)
        {
            Console.WriteLine("Player won the game!");
        }
        else if (_bot.ShipCellsDestroyed == Field.ShipsCount)
        {
            Console.WriteLine("Bot won the game! You lost :(");
        }
    }

    private void InitializeFields()
    {
        Field playerField = _player.BattleField;
        playerField.GenerateField();
        
        Field botField = _bot.BattleField;
        botField.GenerateField();
    }
    
    private bool IsGameEnded()
        => _player.ShipCellsDestroyed == Field.ShipsCount || _bot.ShipCellsDestroyed == Field.ShipsCount; 
}