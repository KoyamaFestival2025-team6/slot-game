using UnityEngine;

public class ReelManager : MonoBehaviour
{
    bool _isRotating = false;
    
    [SerializeField] private Reel[] reels;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ChangeRotationState()
    {
        if (_isRotating)
        {
            StopRotating();
        }
        else
        {
            StartRotating();
        }
    }
    
    private void StartRotating()
    {
        foreach (var reel in reels)
        {
            reel.StartRotating();
        }
        
        _isRotating = true;
    }
    
    private void StopRotating()
    {
        foreach (var reel in reels)
        {
            reel.StopRotating();
        }
        
        _isRotating = false;
    }
    
    public bool IsRotating()
    {
        return _isRotating;
    }
}
