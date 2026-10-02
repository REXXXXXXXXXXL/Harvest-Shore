using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarvestShore.Transition
{
   public class Teleport : MonoBehaviour
   {
    public string sceneToGo;
        public Vector3 positionToGo;//坐标

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))//判断是否是玩家
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
   }
}
