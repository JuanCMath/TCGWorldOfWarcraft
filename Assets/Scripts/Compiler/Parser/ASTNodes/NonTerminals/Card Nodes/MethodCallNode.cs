using System.Collections.Generic;

namespace Compiler
{
    public class MethodCallNode : ExpresionNodes
    {
        public StringNode MethodName { get; }
        public GameObjectReferenceNode Target { get; }
        public ExpresionNodes[] Arguments { get; }

        public MethodCallNode(StringNode methodName, GameObjectReferenceNode target, ExpresionNodes[] arguments)
        {
            MethodName = methodName;
            Target = target;
            Arguments = arguments;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return MethodName;
            yield return Target;
            foreach (var argument in Arguments)
            {
                yield return argument;
            }
        }
    }
}