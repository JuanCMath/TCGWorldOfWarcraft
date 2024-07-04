using System.Collections.Generic;

namespace Compiler
{
    public class VariableReferenceNode : ExpresionNodes
    {
        public StringNode Name { get; }

        public VariableReferenceNode(StringNode name)
        {
            Name = name;
        }
    }
}
