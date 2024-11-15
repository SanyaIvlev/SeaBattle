using PositionStruct;   

namespace SeaBattleClass;

public class SeaBattle
{
    private static int _height = 10;
    private static int _width = 10;
    private char[,] _battleField = new char[_height, _width];
    private char[,] _visibleField = new char[_height, _width];
    
    List<string> ships = new List<string>(10) { "####", "###", "###", "##", "##", "##", "#", "#", "#", "#"};
    
    private Random random = new Random();
    
    private char shipCell = '#';
    private char emptyCell = '.';

    public void RunGame()
    {
        GenerateMap();
        DrawMap();
    }

    private void GenerateMap()
    {
        GenerateEmptyField();
        PlaceShips();
        GenerateVisibleField();
    }
    private void PlaceShips()
    {
        foreach (string ship in ships)
        {
            bool isHorizontal = GetRandomState();
            bool isPlaced = false;
            
            while (!isPlaced)
            {
                Position randomCell = GetRandomCell();
                if (isHorizontal && CanPlaceHorizontalShip(ship.Length, randomCell))
                {
                    for (int i = 0; i < ship.Length; i++)
                    {
                        _battleField[randomCell.y, randomCell.x + i] = shipCell;
                    }
                    isPlaced = true;
                }
                else if (!isHorizontal && CanPlaceVerticalShip(ship.Length, randomCell))
                {
                    for (int i = 0; i < ship.Length; i++)
                    {
                        _battleField[randomCell.y + i, randomCell.x] = shipCell;
                    }

                    isPlaced = true;
                }
            }
        }
    }

    private void GenerateEmptyField()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                _battleField[i, j] = emptyCell;
            }
        }
    }
    private void GenerateVisibleField()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                // _visibleField[i, j] = emptyCell;
                _visibleField[i, j] = _battleField[i,j];
            }
        }
    }

    private void DrawMap()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                char symbol = _visibleField[i, j];
                Console.Write(symbol);
            }
            Console.WriteLine();
        }
    }

    private bool CanPlaceHorizontalShip(int shipLength, Position randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            if (randomCell.x + i >= _width || _battleField[randomCell.y, randomCell.x + i] == shipCell)
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
            if (randomCell.y + i >= _height || _battleField[randomCell.y + i, randomCell.x] == shipCell)
            {
                return false;
            }
        }

        return true;
    }

    private bool GetRandomState()
        => random.Next(2) == 1 ? true : false;

    private Position GetRandomCell()
    {
        int x = random.Next(_width);
        int y = random.Next(_height);
        
        return new Position(x, y);
    }

}