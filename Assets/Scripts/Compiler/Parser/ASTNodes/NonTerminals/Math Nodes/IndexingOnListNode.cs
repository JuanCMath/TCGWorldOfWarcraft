using System.Collections.Generic;


namespace Compiler
{
    public class IndexingOnListNode : ExpresionNodes
    {
        public ExpresionNodes ExpresionNodes { get; set; }
        public ExpresionNodes Index { get; set; }

        public IndexingOnListNode(ExpresionNodes expresionNodes, ExpresionNodes index)
        {
            ExpresionNodes = expresionNodes;
            Index = index;
        }
    }
}