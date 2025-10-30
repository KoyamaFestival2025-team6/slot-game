using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreTextUI;
    private TextMeshProUGUI _scoreText;

    [SerializeField] private Image beforeImage;
    [SerializeField] private Image SagittariusImage;
    [SerializeField] private Image CapricornImage;
    [SerializeField] private Image LeoImage;
    [SerializeField] private Image VirgoImage;
    [SerializeField] private Image TaurusImage;
    
    private void Start()
    {
        _scoreText = scoreTextUI.GetComponent<TextMeshProUGUI>();
        GameManager.Instance.OnScoreChanged += UpdateScoreText;
    }

    private void Update()
    {
        // デバッグ用
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Enterキーが押された瞬間の処理
            List<ZodiacSign> tmpList = new List<ZodiacSign>();
            tmpList.Add(ZodiacSign.Sagittarius);
            StartCoroutine(PlayHitFeedback(tmpList));
        }
    }

    public void UpdateScoreText()
    {
        if (_scoreText == null) Debug.LogError("Score text is null");
        _scoreText.text = "Score : " + GameManager.Instance.Score;
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
                beforeImage.gameObject.SetActive(false);
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
            case ZodiacSign.Aries:
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
    }
}
