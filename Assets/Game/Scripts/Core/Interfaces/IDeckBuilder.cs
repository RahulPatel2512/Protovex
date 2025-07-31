using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core.Interfaces
{
    public struct CardEntry
    {
        public Sprite face;
        public int pairId;
    }

    public interface IDeckBuilder
    {
        List<CardEntry> Build(int totalCards, Sprite[] faces);
    }
}