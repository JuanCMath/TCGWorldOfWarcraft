using System.Collections.Generic;
#nullable enable

namespace Compiler
{
    public class MethodCallNode : ExpresionNodes
    {
        public ExpresionNodes Target { get; }
        public ExpresionNodes MethodName { get; }
        public ASTNode? Arguments { get; }

        public MethodCallNode(ExpresionNodes target, ExpresionNodes methodName, ASTNode arguments)
        {
            MethodName = methodName;
            Target = target;
            Arguments = arguments;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return MethodName;
            yield return Target;
            if (Arguments != null) yield return Arguments;
        }
    }
}