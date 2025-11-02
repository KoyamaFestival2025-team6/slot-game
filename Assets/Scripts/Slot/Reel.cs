using System.Collections.Generic;
using UnityEngine;
using System;
// using NUnit.Framework.Constraints; // Action (イベント) を使うために必要 <- System名前空間にあればこれは不要です

public class Reel : MonoBehaviour
{
    // --- ▼ 変更点 1: シンボル順序リストを追加 ▼ ---
    [Header("リールのシンボル順序")]
    [SerializeField]
    private List<ZodiacSign> symbolOrder = new List<ZodiacSign>();
    
    // 現在のシンボルリスト上のインデックス
    private int _currentSymbolIndex = 0;
    // ---------------------------------------------

    public bool IsRotating { get; private set; }
    private Transform _transform;
    
    // _currentZodiacSign はインデックスに基づいて更新されます
    private ZodiacSign _currentZodiacSign = ZodiacSign.Aries; 
    private float _accumulatedRotation = 0.0f;

    private float speed;

    // 1サインあたりの角度
    private float DEGREES_PER_SIGN; 
    
    private bool _isStopping = false;
    private float _targetRotation = 0.0f; 

    public event Action<Reel, List<ZodiacSign>> OnReelStopped;


    void Start()
    {
        _transform = this.gameObject.GetComponent<Transform>();
        if (_transform == null) Debug.LogError("Reel transform is null");

        // --- ▼ 変更点 2: DEGREES_PER_SIGN の計算をリスト基準に変更 ▼ ---
        if (symbolOrder.Count > 0)
        {
            // 角度をリストの総数で割る
            DEGREES_PER_SIGN = 360.0f / symbolOrder.Count;
            
            // 初期シンボルを設定
            _currentSymbolIndex = 0;
            _currentZodiacSign = symbolOrder[_currentSymbolIndex];
        }
        else
        {
            // リストが未設定の場合のフォールバック
            DEGREES_PER_SIGN = 30.0f; // 12サインと仮定
            Debug.LogError("Reel の 'Symbol Order' リストが空です。Inspectorで設定してください。");
        }
        // ----------------------------------------------------
        
        Slot.GameManager.Instance.OnStart += StartRotating;
        Slot.GameManager.Instance.OnStop += RequestStop;
    }

    void Update()
    {
        if (Slot.GameManager.difficulty == Difficulty.Easy)
        {
            speed = 150.0f;
        }else if (Slot.GameManager.difficulty == Difficulty.Normal)
        {
            speed = 300.0f;
        }
        else
        {
            speed = 400.0f;
        }
        
        if (!IsRotating) return; // 回転してなければ何もしない

        float rotationAmount = speed * Time.deltaTime;

        if (_isStopping)
        {
            if (_accumulatedRotation + rotationAmount >= _targetRotation)
            {
                float overshoot = (_accumulatedRotation + rotationAmount) - _targetRotation;
                rotationAmount = rotationAmount - overshoot;
                
                IsRotating = false;
                _isStopping = false;

                _transform.Rotate(0, rotationAmount, 0);
                _accumulatedRotation += rotationAmount;

                // 停止したことをイベントで通知する
                // CalculateStopResults() はリスト基準で計算される (変更点 5 を参照)
                OnReelStopped?.Invoke(this, CalculateStopResults());

                return; 
            }
        }

        _transform.Rotate(0, rotationAmount, 0);
        _accumulatedRotation += rotationAmount;
        
        // --- ▼ 変更点 3: シンボル更新ロジックをリスト基準に変更 ▼ ---
        if (_accumulatedRotation >= DEGREES_PER_SIGN)
        {
            // リストが空か、要素が1つしかない場合はエラーを防ぐ
            if (symbolOrder.Count == 0)
            {
                Debug.LogError("symbolOrder が空のためシンボルを更新できません。");
                IsRotating = false; // エラーとして止める
                return;
            }

            // インデックスを次に進める (リストの最後に到達したら 0 に戻る)
            _currentSymbolIndex = (_currentSymbolIndex + 1) % symbolOrder.Count;
            
            // 現在のシンボルをリストから取得
            _currentZodiacSign = symbolOrder[_currentSymbolIndex];
            
            // 回転量をリセット
            _accumulatedRotation -= DEGREES_PER_SIGN;
            
            if (_isStopping)
            {
                _targetRotation = 0.0f; 
            }
        }
        // --------------------------------------------------------
    }

    public void StartRotating()
    {
        IsRotating = true;
        _isStopping = false;
    }

    public void RequestStop()
    {
        if (!IsRotating || _isStopping)
        {
            return; 
        }

        _isStopping = true;
        
        _targetRotation = DEGREES_PER_SIGN;
    }

    // --- ▼ 変更点 4: 結果計算をリスト基準に変更 ▼ ---
    /// <summary>
    /// 現在の _currentSymbolIndex に基づいて、停止結果（3つのサイン）を計算する
    /// </summary>
    private List<ZodiacSign> CalculateStopResults()
    {
        List<ZodiacSign> zodiacSigns = new List<ZodiacSign>();
        
        if (symbolOrder.Count == 0)
        {
            Debug.LogError("symbolOrder が空のため停止結果を計算できません。");
            // リストが空の場合、とりあえず現在のシンボル（デフォルト）だけ返す
             zodiacSigns.Add(_currentZodiacSign);
             return zodiacSigns;
        }

        int totalSigns = symbolOrder.Count;
        
        // C# の % 演算子は負の値（例: 0 - 1）で期待通りに動かない場合があるため、
        // (totalSigns) を足してから剰余を計算し、インデックスが必ず正になるようにします。
        
        // 前のインデックス
        int previousIndex = (_currentSymbolIndex - 1 + totalSigns) % totalSigns;
        // 次のインデックス
        int nextIndex = (_currentSymbolIndex + 1) % totalSigns;
        
        zodiacSigns.Add(symbolOrder[previousIndex]);
        zodiacSigns.Add(symbolOrder[_currentSymbolIndex]); // これが中央の停止シンボル
        zodiacSigns.Add(symbolOrder[nextIndex]);
        
        return zodiacSigns;
    }
}