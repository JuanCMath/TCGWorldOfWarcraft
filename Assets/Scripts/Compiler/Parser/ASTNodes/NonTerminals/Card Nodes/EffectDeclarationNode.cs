using System.Collections.Generic;

namespace Compiler
{
    public class EffectDeclarationNode : StatementNodes
    {
        public StringNode Name { get; }
        public ActionDeclarationNode Action { get; }

        public EffectDeclarationNode(StringNode name, ActionDeclarationNode action)
        {
            Name = name;
            Action = action;
        }
    }
}