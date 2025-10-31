using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingSceneUIController : MonoBehaviour
{
    // ★ Inspectorから、ランキングを表示するTextコンポーネントを接続します
    [SerializeField]
    private TextMeshProUGUI[] rankTextElements = new TextMeshProUGUI[10];

    // ランキングの最大表示件数（配列の長さに合わせる）
    private const int MAX_DISPLAY_COUNT = 10;

    void Start()
    {
        // 1. RankingManagerが存在するかチェック
        if (RankingManager.Instance == null)
        {
            Debug.LogError("RankingManagerがシーンに存在しません。ランキング表示を中止します。");
            return;
        }

        // 2. データ表示メソッドを呼び出す
        DisplayRanking();
    }

    private void DisplayRanking()
    {
        // 1. RankingManagerからソート済みのリストを取得
        List<RankEntry> rankingList = RankingManager.Instance.GetRankingList();

        // 2. UI要素にデータを設定
        for (int i = 0; i < MAX_DISPLAY_COUNT; i++)
        {
            // 配列の範囲チェック
            if (i >= rankTextElements.Length) break;

            if (i < rankingList.Count)
            {
                // データが存在する場合
                RankEntry entry = rankingList[i];
                // Textに "順位. スコア: スコア値" の形式で表示
                rankTextElements[i].text = $"{entry.rank}位: {entry.score}";
                // テキストを有効にする (非表示にしていた場合)
                rankTextElements[i].enabled = true;
            }
            else
            {
                // データが存在しない場合 (ランキング件数未満の場合)
                rankTextElements[i].text = $"{i + 1}. ----"; // 順位だけ表示し、スコアはハイフンで埋める
                rankTextElements[i].enabled = true;
            }
        }
    }
}