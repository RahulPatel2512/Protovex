using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridGeneratorUI : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private RectTransform gridContainer;

    [SerializeField] private GameObject cardPrefab;

    [Header("Layout")] [SerializeField] private Vector2 spacing = new(10, 10);

    [Header("Pooling")] [SerializeField] [Tooltip("Create this many pooled items on Awake")]
    private int prewarm = 20;

    [Header("Behaviour")] [SerializeField] private bool rebuildOnRectChange = true;

    private GridLayoutGroup _grid;
    private readonly List<GameObject> _active = new();

    private int _rows = 2;
    private int _cols = 2;

    public int Rows
    {
        get => _rows;
        set
        {
            value = Mathf.Clamp(value, 2, 10);
            if (_rows != value)
            {
                _rows = value;
                Rebuild();
            }
        }
    }

    public int Cols
    {
        get => _cols;
        set
        {
            value = Mathf.Clamp(value, 2, 10);
            if (_cols != value)
            {
                _cols = value;
                Rebuild();
            }
        }
    }

    private void Start()
    {
        _grid = GetComponent<GridLayoutGroup>();
        if (gridContainer == null) gridContainer = (RectTransform)transform;

        _grid.spacing = spacing;
        // _grid.padding = padding;

        if (cardPrefab != null && prewarm > 0)
            cardPrefab.Pool(prewarm);

        Rebuild();
    }

    private void OnValidate()
    {
        if (!_grid) _grid = GetComponent<GridLayoutGroup>();
        if (_grid) _grid.spacing = spacing;
        // _grid.padding = padding;
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!rebuildOnRectChange || !isActiveAndEnabled) return;
        RefreshCellSizeOnly();
    }

    private void Rebuild()
    {
        for (var i = _active.Count - 1; i >= 0; i--)
        {
            var go = _active[i];
            if (go)
            {
                if (go.TryGetComponent<IGridItem>(out var item))
                    item.OnDespawn();

                go.Despawn();
            }
        }

        _active.Clear();

        var count = _rows * _cols;
        for (var i = 0; i < count; i++)
        {
            var go = cardPrefab.Spawn(gridContainer);
            go.name = $"Card_{i}";
            if (go.transform is RectTransform rt)
                rt.localScale = Vector3.one;

            if (go.TryGetComponent<IGridItem>(out var item))
                item.ResetForReuse();

            _active.Add(go);
        }

        RefreshCellSizeOnly();
    }

    private void RefreshCellSizeOnly()
    {
        if (!_grid) return;

        var cell = GridCalculator.ComputeSquareCellSize(
            gridContainer.rect, _rows, _cols, _grid.spacing, _grid.padding);

        _grid.cellSize = cell;
        _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _grid.constraintCount = _cols;
    }
    
    public Vector2Int GetSize()
    {
        return new Vector2Int(_cols, _rows);
    }

    public void SetSize(int cols, int rows)
    {
        _cols = Mathf.Clamp(cols, 2, 10);
        _rows = Mathf.Clamp(rows, 2, 10);
        Rebuild();
    }

    // public Vector2 GetSpacing() => _grid != null ? _grid.spacing : spacing;
    // public RectOffset GetPadding() => _grid != null ? _grid.padding : padding;

    public void SetSpacing(Vector2 s)
    {
        spacing = s;
        if (_grid) _grid.spacing = s;
        RefreshCellSizeOnly();
    }

    public void SetPadding(RectOffset p)
    {
        // padding = p;
        if (_grid) _grid.padding = p;
        RefreshCellSizeOnly();
    }
}