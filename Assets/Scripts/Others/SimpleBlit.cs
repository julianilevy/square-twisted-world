using UnityEngine;

[ExecuteInEditMode]
public class SimpleBlit : MonoBehaviour
{
    public Material transitionMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (transitionMaterial != null)
            Graphics.Blit(src, dst, transitionMaterial);
    }
}
