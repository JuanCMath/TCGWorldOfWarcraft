using System.Collections.Generic;

namespace Compiler
{
    public class EffectActivationNode : StatementNodes
    {
        public StringNode Name { get; set;}
        public List<VariableAssignementNode> Parameters { get; set; }

        public EffectActivationNode(StringNode name, List<VariableAssignementNode> parameters)
        {
            this.Name = name;
            this.Parameters = parameters;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Name;
            foreach (var parameter in Parameters)
                yield return parameter;
        }
    }
}