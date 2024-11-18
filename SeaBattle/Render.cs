using System.Reflection.Emit;
using System.Xml.Schema;

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
    
    public void DrawMap(ref readonly Player player, ref readonly Cell[,] enemyField)
    {
        Console.Clear();
        
        (int x, int y) playerPosition = player.Position;
        
        MakeDrawablePlayerMap(player.BattleField.Cells);
        MakeDrawableEnemyMap(enemyField, playerPosition);
        
        DrawFields();
    }

    private void MakeDrawableEnemyMap(ref readonly Cell[,] field, (int x, int y) playerSelector)
    {
        for (int i = 0; i < Field.Height; i++)
        {
            for (int j = 0; j < Field.Width; j++)
            {
                Cell currentCell = field[i, j];

                LabeledCell drawableCell;
                
                drawableCell.Value = _emptyCell;
                drawableCell.Color = _emptyCellColor;
                
                if ((j, i) == playerSelector)
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
                
                _visibleEnemyMap[i, j] = drawableCell;
            }
        }
    }

    private void MakeDrawablePlayerMap(ref readonly Cell[,] field)
    {
        for (int i = 0; i < Field.Height; i++)
        {
            for (int j = 0; j < Field.Width; j++)
            {
                Cell currentCell = field[i, j];
                
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
        Console.Write("Enemy's field \t\tYour field\n");
        
        for (int i = 0; i < Field.Height; i++)
        {
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
}