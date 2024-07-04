using System.Collections.Generic;

namespace Compiler
{
    public class EffectParamsReferenceNode : StatementNodes
    {
        public List<VariableReferenceNode> Params { get; }

        public EffectParamsReferenceNode(List<VariableReferenceNode> param)
        {
            Params = param;
        }
    }
}