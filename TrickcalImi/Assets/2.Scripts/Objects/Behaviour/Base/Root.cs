namespace TK.BT
{
    /// <summary>
    /// 최상단 노드, 탐색 시작점으로서 특별한 동작은 없다.
    /// </summary>
    public class Root : Node, IParentOfChild
    {
        public Root(BT tree) : base(tree) { }

        public Node Child { get; set; }

        public void AttachChild(Node node)
        {
            Child = node;
        }

        public override Result Excute()
        {
            return Result.Failure;
        }
    }
}
