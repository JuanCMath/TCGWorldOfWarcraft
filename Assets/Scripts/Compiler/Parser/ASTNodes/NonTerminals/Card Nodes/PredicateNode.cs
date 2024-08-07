namespace Compiler
{
    public class PredicateNode : StatementNodes
    {
        public GameObjectReferenceNode identifier { get; set; }
        public ExpresionNodes Condition { get; set; }

        public PredicateNode(GameObjectReferenceNode identifier, ExpresionNodes condition)
        {
            this.identifier = identifier;
            this.Condition = condition;
        }
    }
}