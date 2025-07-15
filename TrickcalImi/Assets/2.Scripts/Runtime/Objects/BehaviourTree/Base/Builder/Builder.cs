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
            return this;
        }

        public Builder CompleteCurrentComposite()
        {
            return this;
        }

        private void AttachChild(Node parent, Node child)
        {

        }
    }
}
