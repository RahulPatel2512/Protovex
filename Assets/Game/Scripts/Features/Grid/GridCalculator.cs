using UnityEngine;

public static class GridCalculator
{
    public static Vector2 ComputeSquareCellSize(
        Rect containerRect,
        int rows, int cols,
        Vector2 spacing,
        RectOffset padding)
    {
        var availW = containerRect.width - padding.left - padding.right - spacing.x * (cols - 1);
        var availH = containerRect.height - padding.top - padding.bottom - spacing.y * (rows - 1);

        var cw = availW / Mathf.Max(cols, 1);
        var ch = availH / Mathf.Max(rows, 1);

        var side = Mathf.Floor(Mathf.Max(0, Mathf.Min(cw, ch)));
        return new Vector2(side, side);
    }
}