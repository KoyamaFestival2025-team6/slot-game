using System.Collections.Generic;
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

    /// <summary>
    /// 3x3のスロット結果を受け取り、横3ライン・斜め2ラインの当たり判定を行いスコアに反映させる
    /// </summary>
    /// <param name="grid">3x3のスロット結果 (grid[行, 列] でアクセス)</param>
    public void ReflectScore(ZodiacSign[,] grid)
    {
        // 1. グリッドが3x3であるか確認（安全対策）
        if (grid == null || grid.GetLength(0) != 3 || grid.GetLength(1) != 3)
        {
            Debug.LogWarning("スロットの結果が3x3ではありません。判定をスキップします。");
            // スコアを加算せずに関数を終了
            Score += 0;
            OnScoreChanged?.Invoke();
            return;
        }

        // 2. 今回のスピンで加算する合計スコアを初期化
        int totalAddScore = 0;
        int lineScore;     // 各ラインの判定結果を一時的に格納する変数
        
        
        // 3. 当たり判定用のヘルパーメソッド（ローカル関数）を定義
        // 3つのシンボルが揃っていれば点数を、揃っていなければ0を返す
        int CheckLine(ZodiacSign s1, ZodiacSign s2, ZodiacSign s3)
        {
            if (s1 == s2 && s2 == s3)
            {
                // 3つ揃った場合
                return ZodiacSignExtensions.GetPoint(s1);
            }
            return 0; // 揃わなかった場合
        }

        // --- 4. 各ラインの判定 [行, 列] ---
        // (a) 横ラインの判定 (3ライン)

        // 上段
        lineScore = CheckLine(grid[0, 0], grid[0, 1], grid[0, 2]);
        if (lineScore > 0)
        {
            // どのシンボルが揃ったか（grid[0, 0]）と点数（lineScore）を表示
            Debug.Log($"🎉 当たりライン: 上段 ({grid[0, 0]}) - {lineScore}点");
            totalAddScore += lineScore;
        }

        // 中段
        lineScore = CheckLine(grid[1, 0], grid[1, 1], grid[1, 2]);
        if (lineScore > 0)
        {
            Debug.Log($"🎉 当たりライン: 中段 ({grid[1, 0]}) - {lineScore}点");
            totalAddScore += lineScore;
        }

        // 下段
        lineScore = CheckLine(grid[2, 0], grid[2, 1], grid[2, 2]);
        if (lineScore > 0)
        {
            Debug.Log($"🎉 当たりライン: 下段 ({grid[2, 0]}) - {lineScore}点");
            totalAddScore += lineScore;
        }

        // (b) 斜めラインの判定 (2ライン)

        // 右下がり (＼)
        lineScore = CheckLine(grid[0, 0], grid[1, 1], grid[2, 2]);
        if (lineScore > 0)
        {
            Debug.Log($"🎉 当たりライン: 右下がり (＼) ({grid[0, 0]}) - {lineScore}点");
            totalAddScore += lineScore;
        }

        // 左下がり (／)
        lineScore = CheckLine(grid[0, 2], grid[1, 1], grid[2, 0]);
        if (lineScore > 0)
        {
            Debug.Log($"🎉 当たりライン: 左下がり (／) ({grid[0, 2]}) - {lineScore}点");
            totalAddScore += lineScore;
        }

        // 5. 最終的なスコアを反映し、イベントを発行する
        Score += totalAddScore;
        OnScoreChanged?.Invoke();

        // 6. 最終的な合計点のデバッグログ
        if (totalAddScore > 0)
        {
            Debug.Log($"💡 合計 {totalAddScore}点獲得！ (現在 " + Score + "点)");
        }
        else
        {
            Debug.Log("ハズレ... (獲得 0点)");
        }
    }
}
