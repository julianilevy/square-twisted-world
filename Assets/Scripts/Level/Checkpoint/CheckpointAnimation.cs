using UnityEngine;

public class CheckpointAnimation : MonoBehaviour
{
    [HideInInspector]
    public Checkpoint checkpoint;
    [HideInInspector]
    public SpriteRenderer checkpointEmission;
    [HideInInspector]
    public Material enabledMaterialEmission;
    [HideInInspector]
    public Material disabledMaterialEmission;

    public void OnEnablingAnimationEnded()
    {
        checkpointEmission.material = new Material(enabledMaterialEmission);
        checkpointEmission.material.color = checkpoint.materialColor;
    }

    public void OnDisablingAnimationEnded()
    {
        checkpointEmission.material = new Material(disabledMaterialEmission);
        checkpointEmission.material.color = Color.gray;
    }

    public void OnEndSpawningAnimationEnded()
    {
    }
}