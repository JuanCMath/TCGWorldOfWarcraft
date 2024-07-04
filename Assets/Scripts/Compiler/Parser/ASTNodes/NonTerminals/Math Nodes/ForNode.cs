using System.Collections.Generic;
using UnityEngine;

namespace Compiler
{
    public class ForNode : StatementNodes
    {
        public GameObjectReferenceNode Objetcs { get; }
        public ExpresionNodes Collection { get; }
    
        public ForNode(GameObjectReferenceNode objects, ExpresionNodes collection)
        {
            Objetcs = objects;
            Collection = collection;
        }

    }
}