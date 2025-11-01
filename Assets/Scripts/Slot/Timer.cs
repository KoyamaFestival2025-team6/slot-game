using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _time;
    
    [SerializeField] private float timeLimit = 60.0f;
    private bool _isCount = false;
    public float Time { get => _time; }
    
    public event Action OnTimerEnd; // 時間切れを通知する

    void Awake()
    {
        ResetTimer();
        Slot.GameManager.Instance.OnStart += StartTimer;
        Slot.GameManager.Instance.OnStop += StopTimer;
    }
    
    void Update()
    {
        if (!_isCount) return;
        
        _time -= UnityEngine.Time.deltaTime;
        if (_time <= 0)
        {
            _time = 0;
            _isCount = false; // ★ カウントダウンを停止する
            OnTimerEnd?.Invoke(); // ★ 時間切れイベントをここで発行する
        }
    }
    
    
    // タイマーを作動させる
    public void StartTimer()
    {
        _isCount = true;
    }
    
    // タイマーを停止させる
    public void StopTimer()
    {
        _isCount = false;
    }

    // タイマーをリセットさせる
    public void ResetTimer()
    {
        _time = timeLimit;
        _isCount = false;
    }
}