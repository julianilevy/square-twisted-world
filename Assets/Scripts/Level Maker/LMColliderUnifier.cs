using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LMColliderUnifier : MonoBehaviour
{
    private LMSettings _lmSettings;
    private BoxCollider2D _currentCollider;
    private float _maxDistanceBetweenTiles = 3.3f;
    private float _currentRowOrColumn;
    private bool _colliderCanGrow;
    private bool _isPassableTile;

    void Awake()
    {
        _lmSettings = GetComponent<LMSettings>();
    }

    public void UnifyColliders() // Llamado desde Saver...
    {
        StartCoroutine(UnifyAndSaveColliders());
    }

    public void DestroyCollidersContainer() // Llamado desde Saver...
    {
        for (int i = 0; i < LevelMakerManager.instance.subLevelsAmount; i++)
        {
            var collidersList = LevelMakerManager.instance.lmContainers.containers[i][typeof(UnifiedCollider)];

            for (int j = 0; j < collidersList.transform.childCount; j++)
                Destroy(collidersList.transform.GetChild(j).gameObject);
        }
    }

    IEnumerator UnifyAndSaveColliders()
    {
        for (int i = 0; i < LevelMakerManager.instance.subLevelsAmount; i++)
        {
            LevelMakerManager.instance.SetActiveSubLevel(i);

            UnifyHorizontal<Tile>(i);
            UnifyVertical<Tile>(i);
            UnifyHorizontal<PassableTile>(i);
            UnifyVertical<PassableTile>(i);
        }

        yield return new WaitForEndOfFrame();
        _lmSettings.lmSaver.ContinuePreSavingProcess();
    }

    void UnifyHorizontal<T>(int index) where T : BaseTile
    {
        var container = LevelMakerManager.instance.lmContainers.containers[index][typeof(T)];
        if (container.transform.childCount <= 0)
            return;

        var list = new List<T>();
        foreach (Transform containerChild in container.transform)
            list.Add(containerChild.GetComponent<T>());

        var orderedList = list.OrderBy(t => t.transform.position.x).ToList().OrderByDescending(t => t.transform.position.y).ToList();

        _currentRowOrColumn = 0f;
        _colliderCanGrow = false;
        _isPassableTile = false;

        if (orderedList.Count > 0)
        {
            if (orderedList[0] is PassableTile)
                _isPassableTile = true;
        }

        for (int i = 0; i < orderedList.Count; i++)
        {
            if (_isPassableTile)
            {
                if (orderedList[i].GetComponent<PassableTile>().IsRotated())
                    continue;
            }

            var previousRowOrColumn = _currentRowOrColumn;
            if (i == 0)
                previousRowOrColumn = orderedList[i].transform.position.y;
            _currentRowOrColumn = orderedList[i].transform.position.y;

            if (previousRowOrColumn != _currentRowOrColumn)
                _colliderCanGrow = false;

            if (!orderedList[i].destroyable)
            {
                if (!_colliderCanGrow)
                {
                    var newEmptyObject = new GameObject(typeof(T).ToString() + " Horizontal Collider: X[" + orderedList[i].transform.position.x.ToString("0.##") + "]" + " - Y[" + orderedList[i].transform.position.y.ToString("0.##") + "]");
                    newEmptyObject.transform.position = orderedList[i].transform.position;
                    newEmptyObject.layer = K.LAYER_OBSTACLE;
                    if (_isPassableTile)
                    {
                        newEmptyObject.tag = K.TAG_PASSABLE;
                        newEmptyObject.transform.eulerAngles = Rotation.EULER_DOWN;
                    }
                    _currentCollider = newEmptyObject.AddComponent<BoxCollider2D>();
                    _currentCollider.size = orderedList[i].GetComponent<BoxCollider2D>().size;
                    var unifiedCollider = newEmptyObject.AddComponent<UnifiedCollider>();
                    LevelMakerManager.instance.lmContainers.AddToContainer(unifiedCollider.GetType(), unifiedCollider.gameObject);

                    if ((i + 1) < orderedList.Count)
                    {
                        if (GetDistance(orderedList[i].transform.position.x, orderedList[i + 1].transform.position.x) <= _maxDistanceBetweenTiles)
                            _colliderCanGrow = true;
                        else
                        {
                            Destroy(newEmptyObject);
                            continue;
                        }

                        if (GetDistance(orderedList[i].transform.position.y, orderedList[i + 1].transform.position.y) > 0)
                        {
                            Destroy(newEmptyObject);
                            continue;
                        }
                    }
                    else
                    {
                        Destroy(newEmptyObject);
                        continue;
                    }

                    orderedList[i].alone = false;
                }
                else
                {
                    if ((i + 1) < orderedList.Count)
                    {
                        if (GetDistance(orderedList[i].transform.position.x, orderedList[i + 1].transform.position.x) >= _maxDistanceBetweenTiles)
                            _colliderCanGrow = false;

                        if (_isPassableTile)
                        {
                            if (orderedList[i + 1].GetComponent<PassableTile>().IsRotated())
                                _colliderCanGrow = false;
                        }
                    }

                    _currentCollider.offset += new Vector2(orderedList[i].GetComponent<BoxCollider2D>().size.x / 2, 0f);
                    _currentCollider.size += new Vector2(orderedList[i].GetComponent<BoxCollider2D>().size.x, 0f);

                    orderedList[i].alone = false;
                }
            }
        }
    }

    void UnifyVertical<T>(int index) where T : BaseTile
    {
        var container = LevelMakerManager.instance.lmContainers.containers[index][typeof(T)];
        if (container.transform.childCount <= 0)
            return;

        var list = new List<T>();
        foreach (Transform containerChild in container.transform)
            list.Add(containerChild.GetComponent<T>());

        var orderedList = list.OrderBy(t => t.transform.position.y).ToList().OrderByDescending(t => t.transform.position.x).ToList();

        _currentRowOrColumn = 0f;
        _colliderCanGrow = false;
        _isPassableTile = false;

        if (orderedList.Count > 0)
        {
            if (orderedList[0].GetComponent<PassableTile>())
                _isPassableTile = true;
        }

        for (int i = 0; i < orderedList.Count; i++)
        {
            if (_isPassableTile)
            {
                if (!orderedList[i].GetComponent<PassableTile>().IsRotated())
                    continue;
                else
                    orderedList[i].alone = false;
                // Quitar ese else cuanto antes. No es la forma correcta de arreglarlo...
            }

            var previousRowOrColumn = _currentRowOrColumn;
            if (i == 0)
                previousRowOrColumn = orderedList[i].transform.position.x;
            _currentRowOrColumn = orderedList[i].transform.position.x;

            if (previousRowOrColumn != _currentRowOrColumn)
                _colliderCanGrow = false;

            if (!orderedList[i].destroyable)
            {
                if (!_colliderCanGrow)
                {
                    var newEmptyObject = new GameObject(typeof(T).ToString() + " Vertical Collider: X[" + orderedList[i].transform.position.x.ToString("0.##") + "]" + " - Y[" + orderedList[i].transform.position.y.ToString("0.##") + "]");
                    newEmptyObject.transform.position = orderedList[i].transform.position;
                    newEmptyObject.layer = K.LAYER_OBSTACLE;
                    if (_isPassableTile)
                    {
                        newEmptyObject.tag = K.TAG_PASSABLE;
                        newEmptyObject.transform.eulerAngles = Rotation.EULER_RIGHT;
                    }
                    _currentCollider = newEmptyObject.AddComponent<BoxCollider2D>();
                    _currentCollider.size = orderedList[i].GetComponent<BoxCollider2D>().size;
                    var unifiedCollider = newEmptyObject.AddComponent<UnifiedCollider>();
                    LevelMakerManager.instance.lmContainers.AddToContainer(unifiedCollider.GetType(), unifiedCollider.gameObject);

                    if ((i + 1) < orderedList.Count)
                    {
                        if (GetDistance(orderedList[i].transform.position.y, orderedList[i + 1].transform.position.y) <= _maxDistanceBetweenTiles)
                            _colliderCanGrow = true;
                        else
                        {
                            Destroy(newEmptyObject);
                            continue;
                        }

                        if (GetDistance(orderedList[i].transform.position.x, orderedList[i + 1].transform.position.x) > 0)
                        {
                            Destroy(newEmptyObject);
                            continue;
                        }
                    }
                    else
                    {
                        Destroy(newEmptyObject);
                        continue;
                    }

                    orderedList[i].alone = false;
                }
                else
                {
                    if ((i + 1) < orderedList.Count)
                    {
                        if (GetDistance(orderedList[i].transform.position.y, orderedList[i + 1].transform.position.y) >= _maxDistanceBetweenTiles)
                            _colliderCanGrow = false;

                        if (_isPassableTile)
                        {
                            if (!orderedList[i + 1].GetComponent<PassableTile>().IsRotated())
                                _colliderCanGrow = false;
                        }
                    }

                    if (_isPassableTile)
                    {
                        _currentCollider.offset += new Vector2(orderedList[i].GetComponent<BoxCollider2D>().size.x / 2, 0f);
                        _currentCollider.size += new Vector2(orderedList[i].GetComponent<BoxCollider2D>().size.x, 0f);
                        continue;
                    }

                    _currentCollider.offset += new Vector2(0f, orderedList[i].GetComponent<BoxCollider2D>().size.y / 2);
                    _currentCollider.size += new Vector2(0f, orderedList[i].GetComponent<BoxCollider2D>().size.y);

                    orderedList[i].alone = false;
                }
            }
        }
    }

    float GetDistance(float pointA, float pointB)
    {
        var difference = pointB - pointA;
        var distance = Mathf.Abs(difference);

        return distance;
    }
}