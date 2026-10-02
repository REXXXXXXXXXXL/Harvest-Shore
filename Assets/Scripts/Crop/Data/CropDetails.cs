using UnityEngine;
[System.Serializable]
public class CropDetails
{
    public int seedItemID;
    [Header("各段階に必要な日数")]
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("成長段階Prefab")]
    public GameObject[] growthPrefabs;
    [Header("成長段階画像")]
    public Sprite[] growthSprites;
    [Header("栽培可能な季節")]
    public Season[] seasons;

    [Space]
    [Header("収穫道具")]
    public int[] harvestToolItemID;
    [Header("道具ごとの使用回数")]
    public int[] requireActionCount;
    [Header("変換先アイテムID")]
    public int transferItemID;

    [Space]
    [Header("収穫アイテム情報")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("再成長時間")]
    public int daysToRegrow;
    public int regrowTimes;

    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;

    public ParticleEffectType particleEffectType;
    public Vector3 particleEffectTypePos;
    public SoundName soundEffect;

    /// <summary>
    /// 检查当前工具是否可用
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)
        {
            if (tool == toolID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获得工具需要使用的次数
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }
}
