using System.Collections.Generic;
using UnityEngine;

namespace Compiler
{
    public class GameObjectNode : TerminalNodes
    {
        public GameObject GameObject { get; set; }

        public GameObjectNode (GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
}