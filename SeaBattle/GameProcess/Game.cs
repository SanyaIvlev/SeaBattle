namespace SeaBattle;

public class Game
{
    private Player _player1 = new(true);
    private Player _player2 = new(false);

    private Player _currentPlayer;
    private Player _currentOpponent;

    public Game()
    {
        _currentPlayer = _player1;
        _currentOpponent = _player2;
        
        if (_player1.IsHuman)
        {
            _player1.OnCursorPositionChanged += DrawMap;
        }
        if(_player2.IsHuman)
        {
            _player2.OnCursorPositionChanged += DrawMap;
        }
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
        Console.Write("Player1's field \tPlayer2's field\n\n");

        string horizontalFieldLabel = "";

        for (int i = 0; i < Field.Width; i++)
        {
            char currentLetter = (char)('A' + i); 
            horizontalFieldLabel += currentLetter;
        }
        
        Console.Write("  " + horizontalFieldLabel + "\t\t" + horizontalFieldLabel + "\n");
        
        for (int i = 0; i < Field.Height; i++)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            WriteRowNumber(i + 1);
            
            for (int j = 0; j < Field.Width; j++)
            {
                Console.ForegroundColor = GetCellColor((j,i), _player1, _player2);
                var value = GetCellValue((j,i), _player1, _player2);
                
                Console.Write(value);
            }
            
            Console.Write("\t\t");
            
            for (int j = 0; j < Field.Width; j++)
            {
                Console.ForegroundColor = GetCellColor((j,i), _player2, _player1);
                var value = GetCellValue((j,i), _player2, _player1);
                
                Console.Write(value);
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
    
    private ConsoleColor GetCellColor((int x, int y) cellPosition, Player player, Player opponent)
    {
        var areShipsVisible = player.IsHuman;
        
        Field currentField = player.BattleField;
        Cell cell = currentField.GetCell(cellPosition.x, cellPosition.y);
        
        if (cell.hasShot)
        {
            if (cell.hasShip)
            {
                return ConsoleColor.Red;
            }

            return  ConsoleColor.Blue;
        }
        
        if (opponent.ActionPosition == cellPosition)
        {
            return ConsoleColor.Green;
        }

        if (cell.hasShip && areShipsVisible)
        {
            return ConsoleColor.Yellow;
        }

        if (cell.hasShip && !areShipsVisible)
        {
            return ConsoleColor.White;
        }

        return ConsoleColor.White;
    }
    
    private char GetCellValue((int x, int y) cellPosition, Player player, Player opponent)
    {
        var areShipsVisible = player.IsHuman;
        
        Field currentField = player.BattleField;
        Cell cell = currentField.GetCell(cellPosition.x, cellPosition.y);
        
        
        if (cell.hasShot)
        {
            if (cell.hasShip)
            {
                return 'X';
            }

            return '\u00a4';
        }
        if (opponent.ActionPosition == cellPosition)
        {
            return '*';
        }
        if (cell.hasShip && areShipsVisible)
        {
            return '#';
        }
        
        if (cell.hasShip && !areShipsVisible)
        {
            return '.';
        }
         return '.';
    }
    
    private bool IsGameEnded()
        => _player1.ShipCellsDestroyed == Field.ShipsCount || _player2.ShipCellsDestroyed == Field.ShipsCount; 
    
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
        //Thread.Sleep(500);
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