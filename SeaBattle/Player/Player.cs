namespace SeaBattle;

public class Player
{
    public PlayerController Controller;
    public User Profile;
    public int Score;

    public Player(User profile, bool isHuman)
    {
        Controller = new(isHuman);
        Profile = profile;
    }

    public void Reset()
    {
        Controller.DecksDestroyed = 0;

        var field = Controller.BattleField;
        field.Generate();
    }
    
    public void GetVictory()
    {
        Score++;
    }
}