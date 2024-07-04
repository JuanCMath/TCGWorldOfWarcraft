using System.Collections.Generic;

namespace Compiler
{
    public class WhileNode : StatementNodes
    {
        public ASTNode Condition { get; }
        public List<ASTNode> BodyStatements { get; }
    
        public WhileNode(ASTNode condition, List<ASTNode> bodyStatements)
        {
            Condition = condition;
            BodyStatements = bodyStatements;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Condition;
            foreach (var item in BodyStatements)
            {
                yield return item;
            }
        }
    }
}