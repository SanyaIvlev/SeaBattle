using System.Xml;

namespace SeaBattle;

public class Boot
{
    private List<User> _users;
    
    private string _nameNode;
    private string _victoriesNode;

    public Boot()
    {
        _users = new List<User>();
        _nameNode = "name";
        _victoriesNode = "victories";
    }
    
    public void StartApplication()
    {
        InitializeUserProfile();
        DisplayProfiles();
    }

    private void InitializeUserProfile()
    {
        XmlDocument usersSave = new XmlDocument();
        usersSave.Load(@"Users.xml");

        var users = usersSave.DocumentElement;
        
        foreach (XmlNode userProfile in users)
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
            _users.Add(existingUser);
            
            
        }
    }
    
    
    private void DisplayProfiles()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("0 - Create User Profile\n");
        
        Console.ForegroundColor = ConsoleColor.Blue;

        for (int i = 0; i < _users.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {_users[i].Name} / {_users[i].Victories} victories! ");
        }
    }

}