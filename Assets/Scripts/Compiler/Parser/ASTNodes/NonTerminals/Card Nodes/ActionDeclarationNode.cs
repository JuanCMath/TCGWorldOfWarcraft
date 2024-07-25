using System.Collections.Generic;

namespace Compiler
{
    public class ActionDeclarationNode : StatementNodes
    {
        public GameObjectReferenceNode Targets { get; }
        public GameObjectReferenceNode Context { get; }
        public BlockNode Body { get; }

        public ActionDeclarationNode(GameObjectReferenceNode targets, GameObjectReferenceNode context, BlockNode body)
        {
            Targets = targets;
            Context = context;
            Body = body;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Body;
        }
    }
}