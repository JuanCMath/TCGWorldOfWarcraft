using System.Collections.Generic;

namespace Compiler
{
    public class PropertyCallNode : ExpresionNodes
    {
        public ExpresionNodes Target { get; }
        public ExpresionNodes PropertyName { get; }
        

        public PropertyCallNode(ExpresionNodes target, ExpresionNodes propertyName)
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