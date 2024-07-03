#nullable enable

using System.Collections.Generic;

namespace Compiler
{
    public class OnActivationNode : StatementNodes
    {
        public List<EffectsToBeActivateNode> effectActivations { get; set; }

        public OnActivationNode(List<EffectsToBeActivateNode> effectActivations)
        {
            this.effectActivations = effectActivations;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            foreach (var child in effectActivations)
            {
                yield return child;
            }
        }
    }
}