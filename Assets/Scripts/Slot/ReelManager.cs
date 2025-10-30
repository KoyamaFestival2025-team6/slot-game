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
            reel.Key.OnClicked += () => StopRotating(reel.Value);
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
    /// リールが1つ停止するたびに呼び出される
    /// </summary>
    /// <param name="stoppedReel">停止したReelオブジェクト</param>
    public void StopRotating(Reel stoppedReel)
    {
        // 1. 止まったリールの結果（3つのサインのリスト）を受け取る
        List<ZodiacSign> columnResults = stoppedReel.StopRotating();

        // 2. このリールが何列目（0, 1, 2）かを特定する
        //    辞書の「Value (stoppedReel)」から「Key (Clickable3DObject)」を逆引きする
        int columnIndex = -1;
        Clickable3DObject keyObject = null;

        foreach (var pair in _reels)
        {
            if (pair.Value == stoppedReel)
            {
                keyObject = pair.Key; // 該当する Key (Clickable3DObject) を見つける
                break;
            }
        }

        if (keyObject != null)
        {
            // 3. Key (Clickable3DObject) が持つ ColumnIndex を使う
            columnIndex = keyObject.ColumnIndex; 
        }
        else
        {
            Debug.LogError("停止したリールが _reels 辞書に見つかりません！");
            return;
        }

        // 4. グリッドの該当する「列」に結果を格納する
        if (columnResults != null && columnResults.Count == 3)
        {
            _resultsGrid[0, columnIndex] = columnResults[0]; // 上段
            _resultsGrid[1, columnIndex] = columnResults[1]; // 中段
            _resultsGrid[2, columnIndex] = columnResults[2]; // 下段
        }
        else
        {
            Debug.LogError("リール " + columnIndex + " から3つの結果が返されませんでした。");
        }

        // 5. 他にまだ回転中のリールがあるかチェック
        foreach (var pair in _reels)
        {
            if (pair.Value.IsRotating)
            {
                return; // まだ回転中のリールがあるので、ここで処理を終了
            }
        }
        
        // 6. ↓↓↓ 全てのリールが停止した場合のみ、ここが実行される ↓↓↓
        _isRotating = false; 

        // 7. 新しいメソッドを呼び出し、スコア判定を開始する
        ProcessSpinResults();
    }

    /// <summary>
    /// 全てのリールが停止した後に呼び出され、スコア判定と結果処理を行う
    /// </summary>
    private void ProcessSpinResults()
    {
        Debug.Log("全てのリールが停止しました。スコア判定を開始します。");
        
        List<ZodiacSign> hitSigns = GameManager.Instance.ReflectScore(_resultsGrid);

        // 7. 当たった柄のリスト（hitSigns）を使って、次の処理を行う
        if (hitSigns.Count > 0)
        {
            // 当たり（hitSigns リストに1つ以上の柄が入っている）
            Debug.Log("当たった柄: " + string.Join(", ", hitSigns));

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
