using UnityEngine;

public abstract class TransformVector2 : MonoBehaviour
{
    [HideInInspector]
    public TransformVector2Constructor transformVector2;

    public virtual void Awake()
    {
        transformVector2 = new TransformVector2Constructor(transform.position, transform.localPosition, transform.up, -transform.up, transform.right, -transform.right);
    }

    public virtual void Update()
    {
        UpdateTransformVector2();
    }

    void UpdateTransformVector2()
    {
        transformVector2 = new TransformVector2Constructor(transform.position, transform.localPosition, transform.up, -transform.up, transform.right, -transform.right);
    }

    protected void SaveTransformData<T>(ref T transformData) where T : TransformData
    {
        transformData.SetPositionValues(transform.position.x, transform.position.y, transform.position.z);
        transformData.SetRotationValues(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }

    protected void LoadTransformData<T>(ref T transformData) where T : TransformData
    {
        transform.position = new Vector3((float)transformData.position[0], (float)transformData.position[1], (float)transformData.position[2]);
        transform.rotation = new Quaternion((float)transformData.rotation[0], (float)transformData.rotation[1], (float)transformData.rotation[2], (float)transformData.rotation[3]);
    }
}