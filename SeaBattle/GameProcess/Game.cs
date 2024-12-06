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
        
        Init();
        
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
        => new Player(true, name);

    private Player CreateBot(string name)
        => new Player(false, name);
    
    public void Init()
    {
        _player1.ShipCellsDestroyed = 0;
        _player2.ShipCellsDestroyed = 0;
        
        _currentPlayer = _player1;
        _currentOpponent = _player2;
        
        GenerateMap();
        
        DrawMap();
    }

    private void RunGameCycle()
    {
        while (!IsGameEnded())
        {
            ProcessInput(); 
            Logic();
            DrawMap();
        }

        DrawResults();
    }

    private void GenerateMap()
    {
        Field playerField = _player1.BattleField;
        playerField.GenerateField();
        
        Field botField = _player2.BattleField;
        botField.GenerateField();
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
        _currentPlayer.Logic(_currentOpponent.BattleField);
        
        if (_currentPlayer.ShipCellsDestroyed == Field.ShipsCount)
        {
            EndThisRound();
        }
        else if (_currentPlayer.IsEndedTurn)
        {
            SwitchPlayer();
        }
    }

    private void EndThisRound()
    {
        _currentPlayer.RoundsWon++;
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n" + _currentPlayer.Name + " wins this round!!!");
        
        Thread.Sleep(2000);
        
        if(!IsGameEnded())
            Init();
    }

    private void SwitchPlayer()
    {
        (_currentPlayer, _currentOpponent) = (_currentOpponent, _currentPlayer);
    }
    
    private void DrawResults()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        
        Console.Write("\n" + _currentPlayer.Name + " has won the game!");
    }


}