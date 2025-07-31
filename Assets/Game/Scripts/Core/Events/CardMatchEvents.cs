using System;
using Game.Scripts.Core.Interfaces;

namespace Game.Scripts.Core.Events
{
    public static class CardMatchEvents
    {
        public static event Action<ICardView, ICardView> OnMatch;
        public static event Action<ICardView, ICardView> OnMismatch;
        public static event Action<float> OnWin;
        public static event Action OnGameStart;
        public static event Action OnGameReset;
        
        public static event System.Action<float> OnTimerUpdated;


        public static void RaiseMatch(ICardView a, ICardView b) => OnMatch?.Invoke(a, b);
        public static void RaiseMismatch(ICardView a, ICardView b) => OnMismatch?.Invoke(a, b);
        public static void RaiseWin(float time) => OnWin?.Invoke(time);
        public static void GameStart()=> OnGameStart?.Invoke();
        public static void GameReset()=> OnGameReset?.Invoke();

        public static void TimerUpdated(float obj)=> OnTimerUpdated?.Invoke(obj);
        
    }
}