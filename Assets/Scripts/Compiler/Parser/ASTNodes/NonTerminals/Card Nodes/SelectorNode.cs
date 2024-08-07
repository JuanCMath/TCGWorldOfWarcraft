using System;
using System.Collections.Generic;

namespace Compiler
{
    public class SelectorNode : StatementNodes
    {
        public StringNode source { get; set; }
        public BooleanNode single { get; set; }
        public PredicateNode predicate { get; set; }

        public SelectorNode(StringNode source, BooleanNode single, PredicateNode predicate)
        {
            this.source = source;
            this.single = single;
            this.predicate = predicate;
        }
    }
}