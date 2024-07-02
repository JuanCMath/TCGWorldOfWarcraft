using System.Collections.Generic;

namespace Compiler
{
    public class VariableReferenceNode : StatementNodes
    {
        public string Name { get; }

        public VariableReferenceNode(string name)
        {
            Name = name;
        }
    }
}
