using System.Collections.Generic;

namespace Compiler
{
    public class NumberNode : TerminalNodes
    {
        public double Value;

        public NumberNode (double value)
        {
            this.Value = value;
        }
    }
}