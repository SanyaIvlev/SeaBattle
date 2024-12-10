namespace SeaBattle;

public interface IAction
{
    (int x, int y) GetPosition();
    void Process();
}