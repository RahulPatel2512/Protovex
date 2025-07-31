using System;
using System.Collections;
using Game.Scripts.Core.Interfaces;
using Game.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Gameplay.Views
{
    public class CardView : MonoBehaviour, IGridItem,ICardView
    {
        [Header("Refs")] [SerializeField] private Image faceImage;
        [SerializeField] private Image backImage;
        [SerializeField] private Button cardButton;

        [Header("State")] [SerializeField] private bool isFaceUp;
        [SerializeField] private bool isMatched;
    
        public event Action<ICardView> Clicked;
        
        [Header("Timing")] [SerializeField] private float mismatchFlipBackDelay = 0.3f;

        public int PairId { get; set; } = -1;
        public bool IsFaceUp => isFaceUp;
        public bool IsMatched => isMatched;
    
        private void Awake()
        {
            if (cardButton)
            {
                cardButton.SetOnClick(OnClicked);
            }
            ApplyVisuals();
        }
    
        private void OnClicked()
        {
            if (isMatched || isFaceUp) return;
            Reveal();
            Clicked?.Invoke(this);
        }
    
        public void Reveal(float animTime = 0.1f)
        {
            if (isMatched || isFaceUp) return;
            isFaceUp = true;
            StartCoroutine(Flip90(animTime, ApplyVisuals, null));
        }

        public void Conceal(float animTime = 0.1f)
        {
            if (isMatched || !isFaceUp) return;
            Scheduler.Invoke(() =>
            {
                isFaceUp = false;
                StartCoroutine(Flip90(animTime, ApplyVisuals, null));
            }, mismatchFlipBackDelay);
        }

        public void SetMatched(bool matched)
        {
            isMatched = matched;
            Scheduler.Invoke(ApplyVisuals,matched?0.2f:0);
            if (cardButton) cardButton.interactable = !matched;
        }

        private void ApplyVisuals()
        {
            if (faceImage) faceImage.enabled = isFaceUp && !isMatched;
            if (backImage) backImage.enabled = !isFaceUp && !isMatched;
            if (cardButton) cardButton.interactable = !isMatched;
        }
    
        public void Setup(Sprite faceSprite, int pairId)
        {
            PairId = pairId;
            if (faceImage) faceImage.sprite = faceSprite;
            isFaceUp = false;
            isMatched = false;
            ApplyVisuals();
        }

        public void ResetForReuse()
        {
            isFaceUp = false;
            isMatched = false;
            PairId = -1;
            ApplyVisuals();
        }

        public void OnDespawn() { }
    
        private IEnumerator Flip90(float timePerHalf, Action onHalfTurn, Action onComplete)
        {
            yield return Rotate90(timePerHalf);
            onHalfTurn?.Invoke();
            yield return Rotate90(timePerHalf);
            onComplete?.Invoke();
        }
    
        private IEnumerator Rotate90(float time)
        {
            var t = transform;
            Quaternion start = t.localRotation;
            Quaternion end   = t.localRotation * Quaternion.Euler(0f, 90f, 0f);

            float dur = Mathf.Max(time, 0.0001f);
            float u = 0f;
            while (u < 1f)
            {
                u += Time.unscaledDeltaTime / dur;
                t.localRotation = Quaternion.Slerp(start, end, u);
                yield return null;
            }
        }
    }
}