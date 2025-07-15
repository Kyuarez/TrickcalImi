using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TK.BT
{
    public interface IParentOfChild : IParent
    {
        Node Child { get; set; }
    }
}
