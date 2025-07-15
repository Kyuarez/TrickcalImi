
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
            Result result = Result.Success;
            foreach (var child in Children) 
            {
                result = child.Excute();
                switch (result)
                {
                    case Result.Success:
                        _currentIndex++;
                        break;
                    case Result.Failure:
                        _currentIndex = 0;
                        return result;
                    case Result.Running:
                        return result;
                    default:
                        break;
                }
            }

            _currentIndex = 0;
            return result;
        }
    }
}
