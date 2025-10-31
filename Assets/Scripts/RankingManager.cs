using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq; // LINQ (OrderByDescendingなど) を使用するために必要

public class RankingManager : MonoBehaviour
{
    // JSONファイルのファイル名
    private const string FileName = "ranking_data.json";
    // ファイルパス
    private string filePath;

    // 現在のランキングデータ
    private RankingData currentRankingData;

    // ランキングに保持する最大件数（インスペクターから設定可能にする）
    [SerializeField]
    private int maxRankCount = 10;

    void Awake()
    {
        // 永続的なデータ保存先パスを設定
        filePath = Path.Combine(Application.persistentDataPath, FileName);

        Debug.Log("JSONファイルの保存先パス: " + filePath);

        // アプリ起動時にランキングデータをロード
        currentRankingData = LoadRankingData();

        // 参照しやすいよう、Singletonパターンとして自身のインスタンスを保持
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする場合
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 外部からアクセスするためのシングルトンインスタンス
    public static RankingManager Instance { get; private set; }


    // --- public メソッド ---

    /// <summary>
    /// 新しいスコアをランキングに追加し、更新・保存する
    /// </summary>
    /// <param name="newScore">今回獲得したスコア</param>
    public void AddNewScore(int newScore)
    {
        // 1. 新しいエントリを作成してリストに追加
        RankEntry newEntry = new RankEntry { score = newScore };
        currentRankingData.rankingList.Add(newEntry);

        // 2. スコアの高い順にソート (LINQを使用)
        // OrderByDescending: スコアが大きい順 (降順) に並べ替え
        currentRankingData.rankingList = currentRankingData.rankingList
            .OrderByDescending(entry => entry.score)
            .ToList();

        // 3. ランキングの件数を制限
        if (currentRankingData.rankingList.Count > maxRankCount)
        {
            // 最大件数を超えた分を削除
            currentRankingData.rankingList.RemoveRange(maxRankCount, currentRankingData.rankingList.Count - maxRankCount);
        }

        // 4. 順位を再割り当て
        for (int i = 0; i < currentRankingData.rankingList.Count; i++)
        {
            currentRankingData.rankingList[i].rank = i + 1; // リストのインデックス + 1 が順位
        }

        // 5. JSONファイルに保存
        SaveRankingData();

        Debug.Log($"ランキングに新しいスコア {newScore} を登録しました。");
    }

    /// <summary>
    /// 現在のランキングデータを取得する
    /// </summary>
    public List<RankEntry> GetRankingList()
    {
        // 外部にリストのコピーを渡すことで、オリジナルデータが意図せず変更されるのを防ぐ
        return new List<RankEntry>(currentRankingData.rankingList);
    }

    // --- private メソッド (ファイル操作) ---

    /// <summary>
    /// JSONファイルからデータをロードする
    /// </summary>
    private RankingData LoadRankingData()
    {
        if (File.Exists(filePath))
        {
            try
            {
                // ファイルからテキストを読み込む
                string jsonString = File.ReadAllText(filePath);
                // JSONからオブジェクトにデシリアライズ
                return JsonUtility.FromJson<RankingData>(jsonString);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ランキングデータのロードに失敗しました。新規データを作成します。エラー: {e.Message}");
                return new RankingData();
            }
        }
        else
        {
            // ファイルが存在しない場合は新規データを作成
            return new RankingData();
        }
    }

    /// <summary>
    /// ランキングデータをJSONファイルに保存する
    /// </summary>
    private void SaveRankingData()
    {
        try
        {
            // オブジェクトからJSON文字列にシリアライズ (第2引数trueで整形して保存)
            string jsonString = JsonUtility.ToJson(currentRankingData, true);
            // ファイルに書き込む
            File.WriteAllText(filePath, jsonString);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ランキングデータの保存に失敗しました。エラー: {e.Message}");
        }
    }
}