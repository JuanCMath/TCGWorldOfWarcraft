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
    }
}