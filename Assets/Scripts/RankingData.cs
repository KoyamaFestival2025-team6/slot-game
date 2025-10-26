using System.Collections.Generic;
using UnityEngine;

// ランキングの各エントリのデータ構造
[System.Serializable]
public class RankEntry
{
    // 順位
    public int rank;
    // スコア
    public int score;

}

// ランキングデータ全体を保持するコンテナ
[System.Serializable]
public class RankingData
{
    // ランキングエントリのリスト
    public List<RankEntry> rankingList = new List<RankEntry>();
}