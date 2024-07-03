using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;

namespace Compiler
{
    public class EffectsToBeActivateNode : StatementNodes
    {
        public EffectParametersAssignementNode Effect { get; }
        public SelectorNode Selector { get; }
        public PostActionNode PostAction { get; }

        public EffectsToBeActivateNode(EffectParametersAssignementNode effect, SelectorNode selector, PostActionNode postAction)
        {
            Effect = effect;
            Selector = selector;
            PostAction = postAction;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Effect;
            yield return Selector;
            yield return PostAction;
        }
    }
}