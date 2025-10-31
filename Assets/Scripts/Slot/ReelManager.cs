using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReelManager : MonoBehaviour
{
    bool _isRotating = false;
    
    [SerializeField] private Clickable3DObject startButton;
    
    [SerializeField] private const int RealCount = 3;
    
    [SerializeField] private Clickable3DObject[] stopReelButtons;
    [SerializeField] private Reel[] stopReelObjs;
    [SerializeField] private UIManager uiManager;
    
    private readonly Dictionary<Clickable3DObject, Reel> _reels = new Dictionary<Clickable3DObject, Reel>();
    
    // 3x3の結果を格納する2次元配列
    private ZodiacSign[,] _resultsGrid = new ZodiacSign[3, 3];
    
    void Start()
    {
        startButton.OnClicked += StartRotating;

        if (stopReelButtons.Length != RealCount || stopReelObjs.Length != RealCount)
        {
            Debug.LogError("設定したリールの個数とボタンもしくはリール本体の個数が違います");
            return;
        }
        for (int i = 0; i < RealCount; i++)
        {
            _reels.Add(stopReelButtons[i], stopReelObjs[i]);
        }
        
        foreach (var reel in _reels)
        {
            reel.Key.OnClicked += () => reel.Value.RequestStop();
        }
        
        foreach (var pair in _reels)
        {
            // 各リールの OnReelStopped イベントに、
            // これから作成する HandleReelStopped メソッドを登録する
            pair.Value.OnReelStopped += HandleReelStopped;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartRotating()
    {
        foreach (var reel in _reels)
        {
            Reel tmp = reel.Value;
            tmp.StartRotating();
        }
        
        _isRotating = true;
    }
    
    /// <summary>
    /// いずれかのリールが「実際に停止した」ときに Reel.cs から呼び出されるメソッド
    /// </summary>
    private void HandleReelStopped(Reel stoppedReel, List<ZodiacSign> columnResults)
    {
        // ★★★
        // このメソッドの中身は、前回作成した
        // 「StopRotating(Reel stoppedReel)」の中身と
        // ほぼ同じロジックになります。
        // ★★★

        // 1. 止まったリールの結果 (columnResults) は引数で渡される

        // 2. このリールが何列目（0, 1, 2）かを特定する
        int columnIndex = -1;
        Clickable3DObject keyObject = null;

        foreach (var pair in _reels)
        {
            if (pair.Value == stoppedReel)
            {
                keyObject = pair.Key;
                break;
            }
        }
    
        if (keyObject == null)
        {
            Debug.LogError("停止したリールが _reels 辞書に見つかりません！");
            return;
        }
        columnIndex = keyObject.ColumnIndex; 

        // 3. グリッドの該当する「列」に結果を格納する
        if (columnResults != null && columnResults.Count == 3)
        {
            Debug.Log($"リール {columnIndex} が停止。結果: {columnResults[0]} (上), {columnResults[1]} (中), {columnResults[2]} (下)");
            _resultsGrid[0, columnIndex] = columnResults[0];
            _resultsGrid[1, columnIndex] = columnResults[1];
            _resultsGrid[2, columnIndex] = columnResults[2];
        }
        // ... (else 節のデバッグログ) ...

        // 4. 他にまだ回転中のリールがあるかチェック
        foreach (var pair in _reels)
        {
            if (pair.Value.IsRotating) // (stoppedReel 自身は IsRotating が false になっている)
            {
                return; // まだ回転中のリールがあるので、ここで処理を終了
            }
        }
    
        // 5. 全てのリールが停止した場合のみ、ここが実行される
        _isRotating = false; 

        // 6. スコア判定を開始する
        ProcessSpinResults();
    }

    /// <summary>
    /// 全てのリールが停止した後に呼び出され、スコア判定と結果処理を行う
    /// </summary>
    private void ProcessSpinResults()
    {
        Debug.Log("全てのリールが停止しました。スコア判定を開始します。");
        
        List<ZodiacSign> hitSigns = Slot.GameManager.Instance.ReflectScore(_resultsGrid);

        // 7. 当たった柄のリスト（hitSigns）を使って、次の処理を行う
        if (hitSigns.Count > 0)
        {
            // 当たり（hitSigns リストに1つ以上の柄が入っている）
            Debug.Log("当たった柄: " + string.Join(", ", hitSigns));
            StartCoroutine(uiManager.PlayHitFeedback(hitSigns));
            // TODO:
            // - hitSigns リストの内容に基づいて、特別な演出（エフェクト）を開始する
            // - （例: hitSigns[0] のシンボルを光らせる、など）
        }
        else
        {
            // ハズレ
        }
        

        // TODO:
        // - スピンボタンを再度押せるようにする
        // - 獲得スコアの演出（エフェクト）を開始する
    }
}
