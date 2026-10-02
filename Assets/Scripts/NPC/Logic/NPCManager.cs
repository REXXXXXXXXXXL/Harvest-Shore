using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
     public SceneRouteDataList_SO sceneRouteDate;
     public List<NPCPosition> npcPositionList;

     private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

     protected override void Awake()
     {
          base.Awake();

          InitSceneRouteDict();
     }

     private void OnEnable()
     {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
     }
     
     private void OnDisable()
     {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
     }

    private void OnStartNewGameEvent(int obj)
    {
        foreach (var character in npcPositionList)
        {
            if (character.npc == null)
                continue;

            character.npc.position = character.position;
            if (character.npc.TryGetComponent(out NPCMovement movement))
                movement.currentScene = character.startScene;
        }
    }

    /// <summary>
    /// 初始化路径字典
    /// </summary>
    private void InitSceneRouteDict()
    {
        if (sceneRouteDate == null || sceneRouteDate.sceneRouteList == null)
            return;

        if (sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                if (sceneRouteDict.ContainsKey(key))
                    continue;

                sceneRouteDict.Add(key, route);
            }
        }
    }

    /// <summary>
    /// 获得两个场景间的路径
    /// </summary>
    /// <param name="fromSceneName">起始场景</param>
    /// <param name="gotoSceneName">目标场景</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        sceneRouteDict.TryGetValue(fromSceneName + gotoSceneName, out SceneRoute route);
        return route;
    }
}
