
namespace TK.BT
{
    /// <summary>
    /// 개별 노드마다 부모 기능 제공 : AttachChild(Node)
    /// </summary>
    public interface IParent
    {
        void AttachChild(Node node);
    }
}
