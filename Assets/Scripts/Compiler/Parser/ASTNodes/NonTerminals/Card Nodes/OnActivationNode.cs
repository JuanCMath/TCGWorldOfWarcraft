#nullable enable

using System.Collections.Generic;

namespace Compiler
{
    public class OnActivationNode : StatementNodes
    {
        public EffectActivationNode? EffectActivation;
        public SelectorNode? SelectorNode;
        public OnActivationNode? OnActivation;

        public OnActivationNode (EffectActivationNode effectActivation, SelectorNode selectorNode, OnActivationNode onActivation)
        {
            this.EffectActivation = effectActivation;
            this.SelectorNode = selectorNode;
            this.OnActivation = onActivation;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return EffectActivation;
            yield return SelectorNode;
            yield return OnActivation;
        }
    }
}