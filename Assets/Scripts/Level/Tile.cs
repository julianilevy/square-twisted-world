using UnityEngine;
using System.Collections;

public class Tile : BaseTile, ISavable, ILoadable<TileData>
{
    private TileData _savedData;
    private float _rayLength = 1.8f;

    public override void Awake()
    {
        base.Awake();
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
        if(!isRecursive)
            yield return new WaitForEndOfFrame();

        var tileUp = GetTile(transform.up);
        var tileRight = GetTile(transform.right);
        var tileLeft = GetTile(-transform.right);
        var tileDown = GetTile(-transform.up);

        if (isRecursive)
        {
            if (tileUp != null)
                tileUp.AddEmissiveEdges(false);
            if (tileRight != null)
                tileRight.AddEmissiveEdges(false);
            if (tileLeft != null)
                tileLeft.AddEmissiveEdges(false);
            if (tileDown != null)
                tileDown.AddEmissiveEdges(false);
        }
        else
        {
            if (tileUp == null)
                emissionUp.gameObject.SetActive(true);
            if (tileRight == null)
                emissionRight.gameObject.SetActive(true);
            if (tileLeft == null)
                emissionLeft.gameObject.SetActive(true);
            if (tileDown == null)
                emissionDown.gameObject.SetActive(true);
        }
    }

    IEnumerator RemoveEmissiveEdgesAtEndOfFrame(bool isRecursive)
    {
        yield return new WaitForEndOfFrame();

        var tileUp = GetTile(transform.up);
        var tileRight = GetTile(transform.right);
        var tileLeft = GetTile(-transform.right);
        var tileDown = GetTile(-transform.up);

        if (tileUp != null)
        {
            emissionUp.gameObject.SetActive(false);
            if (isRecursive)
                tileUp.RemoveEmissiveEdges(false);
        }
        if (tileRight != null)
        {
            emissionRight.gameObject.SetActive(false);
            if (isRecursive)
                tileRight.RemoveEmissiveEdges(false);
        }
        if (tileLeft != null)
        {
            emissionLeft.gameObject.SetActive(false);
            if (isRecursive)
                tileLeft.RemoveEmissiveEdges(false);
        }
        if (tileDown != null)
        {
            emissionDown.gameObject.SetActive(false);
            if (isRecursive)
                tileDown.RemoveEmissiveEdges(false);
        }
    }

    Tile GetTile(Vector3 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, _rayLength);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<Tile>())
                return hit.collider.gameObject.GetComponent<Tile>();
        }

        return null;
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new TileData();
            SaveTransformData(ref _savedData);
            SaveBaseTileData(ref _savedData);
        }
    }

    public void LoadData(TileData tileData)
    {
        _savedData = tileData;

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