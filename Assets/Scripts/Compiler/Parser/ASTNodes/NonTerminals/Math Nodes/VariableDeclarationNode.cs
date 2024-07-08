using System.Collections.Generic;

namespace Compiler
{
    public class VariableDeclarationNode : ExpresionNodes
    {
        public string Name { get; set; }
        public ExpresionNodes Initializer { get; set; }

        public VariableDeclarationNode(string identifier, ExpresionNodes initializer)
        {
            this.Name = identifier;
            this.Initializer = initializer;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Initializer;
        }
    }
}