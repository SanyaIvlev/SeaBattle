using CellClass;
using PositionStruct;

namespace FieldClass;

public class Field
{
    private const int _height = 10;
    private const int _width = 10;

    private Cell[,] _cells = new Cell[_height, _width];

    private List<string> _ships = new() { "####", "###", "###", "##", "##", "##", "#", "#", "#", "#" };

    private Random _random = new();

    public Field()
    {
        foreach (string ship in _ships)
        {
            bool isHorizontal = GetRandomState();
            bool isPlaced = false;
            
            while (!isPlaced)
            { 
                Position randomCell = GetRandomCell();

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

    private void PlaceHorizontal(int shipLength, Position randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            Cell currentCell = _cells[randomCell.y, randomCell.x + i];
            currentCell.hasShip = true;
        }
    }

    private void PlaceVertical(int shipLength, Position randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            Cell currentCell = _cells[randomCell.y + i, randomCell.x];
            currentCell.hasShip = true;
        }

    }
    private bool CanPlaceHorizontal(int shipLength, Position randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            if (randomCell.x + i >= _width || _cells[randomCell.y, randomCell.x + i].hasShip)
            {
                return false;
            }
        }

        return true;
    }
    
    private bool CanPlaceVerticalShip(int shipLength, Position randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            if (randomCell.y + i >= _height || _cells[randomCell.y + i, randomCell.x].hasShip)
            {
                return false;
            }
        }

        return true;
    }


    private bool GetRandomState()
        => _random.Next(2) == 1;

    private Position GetRandomCell()
    {
        int x = _random.Next(_width);
        int y = _random.Next(_height);
        
        return new Position(x, y);
    }
}