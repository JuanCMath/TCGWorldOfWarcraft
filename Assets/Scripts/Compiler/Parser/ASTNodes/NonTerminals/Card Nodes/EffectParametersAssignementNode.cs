using System;
using System.Collections.Generic;
#nullable enable

namespace Compiler
{
    public class EffectParametersAssignementNode : StatementNodes
    {
        public StringNode Name { get; set; }
        public List<VariableAssignementNode>? Parameters { get; set; }

        public EffectParametersAssignementNode(StringNode name, List<VariableAssignementNode> parameters)
        {
            this.Name = name;
            this.Parameters = parameters;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            if (Parameters != null)
            foreach (var parameter in Parameters)
                yield return parameter;
        }
    }
}