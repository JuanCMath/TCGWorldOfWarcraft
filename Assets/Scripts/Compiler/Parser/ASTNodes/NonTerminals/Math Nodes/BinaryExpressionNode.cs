using System.Collections.Generic;

namespace Compiler
{
    public class BinaryExpressionNode : ExpresionNodes
    {
        public ExpresionNodes Left { get; set; }
        public Token Operator { get; set; }
        public ExpresionNodes Right { get; set; }

        public BinaryExpressionNode(ExpresionNodes left, Token op, ExpresionNodes right)
        {
            this.Left = left;
            this.Operator = op;
            this.Right = right;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Left;
            yield return Right;
        }
    }
}


