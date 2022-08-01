using UnityEngine;
using System.Collections;

public class LMCamera : MonoBehaviour
{
    private Camera _camera;
    private float _baseSpeed = 60;
    private float _originalZoom;
    private float _maxZoom;
    private float _minZoom;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _originalZoom = _camera.orthographicSize;
        _maxZoom = _originalZoom - 8f;
        _minZoom = _originalZoom + 20f;
        Destroy(GetComponent<PixelPerfectCamera>());

        if (GameManager.instance.IsEditingLevel())
            StartCoroutine(EqualCameraPosToPlayerPos());
    }

    void LateUpdate()
    {
        Move();
        Zoom();
    }

    void Move()
    {
        var directionalInput = new Vector3(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")));
        var moveSpeed = ((directionalInput * _camera.orthographicSize) / _originalZoom) * _baseSpeed * Time.deltaTime;

        transform.position += moveSpeed;

        var clampedPosX = Mathf.Clamp(transform.position.x, -110, 110);
        var clampedPosY = Mathf.Clamp(transform.position.y, -110, 110);

        transform.position = new Vector3(clampedPosX, clampedPosY, transform.position.z);
    }

    void Zoom()
    {
        if (Input.GetMouseButtonDown(2))
            _camera.orthographicSize = _originalZoom;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (_camera.orthographicSize <= _minZoom)
                _camera.orthographicSize++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (_camera.orthographicSize >= _maxZoom)
                _camera.orthographicSize--;
        }
    }

    IEnumerator EqualCameraPosToPlayerPos()
    {
        yield return new WaitForEndOfFrame();

        var player = GameObject.FindGameObjectWithTag(K.TAG_PLAYER);
        if (player != null && player.GetComponent<Player>())
            transform.position = new Vector3(player.transform.position.x + 12, player.transform.position.y + 4, transform.position.z);
    }
}