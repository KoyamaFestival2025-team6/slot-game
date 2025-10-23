using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // スロットゲームが終了し、スコアが確定したと仮定するメソッド
    public void EndGameAndRegisterScore(int finalScore)
    {
        if (RankingManager.Instance == null)
        {
            Debug.LogError("RankingManagerがシーンに見つかりません。Awake()が実行されているか確認してください。");
            return;
        }

        // 1. スコアをRankingManagerに登録
        RankingManager.Instance.AddNewScore(finalScore);

        // 2. ランキングを取得してコンソールに表示（UI表示の代わり）
        DisplayCurrentRanking();
    }

    private void DisplayCurrentRanking()
    {
        Debug.Log("--- 現在のランキング ---");
        List<RankEntry> ranks = RankingManager.Instance.GetRankingList();

        if (ranks.Count == 0)
        {
            Debug.Log("ランキングは空です。");
            return;
        }

        foreach (var entry in ranks)
        {
            // UIに表示する際は、ここでTextコンポーネントなどに値をセットします
            Debug.Log($"順位: {entry.rank} 位, スコア: {entry.score}");
        }
        Debug.Log("----------------------");
    }


    // --- テスト用 ---
    // デバッグ目的で、キーボード入力でテストスコアを登録する例
    void Update()
    {
        // Pキーを押すとランダムスコアを登録
        if (Input.GetKeyDown(KeyCode.P))
        {
            int randomScore = Random.Range(500, 3000);
            Debug.Log($"テストスコア: {randomScore} でゲーム終了処理を実行します。");
            EndGameAndRegisterScore(randomScore);
        }
    }
}