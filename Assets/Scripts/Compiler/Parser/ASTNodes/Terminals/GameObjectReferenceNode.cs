using System.Collections.Generic;

namespace Compiler
{
    public class GameObjectReferenceNode : TerminalNodes
    {
        public StringNode GameObject { get; set; }

        public GameObjectReferenceNode (StringNode gameObject)
        {
            this.GameObject = gameObject;
        }
    }
}