using UnityEngine;

[ExecuteInEditMode]
public class CustomRenderingOrder : MonoBehaviour
{
    public string renderingLayer = "";
    public int renderingOrder;

    void Update()
    {
        if (Application.isEditor)
            SetRenderingOrder();
    }

    void SetRenderingOrder()
    {
        var renderer = GetComponent<Renderer>();
        renderer.sortingLayerName = renderingLayer;
        renderer.sortingOrder = renderingOrder;
    }
}