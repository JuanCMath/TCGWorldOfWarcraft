using System.Collections.Generic;

namespace Compiler
{
    public class PropertyCallNode : ExpresionNodes
    {
        public StringNode PropertyName { get; }
        public GameObjectReferenceNode Target { get; }

        public PropertyCallNode(StringNode propertyName, GameObjectReferenceNode target)
        {
            PropertyName = propertyName;
            Target = target;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return PropertyName;
            yield return Target;
        }
    }
}