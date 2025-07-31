using System;
using UnityEngine;

namespace Game.Scripts.Core.Interfaces
{
    public interface ICardView
    {
        int PairId { get; set; }
        bool IsFaceUp { get; }
        bool IsMatched { get; }

        void Setup(Sprite faceSprite, int pairId);
        void Reveal(float animTime = 0.2f);
        void Conceal(float animTime = 0.2f);
        void SetMatched(bool matched);

        event Action<ICardView> Clicked;
    }
}