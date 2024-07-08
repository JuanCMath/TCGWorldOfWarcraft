using System.Collections.Generic;

namespace Compiler
{
    public class ConditionalExpresionNode : StatementNodes
    {
        public ExpresionNodes Condition { get; set; }
        public StatementNodes ThenExpr { get; set; }
        public StatementNodes ElseExpr { get; set; }

        public ConditionalExpresionNode(ExpresionNodes condition, StatementNodes thenExpr, StatementNodes elseExpr)
        {
            this.Condition = condition;
            this.ThenExpr = thenExpr;
            this.ElseExpr = elseExpr;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Condition;
            yield return ThenExpr;
            yield return ElseExpr;
        }
        
    }
}