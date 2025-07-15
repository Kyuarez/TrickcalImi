
namespace TK.BT
{
    /// <summary>
    /// 자식 노드를 순서대로 실행하며, 하나라도 성공하면 즉시 성공 반환(순회 종료)
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        public SelectorNode(BT tree) : base(tree) { }
        
        public override Result Excute()
        {
            Result result = Result.Failure;
            //현재까지 진행된 인덱스 부터 끝까지 순회
            for (int i = _currentIndex; i < Children.Count; i++)
            {
                result = Children[i].Excute();

                switch (result)
                {
                    case Result.Success:
                        _currentIndex = 0;
                        return result;
                    case Result.Failure:
                        _currentIndex++;
                        break;
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
