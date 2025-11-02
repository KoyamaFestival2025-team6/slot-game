using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; // EventSystemsの名前空間をインポート

[RequireComponent(typeof(AudioSource))]
// ★ IPointerClickHandler の代わりに IPointerDownHandler を実装
public class Clickable3DObject : MonoBehaviour, IPointerDownHandler
{
    public int ColumnIndex;
    public event Action OnClicked;

    [Header("ボタン設定")]
    [SerializeField]
    private float pressDepth = 0.1f;
    [SerializeField]
    private float pressDuration = 0.05f;

    private Vector3 originalLocalPosition;
    private bool isAnimating = false;
    private AudioSource audioSource;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
        audioSource = GetComponent<AudioSource>();
    }

    // ★ OnPointerClick の代わりに OnPointerDown メソッドを実装
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("3Dモデルが押されました！: " + gameObject.name); // ログも変更

        // SEを再生する
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // アニメーション中でなければ、沈んで浮かぶアニメーションを開始
        if (!isAnimating)
        {
            StartCoroutine(AnimatePress());
        }

        // 元のイベント呼び出し
        // ★ イベント名も OnClicked から OnPressedなどに変えた方が分かりやすいかもしれません
        OnClicked?.Invoke(); 
    }

    public IEnumerator AnimatePress()
    {
        isAnimating = true;
        Debug.Log("あにめーーーーーーーーーーーーーーーーーーーーしょん");

        // --- 1. 沈む ---
        Vector3 pressedPosition = originalLocalPosition - new Vector3(0, pressDepth, 0); 
        
        float elapsed = 0f;
        while (elapsed < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(originalLocalPosition, pressedPosition, elapsed / pressDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = pressedPosition;

        // --- 2. 浮かぶ ---
        elapsed = 0f;
        while (elapsed < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(pressedPosition, originalLocalPosition, elapsed / pressDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalLocalPosition;

        isAnimating = false;
    }
}