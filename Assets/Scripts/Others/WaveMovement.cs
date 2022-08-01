using System.Collections;
using UnityEngine;

public class WaveMovement : MonoBehaviour
{
    public Vector3 axis;
    public bool randomSpeed;
    public float speed = 5f;
    public float movementForce = 30f;
    public float initialDelay;

    private Vector3 _startPosition;
    private bool _ableToMove;
    private float _timeInIdle;

    void Start()
    {
        if (randomSpeed)
            speed = Random.Range(3f, 8f);

        _startPosition = transform.position;

        StartCoroutine(Delay());
    }

    void Update()
    {
        if (_ableToMove)
            transform.position = _startPosition + (axis * Mathf.Sin((Time.time - _timeInIdle) * speed) * movementForce);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(initialDelay);

        _timeInIdle = Time.time;
        _ableToMove = true;
    }
}