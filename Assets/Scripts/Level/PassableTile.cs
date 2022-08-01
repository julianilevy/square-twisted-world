using System.Collections;
using UnityEngine;

public class PassableTile : BaseTile, ISavable, ILoadable<PassableTileData>
{
    private PassableTileData _savedData;
    private float _rayLength = 1.8f;

    public override void Awake()
    {
        base.Awake();

        canBeRotated = true;
    }

    public override void AddEmissiveEdges(bool isRecursive = true)
    {
        if (!beingDragged)
            StartCoroutine(AddEmissiveEdgesAtEndOfFrame(isRecursive));
    }

    public override void RemoveEmissiveEdges(bool isRecursive = true)
    {
        if (!beingDragged)
            StartCoroutine(RemoveEmissiveEdgesAtEndOfFrame(isRecursive));
    }

    protected override void EnableAllEmissiveEdges()
    {
        emissionUp.gameObject.SetActive(true);
        emissionRight.gameObject.SetActive(true);
        emissionLeft.gameObject.SetActive(true);
        emissionDown.gameObject.SetActive(true);
    }

    IEnumerator AddEmissiveEdgesAtEndOfFrame(bool isRecursive)
    {
        if (!isRecursive)
            yield return new WaitForEndOfFrame();

        var passableTileUp = GetPassableTile(transform.up);
        var passableTileRight = GetPassableTile(transform.right);
        var passableTileLeft = GetPassableTile(-transform.right);
        var passableTileDown = GetPassableTile(-transform.up);

        if (isRecursive)
        {
            if (passableTileUp != null)
                passableTileUp.AddEmissiveEdges(false);
            if (passableTileRight != null)
                passableTileRight.AddEmissiveEdges(false);
            if (passableTileLeft != null)
                passableTileLeft.AddEmissiveEdges(false);
            if (passableTileDown != null)
                passableTileDown.AddEmissiveEdges(false);
        }
        else
        {
            if (passableTileUp == null)
                emissionUp.gameObject.SetActive(true);
            if (passableTileRight == null)
                emissionRight.gameObject.SetActive(true);
            if (passableTileLeft == null)
                emissionLeft.gameObject.SetActive(true);
            if (passableTileDown == null)
                emissionDown.gameObject.SetActive(true);
        }
    }

    IEnumerator RemoveEmissiveEdgesAtEndOfFrame(bool isRecursive)
    {
        yield return new WaitForEndOfFrame();

        var passableTileUp = GetPassableTile(transform.up);
        var passableTileRight = GetPassableTile(transform.right);
        var passableTileLeft = GetPassableTile(-transform.right);
        var passableTileDown = GetPassableTile(-transform.up);

        if (passableTileUp != null)
        {
            emissionUp.gameObject.SetActive(false);
            if (isRecursive)
                passableTileUp.RemoveEmissiveEdges(false);
        }
        if (passableTileRight != null)
        {
            emissionRight.gameObject.SetActive(false);
            if (isRecursive)
                passableTileRight.RemoveEmissiveEdges(false);
        }
        if (passableTileLeft != null)
        {
            emissionLeft.gameObject.SetActive(false);
            if (isRecursive)
                passableTileLeft.RemoveEmissiveEdges(false);
        }
        if (passableTileDown != null)
        {
            emissionDown.gameObject.SetActive(false);
            if (isRecursive)
                passableTileDown.RemoveEmissiveEdges(false);
        }
    }

    PassableTile GetPassableTile(Vector3 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, _rayLength);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<PassableTile>())
                return hit.collider.gameObject.GetComponent<PassableTile>();
        }

        return null;
    }

    public bool IsRotated()
    {
        if (transform.rotation.eulerAngles == Rotation.EULER_UP || transform.rotation.eulerAngles == Rotation.EULER_DOWN)
            return false;
        else if (transform.rotation.eulerAngles == Rotation.EULER_RIGHT || transform.rotation.eulerAngles == Rotation.EULER_LEFT)
            return true;

        return false;
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new PassableTileData();
            SaveTransformData(ref _savedData);
            SaveBaseTileData(ref _savedData);
        }
    }

    public void LoadData(PassableTileData passableTileData)
    {
        _savedData = passableTileData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                LoadBaseTileData(ref _savedData);
            }
        }
    }

    public TransformData GetSavedData()
    {
        if (_savedData != null)
            return _savedData;

        return null;
    }
}