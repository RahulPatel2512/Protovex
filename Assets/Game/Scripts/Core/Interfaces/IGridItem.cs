namespace Game.Scripts.Core.Interfaces
{
    public interface IGridItem
    {
        void ResetForReuse();
        void OnDespawn();
    }
}