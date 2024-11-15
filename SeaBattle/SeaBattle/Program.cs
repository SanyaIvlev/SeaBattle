using SeaBattleClass;

static class Program
{
    static void Main(string[] args)
    {
        SeaBattle seaBattle = new SeaBattle();
        
        seaBattle.RunGame();

        if (seaBattle.IsWon())
        {
            Console.WriteLine("You won!");
        }
        else
        {
            Console.WriteLine("You lost!");
        }
    }
}