namespace SeaBattle;

public class Render
{
    private Player _player1;
    private Player _player2;
    
    private LabeledCell[,] _visiblePlayer1Map = new LabeledCell[Field.Height, Field.Width];
    private LabeledCell[,] _visiblePlayer2Map = new LabeledCell[Field.Height, Field.Width];
    
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
         
        MakeDrawableBotMap();
        MakeDrawablePlayerMap();
        
        DrawFields();
        
        
    }
    private void MakeDrawableBotMap()
    {
        for (int i = 0; i < Field.Height; i++)
        {
            for (int j = 0; j < Field.Width; j++)
            {
                var field = _player2.BattleField;
                Cell currentCell = field.GetCell(j, i);

                LabeledCell drawableCell;
                
                drawableCell.Value = _emptyCell;
                drawableCell.Color = _emptyCellColor;
                
                if (_player1.ActionPosition == (j, i))
                {
                    drawableCell.Value = _playerSelectorCell;
                    drawableCell.Color = _playerSelectorColor;
                }
                else if (currentCell.hasShot)
                {
                    if (currentCell.hasShip)
                    {
                        drawableCell.Value = _destroyedShipCell;
                        drawableCell.Color = _destroyedShipColor;
                    }
                    else
                    {
                        drawableCell.Value = _missingShotCell;
                        drawableCell.Color = _missingShotColor;
                    }
                }
                
                _visiblePlayer2Map[i, j] = drawableCell;
            }
        }
    }

    private void MakeDrawablePlayerMap()
    {
        for (int i = 0; i < Field.Height; i++)
        {
            for (int j = 0; j < Field.Width; j++)
            {
                var field = _player1.BattleField;
                Cell currentCell = field.GetCell(j, i);
                
                
                LabeledCell drawableCell;
                
                drawableCell.Value = _emptyCell;
                drawableCell.Color = _emptyCellColor;

                if (currentCell.hasShip)
                {
                    if (currentCell.hasShot)
                    {
                        drawableCell.Value = _destroyedShipCell;
                        drawableCell.Color = _destroyedShipColor;
                    }
                    else
                    {
                        drawableCell.Value = _shipCell;
                        drawableCell.Color = _shipColor;
                    }
                }
                else
                {
                    if (currentCell.hasShot)
                    {
                        drawableCell.Value = _missingShotCell;
                        drawableCell.Color = _missingShotColor;
                    }
                }

                _visiblePlayer1Map[i, j] = drawableCell;
            }
        }
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
                LabeledCell currentCell = _visiblePlayer2Map[i, j];
                
                Console.ForegroundColor = currentCell.Color;
                Console.Write(currentCell.Value);
                
            }
            
            Console.Write("\t\t");
            
            for (int j = 0; j < Field.Width; j++)
            {
                LabeledCell currentCell = _visiblePlayer1Map[i, j];
                
                Console.ForegroundColor = currentCell.Color;
                Console.Write(currentCell.Value);
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
}