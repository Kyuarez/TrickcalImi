using UnityEngine;

namespace TK.BT
{
    /// <summary>
    /// BT 개별 노드 : 최상위 노드
    /// -> Excute() 지원
    /// </summary>
    public abstract class Node
    {
        protected BT _tree;
        //TODO : BlackBoard

        public Node(BT tree)
        {
            _tree = tree;
        }

        public abstract Result Excute();
    }
}

