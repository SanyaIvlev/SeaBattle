using System.Reflection;

namespace SeaBattle;

public class Render
{
    private Player _player1;
    private Player _player2;
    
    private char _emptyCell = '.';
    private char _missingShotCell = '\u00a4';
    private char _shipCell = '#';
    private char _destroyedShipCell = 'X';
    private char _playerSelectorCell = '*';
    
    private ConsoleColor _emptyCellColor = ConsoleColor.White;
    private ConsoleColor _missingShotColor = ConsoleColor.Blue;
    private ConsoleColor _shipColor = ConsoleColor.Yellow;
    private ConsoleColor _destroyedShipColor = ConsoleColor.Red;
    private ConsoleColor _playerSelectorColor = ConsoleColor.Green;
    private ConsoleColor _fieldLabel = ConsoleColor.Magenta;

    public Render(Player player1, Player player2)
    {
        _player1 = player1;
        _player2 = player2;
        
        if (_player1.IsHuman)
        {
            _player1.OnCursorPositionChanged += DrawMap;
        }
        else if(_player2.IsHuman)
        {
            _player2.OnCursorPositionChanged += DrawMap;
        }
    }
    
    public void DrawMap()
    {
        Console.Clear();

        DrawFields();
    }

    private void DrawFields()
    {
        Console.ForegroundColor = _fieldLabel;
        Console.Write("Enemy's field \t\tYour field\n\n");

        string horizontalFieldLabel = "";

        for (int i = 0; i < Field.Width; i++)
        {
            char currentLetter = (char)('A' + i); 
            horizontalFieldLabel += currentLetter;
        }
        
        Console.Write("  " + horizontalFieldLabel + "\t\t" + horizontalFieldLabel + "\n");
        
        for (int i = 0; i < Field.Height; i++)
        {
            Console.ForegroundColor = _fieldLabel;
            WriteRowNumber(i + 1);
            
            for (int j = 0; j < Field.Width; j++)
            {
                Console.ForegroundColor = GetCellColor((j,i), _player1, _player2);
                var value = GetCellValue((j,i), _player1, _player2);
                
                Console.Write(value);
            }
            
            Console.Write("\t\t");
            
            for (int j = 0; j < Field.Width; j++)
            {
                Console.ForegroundColor = GetCellColor((j,i), _player2, _player1);
                var value = GetCellValue((j,i), _player2, _player1);
                
                Console.Write(value);
            }
            
            Console.WriteLine();
        }
    }
    
    private void WriteRowNumber(int i)
    {
        if (i < 10)
        {
            Console.Write(" " + i);
        }
        else
        {
            Console.Write(i);
        }
    }

    private ConsoleColor GetCellColor((int x, int y) cellPosition, Player player, Player opponent)
    {
        var areShipsVisible = player.IsHuman;
        
        Field currentField = player.BattleField;
        Cell cell = currentField.GetCell(cellPosition.x, cellPosition.y);
        
        if (!player.IsHuman && opponent.ActionPosition == cellPosition)
        {
            return _playerSelectorColor;
        }
        
        if (cell.hasShot)
        {
            if (cell.hasShip)
            {
                return _destroyedShipColor;
            }

            return _missingShotColor;
        }

        if (cell.hasShip && areShipsVisible)
        {
            return _shipColor;
        }

        if (cell.hasShip && !areShipsVisible)
        {
            return _emptyCellColor;
        }

        return _emptyCellColor;
    }

    private char GetCellValue((int x, int y) cellPosition, Player player, Player opponent)
    {
        var areShipsVisible = player.IsHuman;
        
        Field currentField = player.BattleField;
        Cell cell = currentField.GetCell(cellPosition.x, cellPosition.y);
        
        if (!player.IsHuman && opponent.ActionPosition == cellPosition)
        {
            return _playerSelectorCell;
        }
        if (cell.hasShot)
        {
            if (cell.hasShip)
            {
                return _destroyedShipCell;
            }

            return _missingShotCell;
        }

        if (cell.hasShip && areShipsVisible)
        {
            return _shipCell;
        }

        if (cell.hasShip && !areShipsVisible)
        {
            return _emptyCell;
        }

        return _emptyCell;
    }

}