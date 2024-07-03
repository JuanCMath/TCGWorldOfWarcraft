using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;

namespace Compiler
{
    public class PostActionNode : StatementNodes
    {
        public EffectParametersAssignementNode parameters { get; set; }
        public SelectorNode selector { get; set; }

        public PostActionNode(EffectParametersAssignementNode parameters, SelectorNode selector)
        {
            this.parameters = parameters;
            this.selector = selector;
        }
    }
}