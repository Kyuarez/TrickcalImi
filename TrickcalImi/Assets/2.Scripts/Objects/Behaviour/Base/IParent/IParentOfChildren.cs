using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TK.BT
{
    public interface IParentOfChildren : IParent
    {
        List<Node> Children { get; set; }
    }
}
