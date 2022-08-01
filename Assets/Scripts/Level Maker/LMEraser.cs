using UnityEngine;

public class LMEraser : MonoBehaviour
{
    [HideInInspector]
    public BasePrefab eraser;

    private LMSettings _lmSettings;

    void Awake()
    {
        _lmSettings = GetComponent<LMSettings>();
        _lmSettings.AddToOnGUIFunctions(MoveEraser);
        _lmSettings.AddToOnGUIFunctions(DeletePrefab);
        _lmSettings.AddToUpdateFunctions(DeleteEraser);
    }

    void MoveEraser()
    {
        if (eraser != null)
        {
            var mousePosition = _lmSettings.lmCamera.ScreenToWorldPoint(Input.mousePosition);
            eraser.transform.position = new Vector2(mousePosition.x, mousePosition.y);
        }
    }

    void DeleteEraser()
    {
        if (Input.GetMouseButtonDown(1))
            _lmSettings.Delete(ref eraser);
    }

    void DeletePrefab()
    {
        if (Input.GetMouseButton(0))
        {
            if (!_lmSettings.IsCollidingWithUI())
            {
                if (eraser != null)
                {
                    var hits = eraser.GetColliderHits(_lmSettings.gridCollisionMask);

                    foreach (var hit in hits)
                    {
                        if (hit)
                        {
                            var firstGridCellHit = hit.GetComponent<GridCell>().GetGridCellHit(_lmSettings.prefabsCollisionMask);

                            if (firstGridCellHit)
                            {
                                var basePrefabModifiable = firstGridCellHit.GetComponentInParent<LMBasePrefabModifable>();
                                var basePrefabChildren = basePrefabModifiable.GetComponentsInChildren<BasePrefab>();

                                if (basePrefabChildren.Length > 0)
                                {
                                    foreach (var prefabChild in basePrefabChildren)
                                    {
                                        foreach (var collider in prefabChild.GetColliderHits(_lmSettings.gridCollisionMask))
                                        {
                                            var gridCellHit = collider.GetComponent<GridCell>();
                                            gridCellHit.tilesFilled[LevelMakerManager.instance.currentSubLevel] = false;
                                            gridCellHit.prefabsFilled[LevelMakerManager.instance.currentSubLevel] = false;
                                        }
                                    }
                                }

                                var basePrefab = basePrefabModifiable.GetComponent<BasePrefab>();
                                if (basePrefab.spawnAmountLimit > 0)
                                    _lmSettings.lmBasePrefabLimiter.RemoveFromDictionary(basePrefab);
                                if (basePrefab.GetComponent<BaseTile>())
                                    basePrefab.GetComponent<BaseTile>().AddEmissiveEdges();

                                Destroy(basePrefab.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetEraserOnMouse(GameObject prefab) // Llamado desde el botón...
    {
        _lmSettings.DeselectAndDeleteAll();
        var newEraser = (Instantiate(prefab.gameObject, Input.mousePosition, prefab.transform.rotation));
        newEraser.name = prefab.name;
        eraser = newEraser.GetComponent<BasePrefab>();
    }
}