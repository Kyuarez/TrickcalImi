

using System.Reflection;

namespace TK.BT
{
    /// <summary>
    /// ConditionNode: Leaf노드 중 하나로서 특정 조건을 검사하여 성공/실패 반환
    /// </summary>
    public class ConditionNode : Node, IParentOfChild
    {
        private PropertyInfo _propertyInfo;

        public Node Child { get; set; }

        public ConditionNode(BT tree, string propertyName) : base(tree)
        {
            
        }


        public void AttachChild(Node node)
        {
            Child = node;
        }

        public override Result Excute()
        {
            //TODO : 조건에 따라서 결과값 조정
            throw new System.NotImplementedException();
        }
    }
}
