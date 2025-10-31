using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _time;
    
    [SerializeField] private float timeLimit = 60.0f;
    private bool _isCount = false;
    public float Time { get => _time; }
    
    public event Action OnTimerEnd;

    void Awake()
    {
        ResetTimer();
    }
    
    void Update()
    {
        if (!_isCount) return;
        
        _time -= UnityEngine.Time.deltaTime;
        if (_time <= 0)
        {
            _time = 0;
        }
    }
    
    public void StartTimer()
    {
        _isCount = true;
    }

    public void ResetTimer()
    {
        _time = timeLimit;
        _isCount = false;
        OnTimerEnd?.Invoke();
    }
}