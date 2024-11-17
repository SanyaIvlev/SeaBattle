using CellClass;

namespace FieldClass;

public class Field
{
    public const int Height = 10;
    public const int Width = 10;

    public Cell[,] Cells = new Cell[Height, Width];

    private List<string> _ships = new() { "####", "###", "###", "##", "##", "##", "#", "#", "#", "#" };
    private int _shipsCount = 20;

    private Random _random = new();

    public Field()
    {
        foreach (string ship in _ships)
        {
            bool isHorizontal = GetRandomState();
            bool isPlaced = false;
            
            while (!isPlaced)
            {
                (int x, int y) randomCell = GetRandomCell();

                if (isHorizontal && CanPlaceHorizontal(ship.Length, randomCell))
                {
                    PlaceHorizontal(ship.Length, randomCell);
                    isPlaced = true;
                }
                else if(!isHorizontal && CanPlaceVerticalShip(ship.Length, randomCell))
                {
                    PlaceVertical(ship.Length, randomCell);
                    isPlaced = true;
                }
            }
        }
    }
    
    public int GetShipsNumber()
        => _shipsCount;
    
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
            Cell currentCell = Cells[randomCell.y, randomCell.x + i];
            currentCell.hasShip = true;
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
            Cell currentCell = Cells[randomCell.y + i, randomCell.x];
            currentCell.hasShip = true;
        }

    }

    private bool GetRandomState()
        => _random.Next(2) == 1;
}