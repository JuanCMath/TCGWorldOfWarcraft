using System.Collections.Generic;
using UnityEngine;

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