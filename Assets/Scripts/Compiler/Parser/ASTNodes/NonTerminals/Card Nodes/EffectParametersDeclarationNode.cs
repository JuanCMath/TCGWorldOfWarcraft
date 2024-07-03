using System.Collections.Generic;

namespace Compiler
{
    public class EffectParametersAssignementNode : StatementNodes
    {
        public List<VariableAssignementNode> Parameters { get; set; }

        public EffectParametersAssignementNode(List<VariableAssignementNode> parameters)
        {
            this.Parameters = parameters;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            foreach (var parameter in Parameters)
                yield return parameter;
        }
    }
}