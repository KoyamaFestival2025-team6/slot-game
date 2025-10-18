using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    
    public event System.Action OnScoreChanged;

    public int Score { get; private set; } = 0;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // シーン遷移後も残す
    }

    public void AddScore(int addScore)
    {
        Score += addScore;
        OnScoreChanged?.Invoke();
    }
}
