using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GridMaker : MonoBehaviour
{
    public GameObject grid;
    public GridCell gridCell;
    public float xInitialPoint = -1920f;
    public float yInitialPoint = 1920f;
    public int width = 600;
    public int height = 600;
    public bool createGrid;
    public bool destroyGrid;

    private float _cellOffset = 3.2f;
    private float _horizontalCellOffset = 0f;
    private float _verticalCellOffset = 0f;
    private int _horizontalGridPosition = 0;
    private int _verticalGridPosition = 0;
    private int _coroutineLimit = 0;

    void Update()
    {
        if (createGrid)
        {
            CreateContainer();
            StartCoroutine(CreateGrid());
            createGrid = false;
        }
        if (destroyGrid)
        {
            DestroyGrid();
            destroyGrid = false;
        }
    }

    void CreateContainer()
    {
        DestroyGrid();
        grid = new GameObject();
        grid.transform.position = Vector3.zero;
        grid.name = "Grid";
    }

    void DestroyGrid()
    {
        if (grid != null)
        {
            _horizontalCellOffset = 0f;
            _verticalCellOffset = 0f;
            _horizontalGridPosition = 0;
            _verticalGridPosition = 0;
            _coroutineLimit = 0;
            DestroyImmediate(grid);
            grid = null;
        }
    }

    IEnumerator CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            _verticalCellOffset = 0f;
            _verticalGridPosition = 0;
            _horizontalGridPosition++;
            CreateCell(new Vector2(xInitialPoint + _horizontalCellOffset, yInitialPoint));
            _horizontalCellOffset += _cellOffset;

            for (int y = 0; y < height - 1; y++)
            {
                _verticalGridPosition++;
                _verticalCellOffset += _cellOffset;
                CreateCell(new Vector2(xInitialPoint + (_horizontalCellOffset - _cellOffset), yInitialPoint - _verticalCellOffset));

                if (_coroutineLimit >= 100000)
                {
                    _coroutineLimit = 0;
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }

    void CreateCell(Vector2 position)
    {
        var instantiatedCell = (Instantiate(gridCell.gameObject, position, gridCell.transform.rotation));
        instantiatedCell.name = "Cell: X[" + (_horizontalGridPosition - 1) + "] - Y[" + _verticalGridPosition + "]";
        instantiatedCell.transform.SetParent(grid.transform);
        _coroutineLimit++;
    }
}