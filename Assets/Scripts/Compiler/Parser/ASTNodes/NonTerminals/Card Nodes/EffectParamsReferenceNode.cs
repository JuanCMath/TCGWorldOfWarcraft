using System.Collections.Generic;

namespace Compiler
{
    public class EffectParamsReferenceNode : StatementNodes
    {
        public List<VariableAssignementNode> Params { get; }

        public EffectParamsReferenceNode(List<VariableAssignementNode> param)
        {
            Params = param;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            if (Params != null)
            foreach (var parameter in Params)
                yield return parameter;
        }
    }
}