using PositionStruct;   

namespace SeaBattleClass;

public class SeaBattle
{
    private static int _height = 10;
    private static int _width = 10;
    private char[,] _battleField = new char[_height, _width];
    private char[,] _visibleField = new char[_height, _width];
    
    private List<string> _ships = new List<string>(10) { "####", "###", "###", "##", "##", "##", "#", "#", "#", "#"};
    private int _shipsCellsCount = 20;
    private int _shipsCellsDestructed = 0;
    
    private Position _selector = new Position();
    private Position _delta = new Position();
    
    private char _shipCell = '#';
    private char _destructedShipCell = 'X';  
    private char _emptyCell = '.';
    private char _selectorSign = '*';

    private int triesLeft = 60;

    private const int ENTER = 13;
    
    private Random _random = new Random();

    public void RunGame()
    {
        GenerateMap();
        DrawMap();
        
        while (!IsGameOver())
        {
            ProcessInput();
            Logic();
            DrawMap();
            WriteTries();
        }
    }

    public bool IsWon()
        => _shipsCellsDestructed == _shipsCellsCount;

    private bool IsLost()
        => triesLeft <= 0;


    private void GenerateMap()
    {
        GenerateEmptyField();
        PlaceShips();
        GenerateVisibleField();
    }
    private void PlaceShips()
    {
        foreach (string ship in _ships)
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
                        _battleField[randomCell.y, randomCell.x + i] = _shipCell;
                    }
                    isPlaced = true;
                }
                else if (!isHorizontal && CanPlaceVerticalShip(ship.Length, randomCell))
                {
                    for (int i = 0; i < ship.Length; i++)
                    {
                        _battleField[randomCell.y + i, randomCell.x] = _shipCell;
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
                _battleField[i, j] = _emptyCell;
            }
        }
    }
    private void GenerateVisibleField()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                _visibleField[i, j] = _emptyCell;
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
                
                if ((j, i) == (_selector.x, _selector.y))
                {
                    symbol = _selectorSign; 
                }
                
                Console.Write(symbol);
            }
            
            Console.WriteLine();
        }
    }
    
    private void WriteTries()
    {
        Console.WriteLine("Tries left : " + triesLeft);
    }

    private bool CanPlaceHorizontalShip(int shipLength, Position randomCell)
    {
        for (int i = 0; i < shipLength; i++)
        {
            if (randomCell.x + i >= _width || _battleField[randomCell.y, randomCell.x + i] == _shipCell)
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
            if (randomCell.y + i >= _height || _battleField[randomCell.y + i, randomCell.x] == _shipCell)
            {
                return false;
            }
        }

        return true;
    }

    private void Logic()
    {
        var (newX, newY) = (_selector.x + _delta.x, _selector.y + _delta.y);
        TryToMoveSelector(newX, newY);
    }

    private void ProcessInput()
    {
        (_delta.x, _delta.y) = (0, 0);

        var input = Console.ReadKey().KeyChar;

        (_delta.x, _delta.y) = input switch
        {
            'W' or 'w' => (0, -1),
            'A' or 'a' => (-1, 0),
            'S' or 's' => (0, 1),
            'D' or 'd' => (1, 0),
            _ => (0, 0)
        };

        if (input == (char)ENTER)
        {
            TryToDestructCell();
        }
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
        _selector.x = x;
        _selector.y = y;
    }

    private void TryToDestructCell()
    {
        if (_battleField[_selector.y, _selector.x] == _shipCell)
        {
            _visibleField[_selector.y, _selector.x] = _destructedShipCell;
            _shipsCellsDestructed++;
        }

        triesLeft--;
    }
    
    private bool IsGameOver()
        => IsWon() || IsLost();
    
    private bool GetRandomState()
        => _random.Next(2) == 1;

    private Position GetRandomCell()
    {
        int x = _random.Next(_width);
        int y = _random.Next(_height);
        
        return new Position(x, y);
    }

}