using UnityEngine;

namespace TK.BT
{
    /// <summary>
    /// BT ���� ��� : �ֻ��� ���
    /// -> Excute() ����
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

