using System.Collections.Generic;
using Enums;

namespace Compiler
{
    public class VariableDeclarationNode : StatementNodes
    {
        public string Name { get; set; }
        public VariableType Type { get; set; }
        public ExpresionNodes Initializer { get; set; }

        public VariableDeclarationNode(string identifier, VariableType type, ExpresionNodes initializer)
        {
            this.Name = identifier;
            this.Type = type;
            this.Initializer = initializer;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Initializer;
        }
    }
}