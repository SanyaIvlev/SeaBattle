namespace SeaBattle;

public enum Gamemode
{
    PvP,
    PvE,
    EvE,
}

public class Game
{
    private Gamemode _gamemode;

    private Player _player1;
    private Player _player2;

    private Player _currentPlayer;
    private Player _currentOpponent;

    public void Start(Gamemode gamemode)
    {
        _gamemode = gamemode;
        SetGameMode();
        
        RegenerateMap();
        
        DrawMap();
        
        RunGameCycle();
    }

    private void SetGameMode()
    {
        (_player1, _player2) = _gamemode switch
        {
            Gamemode.PvP => (CreatePlayer("Player1"), CreatePlayer("Player2")),
            Gamemode.PvE => (CreatePlayer("Player1"), CreateBot("Player2")),
            Gamemode.EvE => (CreateBot("Player1"), CreateBot("Player2")),
        };
    }

    private Player CreatePlayer(string name)
        => new(true, name);

    private Player CreateBot(string name)
        => new(false, name);

    private void RegenerateMap()
    {
        _player1.ShipCellsDestroyed = 0;
        _player2.ShipCellsDestroyed = 0;
        
        _currentPlayer = _player1;
        _currentOpponent = _player2;
        
        GenerateMap();
    }

    private void RunGameCycle()
    {
        while (!IsGameEnded())
        {
            ProcessInput(); 
            Logic();
            DrawMap();
            Wait();
        }

        DrawResults();
    }

    private void GenerateMap()
    {
        Field player1Field = _player1.BattleField;
        player1Field.Generate();
        
        Field player2Field = _player2.BattleField;
        player2Field.Generate();
    }

    private void DrawMap()
    {
        Console.Clear();
        DrawFields();
    }
    
    private void DrawFields()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write(_player1.Name + "\t\t\t\t" + _player2.Name + "\n");
        
        string horizontalFieldLabel = "";

        for (int i = 0; i < Field.Width; i++)
        {
            char currentLetter = (char)('A' + i); 
            horizontalFieldLabel += currentLetter + " ";
        }
        
        Console.Write("  " + horizontalFieldLabel + "\t\t" + horizontalFieldLabel + "\n");
        
        (bool player1FieldVisibility, bool player2FieldVisibility) = GetFieldsVisibility(); 
        
        for (int i = 0; i < Field.Height; i++)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            WriteRowNumber(i + 1);
            
            Field currentField = _player1.BattleField;
            
            for (int j = 0; j < Field.Width; j++)
            {
                var cellCharacteristics = GetCellCharacteristics((j,i), currentField, _player2, player1FieldVisibility);

                Console.ForegroundColor = cellCharacteristics.color;
                Console.Write(cellCharacteristics.value + " ");
            }
            
            Console.Write("\t\t");
            
            currentField = _player2.BattleField;
            
            for (int j = 0; j < Field.Width; j++)
            {
                var cellCharacteristics = GetCellCharacteristics((j,i), currentField, _player1, player2FieldVisibility);
                
                Console.ForegroundColor = cellCharacteristics.color;
                Console.Write(cellCharacteristics.value + " ");
            }
            
            Console.WriteLine();
        }
    }

    private void WriteRowNumber(int i)
    {
        if (i < 10)
        {
            Console.Write(" " + i);
        }
        else
        {
            Console.Write(i);
        }
    }


    private (bool player1FieldVisibility, bool player2FieldVisibility) GetFieldsVisibility()
        => _gamemode switch
        {
            Gamemode.PvP => (false, false),
            Gamemode.PvE => (true, false),
            Gamemode.EvE => (true, true),
        };
    
    private (char value, ConsoleColor color) GetCellCharacteristics((int x, int y) cellPosition, Field currentField, Player opponent, bool areShipsVisible)
    {
        Cell cell = currentField.GetCell(cellPosition.x, cellPosition.y);

        (char value, ConsoleColor color) characteristic = ('.', ConsoleColor.White);
        
        if (cell.hasShot)
        {
            if (cell.hasShip)
            {
                characteristic = ('X', ConsoleColor.Red);
            }
            else
            {
                characteristic = ('\u00a4', ConsoleColor.Blue);
            }
            
            if (opponent.Position == cellPosition)
            {
                characteristic.color = ConsoleColor.Green;
            }
        }
        else if (opponent.Position == cellPosition)
        {
            characteristic = ('*', ConsoleColor.Green);
        }


        else if (cell.hasShip && areShipsVisible)
        {
            characteristic = ('#', ConsoleColor.Yellow);
        }

        return characteristic;
    }
    
    private bool IsGameEnded()
        => _player1.RoundsWon == 3 || _player2.RoundsWon == 3; 
    
    private void ProcessInput()
    {
        _currentPlayer.ProcessInput();
    }
    
    private void Logic()
    {
        if (NeedRegenerateFields())
        {
            _currentPlayer.RoundsWon++;
            RegenerateMap();
            
            return;
        }
        
        _currentPlayer.Logic(_currentOpponent.BattleField);
        
        if (_currentPlayer.IsEndedTurn)
        {
            SwitchPlayer();
        }
    }

    private bool NeedRegenerateFields()
        => _currentPlayer.ShipCellsDestroyed == Field.ShipsCount;

    private void SwitchPlayer()
    {
        (_currentPlayer, _currentOpponent) = (_currentOpponent, _currentPlayer);
    }
    
    private void Wait()
    {
        Thread.Sleep(500);
    }
    
    private void DrawResults()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        
        Console.Write("\n" + _currentPlayer.Name + " has won the game!");
    }


}