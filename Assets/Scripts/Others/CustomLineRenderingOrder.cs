using UnityEngine;

[ExecuteInEditMode]
public class CustomLineRenderingOrder : MonoBehaviour
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
        var renderer = GetComponent<LineRenderer>();
        renderer.sortingLayerName = renderingLayer;
        renderer.sortingOrder = renderingOrder;
    }
}