using System.ComponentModel.Design;
using System.Text;
using System.Xml;

namespace SeaBattle;

public class Boot
{
    private const int ID_LENGTH = 16;
    
    private List<User> _existingUsers;

    private User _user1;
    private User _user2;

    private string xmlPath;
    private string xmlName;
    
    private XmlDocument storedUsersInfo;

    private string _userNode;
    private string _nameNode;
    private string _victoriesNode;
    private string _idNode;

    private User _playerProfile;
    
    private Gamemode _gameMode;
    private bool _isPlayer1Human;
    private bool _isPlayer2Human;

    private Random _random;

    public Boot()
    {
        _existingUsers = new List<User>();

        xmlName = "/Users.xml";
        
        _userNode = "user";
        _nameNode = "name";
        _victoriesNode = "victories";
        _idNode = "id";
        
        string currentDirectory = Directory.GetCurrentDirectory();
        var debugFolder = Directory.GetParent(currentDirectory);
        var binFolder = Directory.GetParent(debugFolder.FullName);
        var projectFolder = Directory.GetParent(binFolder.FullName);
        
        string filePath = Path.GetFullPath(projectFolder.FullName + xmlName);
        
        xmlPath = filePath;
        
        XmlDocument usersSave = new XmlDocument();
        usersSave.Load(xmlPath);

        storedUsersInfo = usersSave;
        
        _random = new Random();
    }
    
    public void StartApplication()
    {
        ResetLoadedProfiles();
        
        InitializeUserProfile();

        SetGameMode();

        TrySetProfiles();

        StartGame();
    }

    private void ResetLoadedProfiles()
    {
        _existingUsers.Clear();
    }

    private void SetGameMode()
    {
        Console.Clear();
        
        Console.WriteLine("Select Game Mode!");
        Console.WriteLine("1 - PvP");
        Console.WriteLine("2 - PvE");
        Console.WriteLine("3 - EvE");
        
        var input = Console.ReadKey();
        char key = input.KeyChar;
        
        int selected = key - '0';

        if (selected is < 1 or > 3)
        {
            StartApplication();
            return;
        }
        
        _gameMode = selected switch
        {
            1 => Gamemode.PvP,
            2 => Gamemode.PvE,
            3 => Gamemode.EvE,
        };
    }

    private void TrySetProfiles()
    {
        Console.Clear();
        
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
        string BotID = GetRandomCharSequence();
        
        return new(name, 0, BotID);
    }

    private void InitializeUserProfile()
    {
        var userProfiles = storedUsersInfo.DocumentElement;
        
        foreach (XmlNode userProfile in userProfiles.ChildNodes)
        {
            string userName = "";
            int victories = 0;
            string id = "";
            
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

                if (userInfo.Name == _idNode)
                {
                    id = userInfo.InnerText;
                }
            }
            
            User existingUser = new User(userName, victories, id);
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
        
        if (selected > 0 && selected <= _existingUsers.Count && _existingUsers.Count > 0 )
        {
            return _existingUsers[selected - 1];
        }
        
        StartApplication();
        
        return null;
    }

    private User CreateNewProfile()
    {
        Console.Clear();
        Console.WriteLine("Enter your name: ");
        
        string name = Console.ReadLine();

        string id = GetRandomCharSequence();
        
        StoreProfile(name);

        return new(name, 0, id);
    }

    private void StoreProfile(string name)
    {
        // тут трохи захардкоджено, але я не бачу як це можна винести нормально
        
        XmlElement? root = storedUsersInfo.DocumentElement;
        
        XmlElement userElement = storedUsersInfo.CreateElement(_userNode);
        
        XmlElement nameElement = storedUsersInfo.CreateElement(_nameNode);
        XmlElement victoriesElement = storedUsersInfo.CreateElement(_victoriesNode);

        XmlElement profileIDElement = storedUsersInfo.CreateElement(_idNode);
        
        XmlText nameText = storedUsersInfo.CreateTextNode(name);
        XmlText victoriesText = storedUsersInfo.CreateTextNode("0");
        
        string randomSequence = GetRandomCharSequence();
        XmlText profileIDText = storedUsersInfo.CreateTextNode(randomSequence);
        
        nameElement.AppendChild(nameText);
        victoriesElement.AppendChild(victoriesText);
        profileIDElement.AppendChild(profileIDText);
        
        userElement.AppendChild(nameElement);
        userElement.AppendChild(victoriesElement);
        userElement.AppendChild(profileIDElement);
        
        root?.AppendChild(userElement);
        
        storedUsersInfo.Save("Users.xml");
        
    }

    private string GetRandomCharSequence()
    {
        string randomSequence = "";
        
        for (int i = 0; i < ID_LENGTH; i++)
        {
            char randomSymbol = (char)_random.Next(97, 123);
            randomSequence+= randomSymbol;
        }
        
        return randomSequence;
    }

    private void StartGame()
    {
        Game game = new();
        game.Start(_gameMode, (_user1, _isPlayer1Human), (_user2, _isPlayer2Human), xmlPath);
    }

}