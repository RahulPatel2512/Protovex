using System;
using Game.Scripts.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private Button nextButton;
        private void OnEnable()
        {
            nextButton.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            nextButton.onClick.RemoveAllListeners();
        }
        
        private void OnClicked()
        {
           CardMatchEvents.GameReset();
        }
    }
}
