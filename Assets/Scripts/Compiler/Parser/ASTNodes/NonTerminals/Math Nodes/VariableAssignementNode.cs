using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Compiler
{
    public class VariableAssignementNode : StatementNodes
    {
        public string Name { get; set; }
        public ExpresionNodes Value { get; set; }

        public VariableAssignementNode(string name, ExpresionNodes value) // Modify the constructor parameter type
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