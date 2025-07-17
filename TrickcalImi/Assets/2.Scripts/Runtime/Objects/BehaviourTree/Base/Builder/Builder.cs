using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TK.BT
{
    public partial class Builder
    {
        private BT _bt;
        private Node _currentNode;
        Stack<CompositeNode> _compositeStack;

        public Builder(BT bt)
        {
            _bt = bt;
        }

        public Builder Root()
        {
            _compositeStack = new Stack<CompositeNode>();
            Blackboard blackboard = new Blackboard(_bt.gameObject);
            _bt.blackBoard = blackboard;
            _bt.nodeStack = new Stack<Node>();
            _bt.root = new Root(_bt);
            _currentNode = _bt.root;
            return this;
        }

        public Builder CompleteCurrentComposite()
        {
            if (_compositeStack.Count > 0) 
            {
                _compositeStack.Pop();
            }
            else
            {
                throw new System.Exception("완성할 컴포지트가 없어요");
            }

            if(_compositeStack.Count > 0)
            {
                _currentNode = _compositeStack.Peek();
            }

            return this;
        }

        private void AttachChild(Node parent, Node child)
        {
            if (parent is IParent)
            {
                ((IParent)parent).AttachChild(child);
            }
            else
            {
                throw new System.Exception($"{parent.GetType().Name} 은 자식을 가질 수 없습니다.");
            }

            if (child is IParent)
            {
                if (child is CompositeNode)
                    _compositeStack.Push((CompositeNode)child);

                _currentNode = child;
            }
            else
            {
                if (_compositeStack.Count > 0)
                {
                    _currentNode = _compositeStack.Peek();
                }
                else
                {
                    _currentNode = null;
                }
            }
        }
    }
}
