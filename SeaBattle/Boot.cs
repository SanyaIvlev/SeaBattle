using System.Xml;

namespace SeaBattle;

public class Boot
{
    private List<User> _existingUsers;

    private User _user1;
    private User _user2;

    private XmlDocument storedUsersInfo;

    private string _userNode;
    private string _nameNode;
    private string _victoriesNode;

    private User _playerProfile;
    
    private Gamemode _gameMode;
    private bool _isPlayer1Human;
    private bool _isPlayer2Human;

    public Boot()
    {
        _existingUsers = new List<User>();
        
        _userNode = "user";
        _nameNode = "name";
        _victoriesNode = "victories";
        
        
        XmlDocument usersSave = new XmlDocument();
        usersSave.Load("Users.xml");

        storedUsersInfo = usersSave;
    }
    
    public void StartApplication()
    {
        InitializeUserProfile();

        SetGameMode();

        TrySetProfiles();

        StartGame();
    }

    private void SetGameMode()
    {
        Console.WriteLine("Select Game Mode!");
        Console.WriteLine("1 - PvP");
        Console.WriteLine("2 - PvE");
        Console.WriteLine("3 - EvE");
        
        var input = Console.ReadKey();
        char key = input.KeyChar;
        
        int selected = key - '0';
        
        _gameMode = selected switch
        {
            1 => Gamemode.PvP,
            2 => Gamemode.PvE,
            3 => Gamemode.EvE,
        };
    }

    private void TrySetProfiles()
    {
        User? user1;
        User? user2;

        bool isPlayer1Human;
        bool isPlayer2Human;
        
        if (_gameMode is Gamemode.PvP)
        {
            isPlayer1Human = true;
            isPlayer2Human = true;
            
            Console.WriteLine("First player profile: \n");
            
            DisplayProfiles();
            user1 = SelectProfile();
            
            Console.Clear();
            Console.WriteLine("Second player profile: \n");
            
            DisplayProfiles();
            user2 = SelectProfile();
        }
        else if (_gameMode is Gamemode.PvE)
        {
            isPlayer1Human = true;
            isPlayer2Human = false;
            
            DisplayProfiles();
            user1 = SelectProfile();

            user2 = CreateBotProfile("Bot");
        }
        else
        {
            isPlayer1Human = false;
            isPlayer2Human = false;
            
            user1 = CreateBotProfile("Bot 1");
            user2 = CreateBotProfile("Bot 2");
        }

        if (user1 == null || user2 == null)
        {
            StartApplication();
            return;
        }
        
        (_user1, _user2) = (user1, user2);
        (_isPlayer1Human, _isPlayer2Human) = (isPlayer1Human, isPlayer2Human); 
    }

    private User CreateBotProfile(string name)
    {
        return new(name, 0);
    }

    private void InitializeUserProfile()
    {
        var userProfiles = storedUsersInfo.DocumentElement;
        
        foreach (XmlNode userProfile in userProfiles.ChildNodes)
        {
            string userName = "";
            int victories = 0;
            
            foreach (XmlNode userInfo in userProfile.ChildNodes)
            {
                if (userInfo.Name == _nameNode)
                {
                    userName = userInfo.InnerText;
                }
                
                if (userInfo.Name == _victoriesNode)
                {
                    victories = int.Parse(userInfo.InnerText);
                }
            }
            
            User existingUser = new User(userName, victories);
            _existingUsers.Add(existingUser);
        }
    }
    
    private void DisplayProfiles()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("0 - Create User Profile\n");
        
        Console.ForegroundColor = ConsoleColor.Blue;

        for (int i = 0; i < _existingUsers.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {_existingUsers[i].Name} / {_existingUsers[i].Victories} victories! ");
        }
    }
    
    private User? SelectProfile()
    {
        var input = Console.ReadKey();
        char key = input.KeyChar;
        
        int selected = key - '0';

        if (selected is 0)
        {
            return CreateNewProfile();
        }
        
        if (selected > 0 && _existingUsers.Count > 0 )
        {
            return _existingUsers[selected - 1];
        }

        return null;
    }

    private User CreateNewProfile()
    {
        Console.Clear();
        Console.WriteLine("Enter your name: ");
        
        string name = Console.ReadLine();

        StoreProfile(name);

        return new(name, 0);
    }

    private void StoreProfile(string name)
    {
        XmlElement? root = storedUsersInfo.DocumentElement;
        
        XmlElement userElement = storedUsersInfo.CreateElement(_userNode);
        
        XmlElement nameElement = storedUsersInfo.CreateElement(_nameNode);
        XmlElement victoriesElement = storedUsersInfo.CreateElement(_victoriesNode);
        
        XmlText nameText = storedUsersInfo.CreateTextNode(name);
        XmlText victoriesText = storedUsersInfo.CreateTextNode("0");
        
        nameElement.AppendChild(nameText);
        victoriesElement.AppendChild(victoriesText);
        
        userElement.AppendChild(nameElement);
        userElement.AppendChild(victoriesElement);
        
        root?.AppendChild(userElement);
        
        storedUsersInfo.Save("Users.xml");
        
    }


    private void StartGame()
    {
        Game game = new();
        game.Start(_gameMode, (_user1, _isPlayer1Human), (_user2, _isPlayer2Human));
    }

}