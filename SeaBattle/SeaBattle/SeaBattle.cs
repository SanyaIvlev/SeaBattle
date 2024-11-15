using PositionStruct;   

namespace SeaBattleClass;

public class SeaBattle
{
    private static int _height = 10;
    private static int _width = 10;
    private char[,] _battleField = new char[_height, _width];
    private char[,] _visibleField = new char[_height, _width];
    
    List<string> ships = new List<string>(10) { "####", "###", "###", "##", "##", "##", "#", "#", "#", "#"};
    
    Position selector = new Position();
    private int dx = 0, dy = 0;
    
    private char shipCell = '#';
    private char emptyCell = '.';
    private char selectorSign = '*';
    
    private Random random = new Random();

    public void RunGame()
    {
        GenerateMap();
        DrawMap();
        while (!IsGameOver())
        {
            GetInput();
            Logic();
            DrawMap();
        }
        
        Console.WriteLine("Game Over");
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
                _visibleField[i, j] = emptyCell;
            }
        }
    }

    private void DrawMap()
    {
        Console.Clear();
        
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                char symbol = _visibleField[i, j];
                
                if ((j, i) == (selector.x, selector.y))
                {
                    symbol = selectorSign; 
                }
                
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

    private void Logic()
    {
        var (newX, newY) = (selector.x + dx, selector.y + dy);
        TryToMoveSelector(newX, newY);
    }
    private void GetInput()
    {
        (dx, dy) = (0, 0);

        var input = Console.ReadKey().KeyChar;

        (dx, dy) = input switch
        {
            'W' or 'w' => (0, -1),
            'A' or 'a' => (-1, 0),
            'S' or 's' => (0, 1),
            'D' or 'd' => (1, 0),
            _ => (0, 0)
        };
    }
    
    private void TryToMoveSelector(int x, int y)
    {
        if (CanMoveSelector(x, y))
        {
            MoveSelector(x, y);
        }
    }

    private bool CanMoveSelector(int x, int y)
        => x >= 0 && y >= 0 && x < _width && y < _height;

    private void MoveSelector(int x, int y)
    {
        selector.x = x;
        selector.y = y;
    }

    private bool IsGameOver()
        => false;
    private bool GetRandomState()
        => random.Next(2) == 1 ? true : false;

    private Position GetRandomCell()
    {
        int x = random.Next(_width);
        int y = random.Next(_height);
        
        return new Position(x, y);
    }

}