using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderBinder : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private GridGeneratorUI grid;

    [SerializeField] private Slider xSlider;
    [SerializeField] private Slider ySlider;
    [SerializeField] private TMP_Text gridValue;
    
    private int _cols = 2;
    private int _rows = 2;

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
}