using System.Collections.Generic;

namespace Compiler
{
    public class WhileNode : ExpresionNodes
    {
        public ASTNode Condition { get; }
        public List<StatementNodes> BodyStatements { get; }
    
        public WhileNode(ASTNode condition, List<StatementNodes> bodyStatements)
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