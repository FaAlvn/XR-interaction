using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private Renderer objectRenderer;
    private bool isHighlighted = false;
    private Color defaultColor = Color.white;
    private Color highlightColor = Color.cyan;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        // URP uses _BaseColor instead of _Color
        objectRenderer.material.SetColor("_BaseColor", defaultColor);
    }

    public void Highlight()
    {
        isHighlighted = !isHighlighted;

        if (isHighlighted)
            objectRenderer.material.SetColor("_BaseColor", highlightColor);
        else
            objectRenderer.material.SetColor("_BaseColor", defaultColor);
    }
}