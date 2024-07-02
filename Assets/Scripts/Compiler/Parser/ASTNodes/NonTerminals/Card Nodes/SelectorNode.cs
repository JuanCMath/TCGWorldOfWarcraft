using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Compiler
{
    public class SelectorNode : StatementNodes
    {
        public GameObjectNode source { get; set; }
        public BooleanNode single { get; set; }
        public PredicateNode predicate { get; set; }

        
    }
}