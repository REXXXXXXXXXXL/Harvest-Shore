using UnityEngine;
using UnityEngine.Events;


namespace HarvestShore.Dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("会話詳細")]
        public Sprite faceImage;
        public bool onLeft;
        public string name;
        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        [HideInInspector] public bool isDone;
    }
}
