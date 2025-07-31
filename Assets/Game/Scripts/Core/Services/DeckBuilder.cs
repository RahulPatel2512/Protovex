using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.Interfaces;
using UnityEngine;

namespace Game.Scripts.Core.Services
{
    public class DeckBuilder : IDeckBuilder
    {
        public List<CardEntry> Build(int totalCards, Sprite[] faces)
        {
            if ((totalCards & 1) == 1) totalCards++;
            int pairCount = totalCards / 2;

            var pairList = PickSpritesForPairs(pairCount, faces);
            var deck = new List<CardEntry>(totalCards);
            foreach (var e in pairList) { deck.Add(e); deck.Add(e); }
            Shuffle(deck);
            return deck;
        }

        private static List<CardEntry> PickSpritesForPairs(int pairCount, Sprite[] faces)
        {
            var res = new List<CardEntry>(pairCount);
            if (faces == null || faces.Length == 0)
            {
                for (int i = 0; i < pairCount; i++)
                    res.Add(new CardEntry { face = null, pairId = i });
                return res;
            }

            var pool = faces.ToList();
            Shuffle(pool);

            int idx = 0;
            for (int i = 0; i < pairCount; i++)
            {
                res.Add(new CardEntry { face = pool[idx], pairId = i });
                idx = (idx + 1) % pool.Count;
            }
            return res;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}