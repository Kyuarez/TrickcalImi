
namespace TK.BT
{
    /// <summary>
    /// 자식 노드를 순서대로 실행하며, 하나라도 실패가 나오면 순회 종료하고 실패 반환
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        public SequenceNode(BT tree) : base(tree) { }

        public override Result Excute()
        {
            throw new System.NotImplementedException();
        }
    }
}
