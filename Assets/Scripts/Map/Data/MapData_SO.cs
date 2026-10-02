using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDate_SO", menuName = "Map/MapDate")]
public class MapData_SO : ScriptableObject
{
    [Header("マップ情報")]
    public int gridWidth;
    public int gridHeight;

    [Header("マップ左下原点")] 
    public int originX;
    public int originY;
        
        
    [SceneName] public string sceneName;
    public List<TileProperty> tileProperties;
}
