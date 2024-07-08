using System.Collections.Generic;

namespace Compiler
{
    public class VariableAssignementNode : ExpresionNodes
    {
        public StringNode Name { get; set; }
        public ExpresionNodes Value { get; set; }

        public VariableAssignementNode(StringNode name, ExpresionNodes value) // Modify the constructor parameter type
        {
            this.Name = name;
            this.Value = value;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield break; // Return an empty collection as there are no child nodes
        }
    }
}