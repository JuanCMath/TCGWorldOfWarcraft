using System.Collections.Generic;

namespace Compiler
{
    public class StringNode : TerminalNodes
    {
        public string Value;

        public StringNode (string value)
        {
            this.Value = value;
        }
    }
}