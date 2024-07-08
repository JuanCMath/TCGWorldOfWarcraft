using System.Collections.Generic;
#nullable enable

namespace Compiler
{
    public class PostActionNode : StatementNodes
    {
        public EffectParametersAssignementNode parameters { get; set; }
        public SelectorNode? selector { get; set; }
        public PostActionNode? postAction {get; set;}

        public PostActionNode(EffectParametersAssignementNode parameters, SelectorNode selector, PostActionNode postAction)
        {
            this.postAction = postAction;
            this.parameters = parameters;
            this.selector = selector;
        }
    }
}