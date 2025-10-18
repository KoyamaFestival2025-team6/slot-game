using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreTextUI;
    private TextMeshProUGUI _scoreText;
    
    
    private void Start()
    {
        _scoreText = scoreTextUI.GetComponent<TextMeshProUGUI>();
        GameManager.Instance.OnScoreChanged += UpdateScoreText;
    }

    public void UpdateScoreText()
    {
        if (_scoreText == null) Debug.LogError("Score text is null");
        _scoreText.text = "Score : " + GameManager.Instance.Score;
    }
}
