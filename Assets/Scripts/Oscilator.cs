using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    [SerializeField] private Vector3 _movementVector;
    private float _movementFactor;
    private Vector3 _startingPos;
    [SerializeField] private float period = 2f;
    private const float tau = Mathf.PI * 2f; // about 6.28

    // Start is called before the first frame update
    void Start()
    {
        _startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period; // grows continually from 0
        float sinWave = Mathf.Sin(cycles * tau); // goes from -1 to +1
        _movementFactor = sinWave / 2 + 0.5f;
        Vector3 offset = _movementVector * _movementFactor;
        transform.position = _startingPos + offset;
    }
}
