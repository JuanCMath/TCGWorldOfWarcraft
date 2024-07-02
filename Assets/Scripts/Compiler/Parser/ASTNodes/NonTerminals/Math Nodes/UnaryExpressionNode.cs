using System.Collections.Generic;

namespace Compiler
{
    public class UnaryExpressionNode : ExpresionNodes
    {
        public Token Operator { get; set; }
        public ExpresionNodes Expression { get; set; }

        public UnaryExpressionNode(Token op, ExpresionNodes expr)
        {
            this.Operator = op;
            this.Expression = expr;
        }
        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Expression;
        }
    }
}