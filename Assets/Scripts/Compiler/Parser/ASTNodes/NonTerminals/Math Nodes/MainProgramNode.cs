using System.Collections.Generic;

namespace Compiler
{
    public class MainProgramNode : ASTNode
    {
        public List<ASTNode> Body { get; set; }

        public MainProgramNode(List<ASTNode> body)
        {
            this.Body = body;
        } 

        public override IEnumerable<ASTNode> GetChildren()
        {
            foreach (var node in Body)
                yield return node;
        }
    }
}