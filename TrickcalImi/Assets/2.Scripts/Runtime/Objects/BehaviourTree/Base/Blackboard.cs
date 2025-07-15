
using UnityEngine;

namespace TK.BT
{
    /// <summary>
    /// BT 노드들이 공유하는 Owner에 대한 데이터
    /// </summary>
    public class Blackboard
    {
        public GameObject Owner { get; private set; }
        public Transform transform { get; private set; }    

        public Blackboard(GameObject owner)
        {
            Owner = owner;
            transform = owner.transform;
        }
    }
}

