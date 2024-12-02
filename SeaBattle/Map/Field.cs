namespace SeaBattle;
public class Field
{
    public const int Height = 10;
    public const int Width = 10;

    public const int ShipsCount = 20;
    private List<string> _ships = new(10) { "####", "###", "###", "##", "##", "##", "#", "#", "#", "#" };

    private Cell[,] Cells;


    private Random _random = new();

    public ref Cell GetCell(int x, int y) 
        => ref Cells[y, x];
    
    
    public void GenerateField()
    {
        Cells = new Cell[Height, Width];
        
        foreach (string ship in _ships)
        {
            bool isHorizontal = GetRandomState();
            bool isPlaced = false;

            int i = 0;
            
            while (!isPlaced)
            {
                if (i > 50)
                {
                    break;
                }

                TryPlaceShip(isHorizontal, ship, out isPlaced);

                i++;
            }
        }
    }

    private void TryPlaceShip(bool isHorizontal, string ship, out bool isPlaced)
    {
        (int x, int y) randomCell = GetRandomCell();
        
        if (isHorizontal && CanPlaceHorizontal(ship.Length, randomCell))
        {
            PlaceHorizontal(ship.Length, randomCell);
            isPlaced = true;
            return;
        }
        if(!isHorizontal && CanPlaceVerticalShip(ship.Length, randomCell))
        {
            PlaceVertical(ship.Length, randomCell);
            isPlaced = true;
            return;
        }

        isPlaced = false;
    }

    public int GetShipsNumber()
        => ShipsCount;
    
    public (int x, int y) GetRandomCell()
    {
        int x = _random.Next(Width);
        int y = _random.Next(Height);
        
        return (x, y);
    }
    
    private bool CanPlaceHorizontal(int shipLength, (int x, int y)randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            if (randomCell.x + i >= Width || Cells[randomCell.y, randomCell.x + i].hasShip)
            {
                return false;
            }
        }

        return true;
    }
    
    private void PlaceHorizontal(int shipLength, (int x, int y)randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            Cells[randomCell.y, randomCell.x + i].hasShip = true;
        }
    }
    
    private bool CanPlaceVerticalShip(int shipLength, (int x, int y)randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            if (randomCell.y + i >= Height || Cells[randomCell.y + i, randomCell.x].hasShip)
            {
                return false;
            }
        }

        return true;
    }
    
    private void PlaceVertical(int shipLength, (int x, int y)randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            Cells[randomCell.y + i, randomCell.x].hasShip = true;
        }

    }

    private bool GetRandomState()
        => _random.Next(2) == 1;
}