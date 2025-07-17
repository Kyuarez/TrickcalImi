/*
 [Builder Generated]
 유니티 에디터 툴로 클래스들 생성, 직접 수정 금지
*/

using System;

namespace TK.BT
{
	public partial class Builder
	{
		public Builder ActionNode(Func<Result> action)
		{
			Node node = new ActionNode(_bt, action);
			AttachChild(_currentNode, node);
			return this;
		}

		public Builder ConditionNode(string propertyName)
		{
			Node node = new ConditionNode(_bt, propertyName);
			AttachChild(_currentNode, node);
			return this;
		}

		public Builder SelectorNode()
		{
			Node node = new SelectorNode(_bt);
			AttachChild(_currentNode, node);
			return this;
		}

		public Builder SequenceNode()
		{
			Node node = new SequenceNode(_bt);
			AttachChild(_currentNode, node);
			return this;
		}

	}
}
