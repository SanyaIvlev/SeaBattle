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
    private string _savePath;

    private Player _player1;
    private Player _player2;
    
    private Player _currentPlayer;
    private Player _currentOpponent;

    public void Start(Gamemode gamemode, (User profile, bool isHuman) firstPlayer, (User profile, bool isHuman) secondPlayer, string savePath)
    {
        _gamemode = gamemode;
        _savePath = savePath;
        
        InitializePlayers(firstPlayer, secondPlayer);
        
        RegenerateMap();
        
        DrawMap();
        
        RunGameCycle();
    }

    private void InitializePlayers((User profile, bool isHuman) firstPlayer, (User profile, bool isHuman) secondPlayer)
    {
        _player1 = new(firstPlayer.profile, firstPlayer.isHuman);
        _player2 = new(secondPlayer.profile, secondPlayer.isHuman);
    }

    private void RegenerateMap()
    {
        _player1.Reset();
        _player2.Reset();
        
        _currentPlayer = _player1;
        _currentOpponent = _player2;
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

    private void DrawMap()
    {
        Console.Clear();
        DrawFields();
    }
    
    private void DrawFields()
    {
        User profile1 = _player1.Profile;
        User profile2 = _player2.Profile;
        
        PlayerController controller1 = _player1.Controller;
        PlayerController controller2 = _player2.Controller;
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write(profile1.Name + "\t\t\t\t" + profile2.Name + "\n");
        
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
            
            Field currentField = controller1.BattleField;
            
            for (int j = 0; j < Field.Width; j++)
            {
                var cellCharacteristics = GetCellCharacteristics((j,i), currentField, controller2, player1FieldVisibility);

                Console.ForegroundColor = cellCharacteristics.color;
                Console.Write(cellCharacteristics.value + " ");
            }
            
            Console.Write("\t\t");
            
            currentField = controller2.BattleField;
            
            for (int j = 0; j < Field.Width; j++)
            {
                var cellCharacteristics = GetCellCharacteristics((j,i), currentField, controller1, player2FieldVisibility);
                
                Console.ForegroundColor = cellCharacteristics.color;
                Console.Write(cellCharacteristics.value + " ");
            }
            
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.Magenta;
        
        Console.Write(profile1.Name + " destroyed " + controller1.DecksDestroyed + " decks!\n");
        Console.Write(profile2.Name + " destroyed " + controller2.DecksDestroyed + " decks!\n");
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
    
    private (char value, ConsoleColor color) GetCellCharacteristics((int x, int y) cellPosition, Field currentField, PlayerController opponent, bool areShipsVisible)
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
        => _player1.Score == 3 || _player2.Score == 3; 
    
    private void ProcessInput()
    {
        var controller = _currentPlayer.Controller;
        controller.ProcessInput();
    }
    
    private void Logic()
    {
        if (NeedRegenerateFields())
        {
            _currentPlayer.GetVictory();
            RegenerateMap();
            
            return;
        }
        
        var currentController = _currentPlayer.Controller;
        var opponentController = _currentOpponent.Controller;
        
        currentController.Logic(opponentController.BattleField);
        
        if (currentController.IsEndedTurn)
        {
            SwitchPlayer();
        }
    }

    private bool NeedRegenerateFields()
    {
        var currentController = _currentPlayer.Controller;
        var currentField = currentController.BattleField;
        
        return currentController.DecksDestroyed == currentField.DecksCount;
    }
        

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
        Player winner;
        
        if (_currentPlayer.Score > _currentOpponent.Score)
            winner = _currentPlayer;
        else
            winner = _currentOpponent;
        
        PlayerController gameWinnerController = winner.Controller;
        User gameWinnerProfile = winner.Profile;
        
        if(gameWinnerController.IsHuman) 
            UpdateWinnerProfile(gameWinnerProfile);
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        
        Console.Write("\n" + gameWinnerProfile.Name + " has won the game!");
    }

    private void UpdateWinnerProfile(User winner)
    {
        XmlDocument storedUsersInfo = new XmlDocument();
        storedUsersInfo.Load(_savePath);
        
        var userProfiles = storedUsersInfo.DocumentElement;
    
        foreach (XmlNode userProfile in userProfiles.ChildNodes)
        {
            var userIDNode = userProfile.ChildNodes[2];
            
            var userIDText = userIDNode.InnerText;
            
            if (userIDText == winner.ID)
            {
                var victoriesNode = userProfile.ChildNodes[1];
                var victoriesText = victoriesNode.InnerText;

                int updatedVictories = int.Parse(victoriesText) + 1;
                
                userProfile.ChildNodes[1].InnerText = Convert.ToString(updatedVictories);
            }
        }
        storedUsersInfo.Save(_savePath);
    }
}