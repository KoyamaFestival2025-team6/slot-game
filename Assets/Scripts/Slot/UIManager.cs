using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Slot;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreTextUI;
    private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI timerText;


    private UISceneName _currentScene = UISceneName.Title;
    [SerializeField] private List<UIScene> rootUISceneObjects;

    [SerializeField] private Image beforeImage;
    [SerializeField] private Image SagittariusImage;
    [SerializeField] private Image CapricornImage;
    [SerializeField] private Image LeoImage;
    [SerializeField] private Image VirgoImage;
    [SerializeField] private Image TaurusImage;
    
    [SerializeField] private Button startButton; // Titleシーンのstartボタン
    [SerializeField] private Button rankingButton; // Titleシーンのランキングボタン
    
    [SerializeField] private Button goToGameButton1;　// Poseシーンのゲーム再開ボタン1
    [SerializeField] private Button goToGameButton2; // Poseシーンのゲーム再開ボタン2
    [SerializeField] private Button goToGameButton3; // リザルト画面のゲーム再開ボタン
    [SerializeField] private Button goToTitleButton; // Poseシーンのタイトルボタン
    [SerializeField] private Button goToTitleButton2; // リザルト画面のタイトルボタン

    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    
    [SerializeField] private TextMeshProUGUI resultText; // リザルト画面のテキスト
    
    [SerializeField] private CameraManager cameraManager;
    
    [SerializeField] Timer timer;
    
    private void Start()
    {
        _scoreText = scoreTextUI.GetComponent<TextMeshProUGUI>();
        Slot.GameManager.Instance.OnScoreChanged += UpdateScoreText;

        normalButton.gameObject.GetComponent<Image>().color = Color.red;
            
        startButton.onClick.AddListener(() =>
        {
            ActivateUIScene(UISceneName.Game);
            cameraManager.ChangeGameCamera();
        });
        
        rankingButton.onClick.AddListener(() => 
        {
            ActivateUIScene(UISceneName.Ranking);
        });

        goToGameButton1.onClick.AddListener(() =>
        {
            ActivateUIScene(UISceneName.Game);
            Slot.GameManager.Instance.isNotAddPoints = false;
            Slot.GameManager.Instance.StartGame();
        });

        goToGameButton2.onClick.AddListener(() =>
        {
            ActivateUIScene(UISceneName.Game);
            Slot.GameManager.Instance.isNotAddPoints = false;
            Slot.GameManager.Instance.StartGame();
        });
        
        goToGameButton3.onClick.AddListener((() =>
        {
            timer.ResetTimer();
            Slot.GameManager.Instance.ResetScore();
            Debug.Log("Game Start");
            ActivateUIScene(UISceneName.Game);
            Slot.GameManager.Instance.StartGame();
        }));
        
        goToTitleButton.onClick.AddListener(() =>
        {
            // 再度シーンの読み込み
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        });

        goToTitleButton2.onClick.AddListener(() =>
        {
            // 再度シーンの読み込み
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        });
        
        easyButton.onClick.AddListener(() =>
        {
            Slot.GameManager.difficulty = Difficulty.Easy;
            easyButton.gameObject.GetComponent<Image>().color = Color.red;
            normalButton.gameObject.GetComponent<Image>().color = Color.white;
            hardButton.gameObject.GetComponent<Image>().color = Color.white;
        });
        
        normalButton.onClick.AddListener(() =>
        {
            Slot.GameManager.difficulty = Difficulty.Normal;
            easyButton.gameObject.GetComponent<Image>().color = Color.white;
            normalButton.gameObject.GetComponent<Image>().color = Color.red;
            hardButton.gameObject.GetComponent<Image>().color = Color.white;
        });
        
        hardButton.onClick.AddListener((() =>
        {
            Slot.GameManager.difficulty = Difficulty.Hard;
            easyButton.gameObject.GetComponent<Image>().color = Color.white;
            normalButton.gameObject.GetComponent<Image>().color = Color.white;
            hardButton.gameObject.GetComponent<Image>().color = Color.red;
        }));

        // ゲーム終了時GameManagerから呼ばれる
        Slot.GameManager.Instance.OnGameOver += () => 
        {
            Debug.Log("Game Over");
            ActivateUIScene(UISceneName.Result);
            if (resultText == null)
            {
                Debug.LogError("resultText is null");
                return;
            }
            resultText.text = Slot.GameManager.Instance.Score.ToString();
        };
        
        timerText.text = Slot.GameManager.Instance.GetTimer().Time.ToString("00.00");
    }

    private void Update()
    {
        // Escキーを押すとメニュー画面に遷移
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_currentScene == UISceneName.Game)
            {
               ActivateUIScene(UISceneName.Pose);
               Slot.GameManager.Instance.isNotAddPoints = true;
               Slot.GameManager.Instance.StopGame();
            }
            else if (_currentScene == UISceneName.Pose)
            {
                ActivateUIScene(UISceneName.Game);
                Slot.GameManager.Instance.StartGame();
            }else if (_currentScene == UISceneName.Ranking)
            {
                ActivateUIScene(UISceneName.Title);
                cameraManager.ChangeTitleCamera();
            }
        }
        
        // タイマーを更新する
        if (_currentScene == UISceneName.Game || _currentScene == UISceneName.Pose)
        {
            // 毎フレーム、現在のタイマーの値を取得してテキストに設定する
            timerText.text = Slot.GameManager.Instance.GetTimer().Time.ToString("00.00");
        }
    }

    public void UpdateScoreText()
    {
        if (_scoreText == null) Debug.LogError("Score text is null");
        _scoreText.text = "Score : " + Slot.GameManager.Instance.Score;
    }

    public IEnumerator PlayHitFeedback(List<ZodiacSign> zodiacSigns)
    {
        if (zodiacSigns.Count <= 0) yield break;
        
        ZodiacSign goodSign =  ZodiacSign.Aries;  // 当たった中で一番いい柄
        int maxScore = 0;    // 当たった中で一番いいスコア
        
        foreach (var zodiacSign in zodiacSigns)
        {
            if (zodiacSign.GetPoint() > maxScore)
            {
                goodSign = zodiacSign;
                maxScore = zodiacSign.GetPoint();
            }
        }

        if (!(goodSign == ZodiacSign.Taurus || goodSign == ZodiacSign.Virgo || goodSign == ZodiacSign.Leo ||
              goodSign == ZodiacSign.Capricorn || goodSign == ZodiacSign.Sagittarius))
        {
            yield break; // これら以外の柄だったら演出なし
        }
        if(Slot.GameManager.Instance.isNotAddPoints) yield break; // スコアを反映させない場合は演出を出させない
        
        
        beforeImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        Vector3 originPos;
        
        switch (goodSign)
        {
            case ZodiacSign.Sagittarius:
                SagittariusImage.gameObject.SetActive(true);
                originPos = SagittariusImage.gameObject.GetComponent<RectTransform>().anchoredPosition;
                SagittariusImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(2.0f);
                Tween t = SagittariusImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(originPos, 0.3f).SetEase(Ease.InBack,3.0f);
                yield return t.WaitForCompletion();
                SagittariusImage.gameObject.SetActive(false);
                beforeImage.gameObject.SetActive(false);
                break;
            case ZodiacSign.Capricorn:
                CapricornImage.gameObject.SetActive(true);
                originPos = CapricornImage.gameObject.GetComponent<RectTransform>().anchoredPosition;
                CapricornImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(2.0f);
                t = CapricornImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(originPos, 0.3f).SetEase(Ease.InBack,3.0f);
                yield return t.WaitForCompletion();
                CapricornImage.gameObject.SetActive(false);
                break;
            case ZodiacSign.Leo:
                LeoImage.gameObject.SetActive(true);
                originPos = LeoImage.gameObject.GetComponent<RectTransform>().anchoredPosition;
                LeoImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(2.0f);
                t = LeoImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(originPos, 0.3f).SetEase(Ease.InBack,3.0f);
                yield return t.WaitForCompletion();
                LeoImage.gameObject.SetActive(false);
                break;
            case ZodiacSign.Virgo:
                VirgoImage.gameObject.SetActive(true);
                originPos = VirgoImage.gameObject.GetComponent<RectTransform>().anchoredPosition;
                VirgoImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(2.0f);
                t = VirgoImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(originPos, 0.3f).SetEase(Ease.InBack,3.0f);
                yield return t.WaitForCompletion();
                VirgoImage.gameObject.SetActive(false);
                break;
            case ZodiacSign.Taurus:
                TaurusImage.gameObject.SetActive(true);
                originPos = TaurusImage.gameObject.GetComponent<RectTransform>().anchoredPosition;
                TaurusImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(2.0f);
                t = TaurusImage.gameObject.GetComponent<RectTransform>().DOAnchorPos(originPos, 0.3f).SetEase(Ease.InBack,3.0f);
                yield return t.WaitForCompletion();
                TaurusImage.gameObject.SetActive(false);
                break;
            default:
                originPos = Vector3.zero;
                break;
        }
        beforeImage.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 指定されたシーン名のUIをアクティブにし、それ以外を非アクティブにする
    /// </summary>
    private void ActivateUIScene(UISceneName newScene)
    {
        _currentScene = newScene;
    
        foreach (var rootUI in rootUISceneObjects)
        {
            // rootUI の名前が newScene と一致するかどうかで SetActive を切り替える
            bool isActive = (rootUI.GetName() == newScene);
            rootUI.GetRootObject().SetActive(isActive);
        }
    }
}
