using System.Collections.Generic;

namespace Compiler
{
    public abstract class ASTNode {

        public virtual IEnumerable<ASTNode> GetChildren()
        {
            yield break;
        }
    }

    public abstract class ExpresionNodes : ASTNode {}
    public abstract class StatementNodes: ASTNode {}
    public abstract class TerminalNodes: ExpresionNodes  {}
}
