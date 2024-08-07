using System.Collections.Generic;

namespace Compiler
{
    public class NumberNode : TerminalNodes
    {
        public int Value;

        public NumberNode (int value)
        {
            this.Value = value;
        }
    }
}