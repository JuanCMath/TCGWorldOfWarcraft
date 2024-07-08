using System.Collections.Generic;

namespace Compiler
{
    public class ActionDeclarationNode : StatementNodes
    {
        public GameObjectReferenceNode Targets { get; }
        public GameObjectReferenceNode Context { get; }
        public StatementNodes Action { get; }

        public ActionDeclarationNode(GameObjectReferenceNode targets, GameObjectReferenceNode context, StatementNodes action)
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