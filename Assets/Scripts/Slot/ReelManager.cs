using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    
    /// <summary>
    /// 
    /// </summary>
    
    public void StartRotating()
    {
        Slot.GameManager.Instance.GetTimer().StartTimer();
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
        // --- ▼▼▼ ここにデバッグログを追加 ▼▼▼ ---
        // C# (Unity) でのプログラミング経験がおありとのことですので、
        // ログの整形には StringBuilder を使うのが効率的です。
        StringBuilder gridLog = new StringBuilder();
        gridLog.AppendLine("--- 判定グリッド内容 (日本語名) ---");
    
        // 3x3 のグリッドを [行, 列] でループ
        for (int row = 0; row < 3; row++)
        {
            // 各シンボルを日本語名に変換
            string rowStr = string.Format("[ {0} | {1} | {2} ]",
                ZodiacSignExtensions.GetJapaneseName(_resultsGrid[row, 0]), // [行, 列0]
                ZodiacSignExtensions.GetJapaneseName(_resultsGrid[row, 1]), // [行, 列1]
                ZodiacSignExtensions.GetJapaneseName(_resultsGrid[row, 2])  // [行, 列2]
            );
        
            // "行 0: [ おひつじ座 | おうし座 | ふたご座 ]" のような形式で追加
            gridLog.AppendLine($"行 {row}: {rowStr}");
        }
    
        // 組み立てた文字列をDebug.Logで一括表示
        Debug.Log(gridLog.ToString());
        // --- ▲▲▲ デバッグログ追加 ▲▲▲ ---
        
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
    
    /// <summary>
    /// 現在回転しているリールのうち、最も左側（インデックスが最小）のリールの
    /// 停止「要求」を出します。
    /// </summary>
    public bool StopLeftmostSpinningReel()
    {
        // stopReelObjs[0] (左), [1] (中), [2] (右) の順にチェックします。
        // この配列は Start() で使ったものと同じです。
        
        for (int i = 0; i < stopReelObjs.Length; i++)
        {
            Reel currentReel = stopReelObjs[i];

            // このリールが null でなく、かつ IsRotating が true か確認
            if (currentReel != null && currentReel.IsRotating)
            {
                // 回転中のリールを最初に見つけたら（それが一番左）
                
                Debug.Log($"[ReelManager] 左から {i} 番目（一番左）の回転中リールに停止要求を出します。");
                
                // 停止要求を出す
                Clickable3DObject obj = _reels.FirstOrDefault(x => x.Value == currentReel).Key;
                if (obj != null)
                {
                    var audioSource = obj.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.Play();
                        StartCoroutine(obj.AnimatePress());
                    }
                }
                currentReel.RequestStop();
                
                // 目的（一番左の1つだけを止める）は達成したので、ループを抜ける
                return true;
            }
        }
        
        Debug.Log("[ReelManager] 停止要求：すべて停止済みのため false を返します。");
        return false;
    }
}
