using System;
using Game.Scripts.Core.Events;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class TimerUIDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private string format = "mm':'ss'.'ff";

        private void OnEnable()
        {
            CardMatchEvents.OnTimerUpdated += HandleTimerUpdate;
        }

        private void OnDisable()
        {
            CardMatchEvents.OnTimerUpdated -= HandleTimerUpdate;
        }

        private void HandleTimerUpdate(float elapsedSeconds)
        {
            timerText.text = FormatTime(elapsedSeconds);
        }

        private string FormatTime(float seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.ToString(format);
        }
    }
}