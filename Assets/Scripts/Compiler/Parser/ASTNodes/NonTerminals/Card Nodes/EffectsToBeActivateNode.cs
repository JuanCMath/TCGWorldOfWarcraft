using System.Collections.Generic;
#nullable enable

namespace Compiler
{
    public class EffectsToBeActivateNode : StatementNodes
    {
        public EffectParametersAssignementNode Effect { get; }
        public SelectorNode? Selector { get; }
        public PostActionNode? PostAction { get; }

        public EffectsToBeActivateNode(EffectParametersAssignementNode effect, SelectorNode selector, PostActionNode postAction)
        {
            Effect = effect;
            Selector = selector;
            PostAction = postAction;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Effect;
            if (Selector != null) yield return Selector;
            if (PostAction != null) yield return PostAction;
        }
    }
}