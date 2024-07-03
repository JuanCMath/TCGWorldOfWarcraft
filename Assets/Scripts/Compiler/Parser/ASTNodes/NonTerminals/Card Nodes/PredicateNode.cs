using System;
using System.Collections.Generic;

namespace Compiler
{
    public class PredicateNode : StatementNodes
    {
        public GameObjectReferenceNode identifier { get; set; }
        public ExpresionNodes Condition { get; set; }

        public PredicateNode(GameObjectReferenceNode identifier, ExpresionNodes condition)
        {
            this.Condition = condition;
        }
    }
}