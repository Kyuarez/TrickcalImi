using System.Collections.Generic;

namespace TK.BT
{
    /// <summary>
    /// 여러 노드들을 자식으로 가지고 있어, 조건에 따라 Result 처리
    /// (Selector, Sequence, Parallel의 부모 노드)
    /// </summary>
    public abstract class CompositeNode : Node, IParentOfChildren
    {
        /// <summary>
        /// 현재까지 진행된 자식 노드 인덱스
        /// </summary>
        protected int _currentIndex;

        public List<Node> Children { get; set; }

        public CompositeNode(BT tree) : base(tree) 
        {
            Children = new List<Node>();
        }

        public void AttachChild(Node node)
        {
            Children.Add(node);
        }
    }
}
