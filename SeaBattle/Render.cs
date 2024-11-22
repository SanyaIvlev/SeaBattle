namespace SeaBattle;

public class Render
{
    private LabeledCell[,] _visiblePlayerMap = new LabeledCell[Field.Height, Field.Width];
    private LabeledCell[,] _visibleEnemyMap = new LabeledCell[Field.Height, Field.Width];
    
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
    
    public void DrawMap(ref readonly Field playerField, ref readonly Field enemyField)
    {
        Console.Clear();
        
        MakeDrawableBotMap(enemyField);
        MakeDrawablePlayerMap(playerField);
        
        DrawFields();
    }

    private void MakeDrawableBotMap(ref readonly Field field)
    {
        for (int i = 0; i < Field.Height; i++)
        {
            for (int j = 0; j < Field.Width; j++)
            {
                Cell currentCell = field.GetCell(j, i);

                LabeledCell drawableCell;
                
                drawableCell.Value = _emptyCell;
                drawableCell.Color = _emptyCellColor;
                
                if (currentCell.hasShot)
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
                
                _visibleEnemyMap[i, j] = drawableCell;
            }
        }
    }

    private void MakeDrawablePlayerMap(ref readonly Field field)
    {
        for (int i = 0; i < Field.Height; i++)
        {
            for (int j = 0; j < Field.Width; j++)
            {
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

                _visiblePlayerMap[i, j] = drawableCell;
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
                LabeledCell currentCell = _visibleEnemyMap[i, j];
                
                Console.ForegroundColor = currentCell.Color;
                Console.Write(currentCell.Value);
                
            }
            
            Console.Write("\t\t");
            
            for (int j = 0; j < Field.Width; j++)
            {
                LabeledCell currentCell = _visiblePlayerMap[i, j];
                
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