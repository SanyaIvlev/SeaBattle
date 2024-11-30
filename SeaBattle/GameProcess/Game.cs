namespace SeaBattle;

public class Game
{
    private Player _player1 = new(true); // !! поки перемикати PvP, PvE, EvE можна змінюючи цей аргумент (true - людина, false - бот) 
    private Player _player2 = new(false); // !! поки перемикати PvP, PvE, EvE можна змінюючи цей аргумент (true - людина, false - бот) 

    private Player _currentPlayer;
    private Player _currentOpponent;

    public void Start()
    {
        RunGameCycle();
    }
    
    private void RunGameCycle()
    {
        Init();

        while (!IsGameEnded())
        {
            ProcessInput(); 
            Logic();
            DrawMap();
        }

        DrawResults();
    }

    private void Init()
    {
        _currentPlayer = _player1;
        _currentOpponent = _player2;
        
        GenerateMap();
        
        DrawMap();
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
        //Console.Write("  Player1's field \t\tPlayer2's field\n\n");

        string horizontalFieldLabel = "";

        for (int i = 0; i < Field.Width; i++)
        {
            char currentLetter = (char)('A' + i); 
            horizontalFieldLabel += currentLetter + " ";
        }
        
        Console.Write("  " + horizontalFieldLabel + "\t\t" + horizontalFieldLabel + "\n");
        
        (bool player1FieldVisibility, bool player2FieldVisibility) = GetFieldsVisibility(_player1, _player2); 
        
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
    
    
    private (bool player1FieldVisibility, bool player2FieldVisibility) GetFieldsVisibility(Player player1, Player player2)
    {
        if (player1.IsHuman && player2.IsHuman)
        {
            return (false, false);
        }
        
        if (!player1.IsHuman && !player2.IsHuman)
        {
            return (true, true);
        }

        if (player1.IsHuman && !player2.IsHuman)
        {
            return (true, false);
        }

        return (false, true);
    }
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
            
            if (opponent.ActionPosition == cellPosition)
            {
                characteristic.color = ConsoleColor.Green;
            }
        }
        else if (opponent.ActionPosition == cellPosition)
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
        => _player1.ShipCellsDestroyed == Field.ShipsCount || _player2.ShipCellsDestroyed == Field.ShipsCount; 
    
    private void ProcessInput()
    {
        _currentPlayer.ProcessInput();
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
    
    private void DrawResults()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        
        if (_player1.ShipCellsDestroyed == Field.ShipsCount)
        {
            Console.WriteLine("Player 1 has won the game!");
        }
        else if (_player2.ShipCellsDestroyed == Field.ShipsCount)
        {
            Console.WriteLine("Player 2 has won the game!");
        }
    }


}