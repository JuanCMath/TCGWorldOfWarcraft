using System;
using System.Collections.Generic;

namespace Compiler
{
    public class PredicateNode : StatementNodes
    {
        public GameObjectNode unit { get; set; }
        public ExpresionNodes filter { get; set; }

        public PredicateNode(GameObjectNode unit, ExpresionNodes filter)
        {
            this.unit = unit;
            this.filter = filter;
        }   
    }
}