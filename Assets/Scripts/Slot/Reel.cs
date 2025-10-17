using UnityEngine;

public class Reel : MonoBehaviour
{
    // private bool _isRotating = false;
    
    private bool _isRotating = true;
    private Transform _transform;
    private Vector3 _startEulerAngles;

    [SerializeField] private float rotationPerFrame = 10.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _transform = this.gameObject.GetComponent<Transform>();
        
        if (_transform == null)
        {
            Debug.LogError("Reel transform is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRotating)
        {
            _transform.Rotate(0, rotationPerFrame * Time.deltaTime, 0);
            Debug.Log(_transform.eulerAngles.x);
        }
    }

    public void StartRotating()
    {
        _isRotating = true;
    }

    public void StopRotating()
    {
        _isRotating = false;
    }
}
