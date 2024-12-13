using System.Xml;

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
    
    private int _player1Score;
    private int _player2Score;

    private Player _currentPlayer;
    private Player _currentOpponent;

    private Player _lastRoundWinner;

    public void Start(Gamemode gamemode, (User profile, bool isHuman) firstPlayer, (User profile, bool isHuman) secondPlayer)
    {
        _gamemode = gamemode;
        
        InitializePlayers(firstPlayer, secondPlayer);
        
        RegenerateMap();
        
        DrawMap();
        
        RunGameCycle();
    }

    private void InitializePlayers((User profile, bool isHuman) firstPlayer, (User profile, bool isHuman) secondPlayer)
    {
        _player1 = new(firstPlayer.isHuman, firstPlayer.profile);
        _player2 = new(secondPlayer.isHuman, secondPlayer.profile);
    }

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

        EndGame();
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
        => _player1Score == 3 || _player2Score == 3; 
    
    private void ProcessInput()
    {
        _currentPlayer.ProcessInput();
    }
    
    private void Logic()
    {
        if (NeedRegenerateFields())
        {
            _lastRoundWinner = _currentPlayer;
            RegenerateMap();

            if (_lastRoundWinner == _player1)
                _player1Score++;
            else
                _player2Score++;
            
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
    
    private void EndGame()
    {
        if(_lastRoundWinner.IsHuman) 
            UpdateWinnerProfile();
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        
        Console.Write("\n" + _lastRoundWinner.Name + " has won the game!");
    }

    private void UpdateWinnerProfile()
    {
        XmlDocument storedUsersInfo = new XmlDocument();
        storedUsersInfo.Load("Users.xml");
        
        var userProfiles = storedUsersInfo.DocumentElement;
    
        foreach (XmlNode userProfile in userProfiles.ChildNodes)
        {
            var userNameNode = userProfile.ChildNodes[0];
            
            var userNameText = userNameNode.InnerText;
            
            if (userNameText == _lastRoundWinner.Name)
            {
                var victoriesNode = userProfile.ChildNodes[1];
                var victoriesText = victoriesNode.InnerText;

                int updatedVictories = int.Parse(victoriesText) + 1;
                
                userProfile.ChildNodes[1].InnerText = Convert.ToString(updatedVictories);
            }
        }
        storedUsersInfo.Save("Users.xml");
    }
}