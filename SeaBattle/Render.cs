using System.Xml.Schema;

namespace SeaBattle;

public class Render
{
    private char[,] _visiblePlayerMap = new char[Field.Height, Field.Width];
    private char[,] _visibleEnemyMap = new char[Field.Height, Field.Width];
    
    private char _emptyCell = '.';
    private char _missingShotCell = '~';
    private char _shipCell = '#';
    private char _destroyedShipCell = 'X';
    private char _playerSelectorCell = '*';


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
                
                char cellSymbol = _emptyCell;
                
                if ((j, i) == playerSelector)
                {
                    cellSymbol = _playerSelectorCell;
                }
                else if (currentCell.hasShot)
                {
                    if (currentCell.hasShip)
                    {
                        cellSymbol = _destroyedShipCell;
                    }
                    else
                    {
                        cellSymbol = _missingShotCell;
                    }
                }
                
                _visibleEnemyMap[i, j] = cellSymbol;
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
                
                char cellSymbol = _emptyCell;
                
                if (currentCell.hasShip)
                {
                    if (currentCell.hasShot)
                    {
                        cellSymbol = _destroyedShipCell;
                    }
                    else
                    {
                        cellSymbol = _shipCell;
                    }
                }
                else
                {
                    if (currentCell.hasShot)
                    {
                        cellSymbol = _missingShotCell;
                    }
                }
                
                _visiblePlayerMap[i, j] = cellSymbol;
            }
        }
    }
    
    
    private void DrawFields()
    {
        for (int i = 0; i < Field.Height; i++)
        {
            for (int j = 0; j < Field.Width; j++)
            {
                Console.Write(_visibleEnemyMap[i,j]);
            }
            
            Console.Write("         ");
            
            for (int j = 0; j < Field.Width; j++)
            {
                Console.Write(_visiblePlayerMap[i,j]);
            }
            
            Console.WriteLine();
        }
    }

}