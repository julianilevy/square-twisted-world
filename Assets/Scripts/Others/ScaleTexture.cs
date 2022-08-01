using UnityEngine;

[ExecuteInEditMode]
public class ScaleTexture : MonoBehaviour
{
    public string textureName = "_MainTex";
    public float baseMultiplier = 50;
    public float scale = 1;

    private float _finalScaleX;
    private float _finalScaleY;

    void Update()
    {
        if (Application.isEditor)
        {
            TransformScale();
            TextureScale();
        }
    }

    void TransformScale()
    {
        SetFinalScaleValues();
        transform.localScale = new Vector3(_finalScaleX, _finalScaleY, 1);
    }

    void TextureScale()
    {
        var material = GetComponent<MeshRenderer>().sharedMaterial;
        material.SetTextureScale(textureName, new Vector2(scale, scale));
    }

    void SetFinalScaleValues()
    {
        var texture = GetComponent<MeshRenderer>().sharedMaterial.GetTexture(textureName);
        float maxValue = Mathf.Max(texture.width, texture.height);
        float minValue = Mathf.Min(texture.width, texture.height);
        var ratio = maxValue * 1 / minValue;

        if (texture.width == maxValue)
            _finalScaleX = scale * baseMultiplier * ratio;
        else if (texture.width == minValue)
            _finalScaleX = scale * baseMultiplier * 1;
        if (texture.height == maxValue)
            _finalScaleY = scale * baseMultiplier * ratio;
        else if (texture.height == minValue)
            _finalScaleY = scale * baseMultiplier * 1;
    }
}