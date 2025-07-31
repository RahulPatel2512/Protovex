using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Core.Interfaces;
using Game.Scripts.Core.Services;
using Game.Scripts.Grid;
using UnityEngine;

namespace Game.Scripts.Gameplay.Controllers
{
    public class CardBoard : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private MonoBehaviour cardFactoryBehaviour;
        [SerializeField] private Sprite[] faces;
    
        [Header("References")] [SerializeField]
        private GridGeneratorUI grid;
    
        [Header("Timing")]
        [SerializeField] private float mismatchFlipBackDelay = 0.6f;

        // Services
        private ICardFactory _factory;
        private IDeckBuilder _deckBuilder;
        private ITimerService _timer;

        // Runtime
        private readonly List<ICardView> _cards = new();
        private readonly List<ICardView> _selection = new(2);
        private bool _inputLocked;

        // Events
        public event Action<ICardView, ICardView> OnMatch;
        public event Action<ICardView, ICardView> OnMismatch;
        public event Action<float> OnWin;

        private void Awake()
        {
            _factory = cardFactoryBehaviour as ICardFactory;
            if (_factory == null)
                Debug.LogError("CardBoard: 'cardFactoryBehaviour' must implement ICardFactory (e.g., GridCardFactory).");

            _deckBuilder = new DeckBuilder();
            _timer = new SimpleTimerService();
        }
        private void Update()
        {
            _timer.Tick(Time.unscaledDeltaTime);
        }

        public void BuildBoard()
        {
            int total = grid.Cols  * grid.Rows;
            if ((total & 1) == 1) { grid.Cols += 1; total = grid.Cols * grid.Rows; }

            _cards.Clear();
            _cards.AddRange(_factory.Build(grid.Cols, grid.Rows));

            var deck = _deckBuilder.Build(total, faces);
            for (int i = 0; i < _cards.Count; i++)
            {
                var cv = _cards[i];
                cv.Clicked -= OnCardClicked;
                Debug.Log("Build"+deck[i].pairId);
                cv.Setup(deck[i].face, deck[i].pairId);
                cv.Clicked += OnCardClicked;
            }

            _selection.Clear();
            _inputLocked = false;

            _timer.Reset(); 
        }

        private void OnCardClicked(ICardView card)
        {
            if (_inputLocked || card == null) return;

            if (!_timer.IsRunning) _timer.Start();

            card.Reveal();

            if (_selection.Count == 0)
            {
                _selection.Add(card);
                return;
            }

            if (_selection.Count == 1)
            {
                if (_selection[0] == card) return;
                _selection.Add(card);
                _inputLocked = true;
                StartCoroutine(ResolveSelection());
            }
        }

        private IEnumerator ResolveSelection()
        {
            yield return new WaitForSecondsRealtime(0.15f);

            var a = _selection[0];
            var b = _selection[1];

            if (a.PairId == b.PairId)
            {
                a.SetMatched(true);
                b.SetMatched(true);
                OnMatch?.Invoke(a, b);

                if (IsAllMatched())
                {
                    _timer.Stop();
                    OnWin?.Invoke(_timer.Elapsed);
                }
            }
            else
            {
                OnMismatch?.Invoke(a, b);
                yield return new WaitForSecondsRealtime(mismatchFlipBackDelay);
                a.Conceal();
                b.Conceal();
            }

            _selection.Clear();
            _inputLocked = false;
        }

        private bool IsAllMatched()
        {
            for (int i = 0; i < _cards.Count; i++)
                if (!_cards[i].IsMatched) return false;
            return true;
        }

        public void Restart() => BuildBoard();
        
        public float GetElapsedSeconds() => _timer.Elapsed;
    }
}
