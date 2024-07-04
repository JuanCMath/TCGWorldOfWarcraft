using System.Collections.Generic;

namespace Compiler
{
    public class EffectDeclarationNode : StatementNodes
    {
        public StringNode Name { get; }
        public EffectParamsReferenceNode Params {get; set;} 
        public ActionDeclarationNode Action { get; }

        public EffectDeclarationNode(StringNode name, EffectParamsReferenceNode param, ActionDeclarationNode action)
        {
            Name = name;
            Params = param;
            Action = action;
        }
    }
}