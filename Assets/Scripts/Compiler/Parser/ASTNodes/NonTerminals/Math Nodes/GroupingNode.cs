using System.Collections.Generic;

namespace Compiler
{
    public class GroupingNode : ExpresionNodes
    {
        public ExpresionNodes Expression { get; set; }

        public GroupingNode(ExpresionNodes expr)
        {
            this.Expression = expr;
        }
        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Expression;
        }
    }
}