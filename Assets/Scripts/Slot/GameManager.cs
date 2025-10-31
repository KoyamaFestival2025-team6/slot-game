using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slot
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;
    
        public event System.Action OnScoreChanged;

        public int Score { get; private set; } = 0;
        
        [SerializeField] Timer timer;
    
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
        /// 3x3のグリッドを判定し、スコアを反映。
        /// 【戻り値】として「当たった柄のリスト」を返します。
        /// </summary>
        /// <param name="grid">3x3のスロット結果</param>
        /// <returns>当たったシンボル(ZodiacSign)のリスト。ハズレの場合は空のリスト。</returns>
        public List<ZodiacSign> ReflectScore(ZodiacSign[,] grid)
        {
            // ★ 1. 当たった柄を格納するためのリストを作成
            List<ZodiacSign> winningSigns = new List<ZodiacSign>();

            // 2. グリッドのチェック (3x3でなければ空のリストを返す)
            if (grid == null || grid.GetLength(0) != 3 || grid.GetLength(1) != 3)
            {
                Debug.LogWarning("スロットの結果が3x3ではありません。");
                Score += 0;
                OnScoreChanged?.Invoke();
                return winningSigns; // 空のリストを返す
            }

            int totalAddScore = 0; 
            int lineScore;     

            // --- 3. 各ラインの判定 ---

            // (a) 横ライン
            lineScore = CheckLine(grid[0, 0], grid[0, 1], grid[0, 2]);
            if (lineScore > 0)
            {
                Debug.Log($"🎉 当たりライン: 上段 ({grid[0, 0]}) - {lineScore}点");
                totalAddScore += lineScore;
                winningSigns.Add(grid[0, 0]); // ★ 当たった柄をリストに追加
            }

            lineScore = CheckLine(grid[1, 0], grid[1, 1], grid[1, 2]);
            if (lineScore > 0)
            {
                Debug.Log($"🎉 当たりライン: 中段 ({grid[1, 0]}) - {lineScore}点");
                totalAddScore += lineScore;
                winningSigns.Add(grid[1, 0]); // ★ 当たった柄をリストに追加
            }

            lineScore = CheckLine(grid[2, 0], grid[2, 1], grid[2, 2]);
            if (lineScore > 0)
            {
                Debug.Log($"🎉 当たりライン: 下段 ({grid[2, 0]}) - {lineScore}点");
                totalAddScore += lineScore;
                winningSigns.Add(grid[2, 0]); // ★ 当たった柄をリストに追加
            }

            // (b) 斜めライン
            lineScore = CheckLine(grid[0, 0], grid[1, 1], grid[2, 2]);
            if (lineScore > 0)
            {
                Debug.Log($"🎉 当たりライン: 右下がり (＼) ({grid[0, 0]}) - {lineScore}点");
                totalAddScore += lineScore;
                winningSigns.Add(grid[0, 0]); // ★ 当たった柄をリストに追加
            }

            lineScore = CheckLine(grid[0, 2], grid[1, 1], grid[2, 0]);
            if (lineScore > 0)
            {
                Debug.Log($"🎉 当たりライン: 左下がり (／) ({grid[0, 2]}) - {lineScore}点");
                totalAddScore += lineScore;
                winningSigns.Add(grid[0, 2]); // ★ 当たった柄をリストに追加
            }

            // 4. 最終スコアを反映
            Score += totalAddScore;
            OnScoreChanged?.Invoke();

            // 5. 最終ログ
            if (totalAddScore > 0)
            {
                Debug.Log($"💡 合計 {totalAddScore}点獲得！ (現在 " + Score + "点)");
            }
            else
            {
                Debug.Log("ハズレ... (獲得 0点)");
            }

            // ★ 6. 当たった柄のリストを返す
            // (もし「Aries」が2ラインで当たった場合、リストに2回入る。
            //  重複を削除したい場合は .Distinct().ToList() を使う)
            return winningSigns.Distinct().ToList(); // 重複を削除して返す
            // return winningSigns; // 重複を許可する場合
        }
   
        private int CheckLine(ZodiacSign s1, ZodiacSign s2, ZodiacSign s3)
        {
            if (s1 == s2 && s2 == s3)
            {
                return ZodiacSignExtensions.GetPoint(s1);
            }
            return 0;
        }
        
        public Timer GetTimer()
        {
            return timer;
        }
    }

}