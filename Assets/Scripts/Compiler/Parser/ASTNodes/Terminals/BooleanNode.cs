namespace Compiler
{
    public class BooleanNode : TerminalNodes
    {
        public bool Value { get; set; }

        public BooleanNode(bool value)
        {
            this.Value = value;
        }
    }
}