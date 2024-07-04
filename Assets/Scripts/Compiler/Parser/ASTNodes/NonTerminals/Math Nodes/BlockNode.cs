using System.Collections.Generic;

namespace Compiler
{
    public class BlockNode : StatementNodes
    {
        public List<ASTNode> Statements { get; }

        public BlockNode(List<ASTNode> statements)
        {
            Statements = statements;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            foreach (var item in Statements)
            {
                yield return item;
            }
        }
    }
}