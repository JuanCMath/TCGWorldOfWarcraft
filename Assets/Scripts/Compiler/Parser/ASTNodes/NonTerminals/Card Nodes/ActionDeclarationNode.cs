using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;

namespace Compiler
{
    public class ActionDeclarationNode : StatementNodes
    {
        public GameObjectNode Targets { get; }
        public GameObjectNode Context { get; }
        public StatementNodes Action { get; }

        public ActionDeclarationNode(GameObjectNode targets, GameObjectNode context, StatementNodes action)
        {
            Targets = targets;
            Context = context;
            Action = action;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Action;
        }
    }
}