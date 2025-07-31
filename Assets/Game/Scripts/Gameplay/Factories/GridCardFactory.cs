using System.Linq;
using Game.Scripts.Core.Interfaces;
using Game.Scripts.Gameplay.Views;
using Game.Scripts.Grid;
using UnityEngine;

namespace Game.Scripts.Gameplay.Factories
{
    public class GridCardFactory : MonoBehaviour, ICardFactory
    {
        [SerializeField] private GridGeneratorUI grid;

        public ICardView[] Build(int cols, int rows)
        {
            grid.SetSize(cols, rows);
            
            return grid.GetComponentsInChildren<CardView>(true)
                .Cast<ICardView>()
                .ToArray();
        }
    }
}