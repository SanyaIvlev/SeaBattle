using FieldClass;
using PlayerClass;

namespace SeaBattle;

public class Game
{
    private Player _player = new();
    private Enemy _bot = new();

    public void Run()
    {
        while (true)
        {
            while (_player.IsPlayerTurn)
            {
                _player.ProcessInput();
                _player.Logic();
                
            }
        }
    }
    
    
}