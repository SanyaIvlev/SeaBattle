using System.Text;

namespace SeaBattle;

public class User
{
    public string Name;
    public int Victories;
    public string ID;

    public User(string name, int victories, string id)
    {
        Name = name;
        Victories = victories;
        ID = id;
    }

}