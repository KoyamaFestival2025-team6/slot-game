using UnityEngine;

public class Reel : MonoBehaviour
{
    private bool _isRotating = false;
    private Transform _transform;
    
    private ZodiacSign _currentZodiacSign = ZodiacSign.Aries;
    private float _accumulatedRotation = 0.0f;

    [SerializeField] private float speed = 12.0f;
    
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
            float rotationAmount = speed * Time.deltaTime;
            _transform.Rotate(0, rotationAmount, 0);
            
            _accumulatedRotation += rotationAmount;
            
            if (_accumulatedRotation >= 360.0f / ZodiacSignExtensions.TotalSigns)
            {
                _currentZodiacSign = _currentZodiacSign.Next();
                _accumulatedRotation -= 360.0f / ZodiacSignExtensions.TotalSigns;
                Debug.Log($"次の星座: {_currentZodiacSign.GetJapaneseName()}");
            }
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
