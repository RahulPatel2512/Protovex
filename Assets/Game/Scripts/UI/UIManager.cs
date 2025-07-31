using System;
using Game.Scripts.Core.Events;
using Game.Scripts.Gameplay.Controllers;
using Game.Scripts.Grid;
using Game.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GridGeneratorUI grid;

        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject gamePlayScreen;
        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject failScreen;
    

        [SerializeField] private Slider xSlider;
        [SerializeField] private Slider ySlider;
        [SerializeField] private TMP_Text gridValue;
    
        private int _cols = 2;
        private int _rows = 2;
    
        [SerializeField] private Button playButton;
        
        [SerializeField] private AudioManager audioManager;
        public static AudioManager Audio;

        private void Awake()
        {
            if (playButton)
            {
                playButton.SetOnClick(OnClicked);
            }
            Audio = audioManager;
        }

        private void OnEnable()
        {
            CardMatchEvents.OnWin += GameWin;
            CardMatchEvents.OnGameReset += GameReset;
        }
        
        private void OnDisable()
        {
            CardMatchEvents.OnWin -= GameWin;
            CardMatchEvents.OnGameReset -= GameReset;
        }

        private void Start()
        {
            Setup(xSlider, OnXChanged);
            Setup(ySlider, OnYChanged);

            OnXChanged(xSlider.value);
            OnYChanged(ySlider.value);
        }

        private void OnDestroy()
        {
            xSlider.onValueChanged.RemoveListener(OnXChanged);
            ySlider.onValueChanged.RemoveListener(OnYChanged);
        }

        private static void Setup(Slider s, UnityAction<float> cb)
        {
            s.minValue = 2;
            s.maxValue = 10;
            s.wholeNumbers = true;
            s.onValueChanged.AddListener(cb);
        }

        private void OnXChanged(float v)
        {
            _cols = Mathf.RoundToInt(v);
            gridValue.text = $"X: {_cols} Y: {_rows}";
            grid.Cols = _cols;
        }

        private void OnYChanged(float v)
        {
            _rows = Mathf.RoundToInt(v);
            gridValue.text = $"X: {_cols} Y: {_rows}";
            grid.Rows = _rows;
        }
    
        private void OnClicked()
        {
            mainMenuScreen.SetActive(false);
            gamePlayScreen.SetActive(true);
            CardMatchEvents.GameStart();
        }
        
        private void GameWin(float obj)
        {
            gamePlayScreen.SetActive(false);
            winScreen.SetActive(true);
        }

        private void GameReset()
        {
            mainMenuScreen.SetActive(true);
            winScreen.SetActive(false);
            failScreen.SetActive(false);
        }
    }
}

public static class UIManagerExtensions
{
    public static void SetOnClick(this Button button, System.Action action, string sfx = null, float sfxVol = 1f)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            var source = UIManager.Audio.PlaySound(sfx ?? $"Click");
            if (source) source.volume = sfxVol;
            action?.Invoke();
        });
    }
}