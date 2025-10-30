using System.Collections.Generic;
using UnityEngine;
using System; // Action (イベント) を使うために必要

public class Reel : MonoBehaviour
{
    public bool IsRotating { get; private set; }
    private Transform _transform;
    
    private ZodiacSign _currentZodiacSign = ZodiacSign.Aries;
    private float _accumulatedRotation = 0.0f;

    [SerializeField] private float speed = 12.0f;

    // --- ▼ 修正点 1: 必要な変数を追加 ▼ ---

    // 1サインあたりの角度 (360 / 12 = 30度)
    private float DEGREES_PER_SIGN; 
    
    // 停止処理中かどうかのフラグ
    private bool _isStopping = false;
    // 停止目標の角度（_accumulatedRotation がこの値になったら止まる）
    private float _targetRotation = 0.0f; 

    // --- ▼ 修正点 2: イベント（コールバック）を追加 ▼ ---
    /// <summary>
    /// リールの回転が「完全に」停止した時に呼び出されるイベント
    /// (停止したリール自身, 停止結果のリスト) を通知する
    /// </summary>
    public event Action<Reel, List<ZodiacSign>> OnReelStopped;


    void Start()
    {
        _transform = this.gameObject.GetComponent<Transform>();
        if (_transform == null) Debug.LogError("Reel transform is null");

        // ゼロ除算を避ける
        if (ZodiacSignExtensions.TotalSigns > 0)
        {
            DEGREES_PER_SIGN = 360.0f / ZodiacSignExtensions.TotalSigns;
        }
        else
        {
            DEGREES_PER_SIGN = 30.0f; // フォールバック
            Debug.LogError("ZodiacSignExtensions.TotalSigns が 0 です。");
        }
    }

    void Update()
    {
        if (!IsRotating) return; // 回転してなければ何もしない

        float rotationAmount = speed * Time.deltaTime;

        // --- ▼ 修正点 3: 停止処理ロジック ▼ ---
        if (_isStopping)
        {
            // (停止要求が出ている場合)
            
            // 次のフレームで目標角度を超えるか判定
            if (_accumulatedRotation + rotationAmount >= _targetRotation)
            {
                // 目標角度を超える場合 (またはピッタリの場合)

                // 1. 超えすぎた分 (overshoot) を計算
                float overshoot = (_accumulatedRotation + rotationAmount) - _targetRotation;
                
                // 2. ちょうど目標角度で止まるように、今回の回転量を調整
                rotationAmount = rotationAmount - overshoot;
                
                // 3. 状態を「完全停止」にする
                IsRotating = false;
                _isStopping = false;

                // 4. 回転を適用（これでピッタリ止まる）
                _transform.Rotate(0, rotationAmount, 0);
                _accumulatedRotation += rotationAmount; // _targetRotation とほぼ同じ値になる

                // 5. 停止したシンボルを更新（もし必要なら）
                // (元のロジックでは _accumulatedRotation がリセットされる時にシンボル更新なので、
                //  ここでは現在の _currentZodiacSign を使うのが正しい)
                
                // 6. 停止したことをイベントで通知する
                OnReelStopped?.Invoke(this, CalculateStopResults());

                // Debug.Log($"リール停止: {_currentZodiacSign} (ぴったり停止)");
                return; // これ以上 Update 処理をしない
            }
        }

        // (通常の回転中、または停止位置まで回転中の場合)
        _transform.Rotate(0, rotationAmount, 0);
        _accumulatedRotation += rotationAmount;
        
        // 元のシンボル更新ロジック
        if (_accumulatedRotation >= DEGREES_PER_SIGN)
        {
            _currentZodiacSign = _currentZodiacSign.Next();
            _accumulatedRotation -= DEGREES_PER_SIGN;
            
            // もし停止処理中にシンボルをまたいだ場合、目標角度もリセットする
            if (_isStopping)
            {
                _targetRotation = 0.0f; // _accumulatedRotation がリセットされたので、目標も 0 (次の DEGREES_PER_SIGN) になる
                
                // ★注意：もし speed が速すぎて 1フレームで 30度以上回る場合、
                // ここの _targetRotation の再設定は「次に止まるべき 30度」にする必要があります。
                // 今のコードは「次に _accumulatedRotation が DEGREES_PER_SIGN になるまで」回る想定です。
            }
        }
    }

    public void StartRotating()
    {
        IsRotating = true;
        _isStopping = false;
    }

    // --- ▼ 修正点 4: StopRotating を RequestStop に変更 ▼ ---
    /// <summary>
    /// リールの停止「要求」を出す。すぐには止まらない。
    /// </summary>
    public void RequestStop()
    {
        if (!IsRotating || _isStopping)
        {
            return; // 既に止まっているか、停止処理中
        }

        _isStopping = true;
        
        // 現在の回転量（0～30度の間）から、次に止まるべき角度（30度）を設定
        _targetRotation = DEGREES_PER_SIGN;
        
        // Debug.Log($"停止要求。現在 {_accumulatedRotation}度。目標 {_targetRotation}度");
    }

    // --- ▼ 修正点 5: 結果計算を別メソッドに分離 ▼ ---
    /// <summary>
    /// 現在の _currentZodiacSign に基づいて、停止結果（3つのサイン）を計算する
    /// </summary>
    private List<ZodiacSign> CalculateStopResults()
    {
        List<ZodiacSign> zodiacSigns = new List<ZodiacSign>();
        int totalSigns = (int)ZodiacSignExtensions.TotalSigns;
        int currentIndex = (int)_currentZodiacSign;
        
        int previousIndex = (currentIndex - 1 + totalSigns) % totalSigns;
        int nextIndex = (currentIndex + 1) % totalSigns;
        
        zodiacSigns.Add((ZodiacSign)previousIndex);
        zodiacSigns.Add(_currentZodiacSign);
        zodiacSigns.Add((ZodiacSign)nextIndex);
        
        return zodiacSigns;
    }
}