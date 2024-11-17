namespace SeaBattle;

public class Game
{
    private Player _player = new();
    private Enemy _bot = new();
    
    private Render _render = new();

    public void Run()
    {
        InitializeFields();
        
        while (true)
        {
            _render.DrawMap(ref _player, ref _bot.BattleField.Cells);
            
            _player.IsPlayerTurn = true;
            
            while (_player.IsPlayerTurn)
            {
                _player.ProcessInput(ref _bot.BattleField);
                _player.Logic();
                _render.DrawMap(ref _player, ref _bot.BattleField.Cells);
            }
            
            _bot.Logic(ref _player.BattleField);
        }
    }

    private void InitializeFields()
    {
        Field playerField = _player.BattleField;
        playerField.GenerateField();
        
        Field botField = _bot.BattleField;
        botField.GenerateField();
    }
}