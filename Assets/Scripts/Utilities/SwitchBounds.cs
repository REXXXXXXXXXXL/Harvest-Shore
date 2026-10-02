using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
   //TOD0:切换场景后更改调用

   private void OnEnable()//注册事件
    {
        EventHandler.AfterSceneLoadedEvent += SwitchConfinerShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchConfinerShape;
    }

   private void SwitchConfinerShape()
   {
     if (!SceneTools.TryFindComponentWithTag("BoundsConfiner", out PolygonCollider2D confinerShape))
        return;

     CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
     if (confiner == null)
        return;

     confiner.m_BoundingShape2D = confinerShape;

     //Call this if the bounding shape's points change at runtime
     //如果边界形状的点在运行时发生更改，则调用此函数
     confiner.InvalidatePathCache();
   }
}
