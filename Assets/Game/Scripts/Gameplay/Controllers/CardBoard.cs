using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Core.Events;
using Game.Scripts.Core.Interfaces;
using Game.Scripts.Core.Services;
using Game.Scripts.Grid;
using Game.Scripts.UI;
using Game.Scripts.Utility;
using UnityEngine;

namespace Game.Scripts.Gameplay.Controllers
{
    public class CardBoard : MonoBehaviour
    {
        [Header("Dependencies")] [SerializeField]
        private MonoBehaviour cardFactoryBehaviour;

        [SerializeField] private Sprite[] faces;

        [Header("References")] [SerializeField]
        private GridGeneratorUI grid;
        // Services
        private ICardFactory _factory;
        private IDeckBuilder _deckBuilder;
        private ITimerService _timer;

        // Runtime
        private readonly List<ICardView> _cards = new();
        private readonly List<ICardView> _selection = new(2);
        private bool _inputLocked;

        private void OnEnable()
        {
            CardMatchEvents.OnGameStart += WaitAndBuildBoard;
            CardMatchEvents.OnGameReset += Restart;
        }

        private void OnDisable()
        {
            CardMatchEvents.OnGameStart -= WaitAndBuildBoard;
            CardMatchEvents.OnGameReset -= Restart;
        }


        private void Awake()
        {
            _factory = cardFactoryBehaviour as ICardFactory;
            if (_factory == null)
                Debug.LogError(
                    "CardBoard: 'cardFactoryBehaviour' must implement ICardFactory (e.g., GridCardFactory).");

            _deckBuilder = new DeckBuilder();
            _timer = new SimpleTimerService();
        }

        private void Update()
        {
            _timer.Tick(Time.unscaledDeltaTime);
            CardMatchEvents.TimerUpdated(_timer.Elapsed); 
        }

        private void WaitAndBuildBoard()
        {
            Scheduler.Invoke(BuildBoard, 0.1f);
        }

        private void BuildBoard()
        {
            var total = grid.Cols * grid.Rows;
            if ((total & 1) == 1)
            {
                grid.Cols += 1;
                total = grid.Cols * grid.Rows;
            }

            _cards.Clear();
            _cards.AddRange(_factory.Build(grid.Cols, grid.Rows));

            var deck = _deckBuilder.Build(total, faces);
            for (var i = 0; i < _cards.Count; i++)
            {
                var cv = _cards[i];
                cv.Clicked -= OnCardClicked;
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
                CardMatchEvents.RaiseMatch(a, b);
                UIManager.Audio.PlaySound("Match");

                if (IsAllMatched())
                {
                    _timer.Stop();
                    Scheduler.Invoke(()=>CardMatchEvents.RaiseWin(_timer.Elapsed), 0.2f);
                }
            }
            else
            {
                CardMatchEvents.RaiseMismatch(a, b);
                UIManager.Audio.PlaySound("Mismatch");
                a.Conceal();
                b.Conceal();
            }

            _selection.Clear();
            _inputLocked = false;
        }

        private bool IsAllMatched()
        {
            for (var i = 0; i < _cards.Count; i++)
                if (!_cards[i].IsMatched)
                    return false;
            return true;
        }

        private void Restart()
        {
            BuildBoard();
        }

        public float GetElapsedSeconds()
        {
            return _timer.Elapsed;
        }
    }
}