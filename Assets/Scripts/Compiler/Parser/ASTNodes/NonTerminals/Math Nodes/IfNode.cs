using System.Collections.Generic;

namespace Compiler
{
    public class IfNode : StatementNodes
    {
        public ExpresionNodes Condition { get; set; }
        public List<StatementNodes> ThenStatements { get; set; }
        public List<StatementNodes> ElseStatements { get; set; }

        public IfNode(ExpresionNodes condition,List<StatementNodes> thenStatements, List<StatementNodes> elseStatements)
        {
            this.Condition = condition;
            this.ThenStatements = thenStatements;
            this.ElseStatements = elseStatements;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            foreach (var item in ThenStatements)
            {
                yield return item;
            }
            foreach (var item in ElseStatements)
            {
                yield return item;
            }
        }
        
    }
}