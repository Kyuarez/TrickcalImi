using System;

namespace TK.BT
{
    /// <summary>
    /// ActionNode: Leaf노드 중 하나로서, 실제 주체의 행동 수행
    /// </summary>
    public class ActionNode : Node
    {
        private Func<Result> _action;

        public ActionNode(BT tree, Func<Result> action) : base(tree) 
        {
            _action = action;
        }

        public override Result Excute()
        {
            return _action.Invoke();
        }
    }
}
