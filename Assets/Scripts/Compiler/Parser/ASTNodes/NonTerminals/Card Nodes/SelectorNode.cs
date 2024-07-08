using System;
using System.Collections.Generic;

namespace Compiler
{
    public class SelectorNode : StatementNodes
    {
        public GameObjectReferenceNode source { get; set; }
        public BooleanNode single { get; set; }
        public PredicateNode predicate { get; set; }

        public SelectorNode(GameObjectReferenceNode source, BooleanNode single, PredicateNode predicate)
        {
            this.source = source;
            this.single = single;
            this.predicate = predicate;
        }
    }
}