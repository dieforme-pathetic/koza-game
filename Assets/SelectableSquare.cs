using UnityEngine;

public class SelectableSquare : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void Select()
    {
        isSelected = true;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.green;
    }

    public void Deselect()
    {
        isSelected = false;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}