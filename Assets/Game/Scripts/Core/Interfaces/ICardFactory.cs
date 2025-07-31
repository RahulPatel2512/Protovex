namespace Game.Scripts.Core.Interfaces
{
    public interface ICardFactory
    {
        ICardView[] Build(int cols, int rows);
    }
}