using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IGridItem
{
    [Header("Refs")] [SerializeField] private Image faceImage;
    [SerializeField] private Image backImage;

    [Header("State")] [SerializeField] private bool isFaceUp;
    [SerializeField] private bool isMatched;

    public void ResetForReuse()
    {
        isFaceUp = false;
        isMatched = false;
        ApplyVisuals();
    }

    public void OnDespawn()
    {
    }

    public void Flip()
    {
        if (isMatched) return;
        isFaceUp = !isFaceUp;
        ApplyVisuals();
    }

    public void SetMatched(bool matched)
    {
        isMatched = matched;
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (faceImage) faceImage.enabled = isFaceUp && !isMatched;
        if (backImage) backImage.enabled = !isFaceUp && !isMatched;
    }
}