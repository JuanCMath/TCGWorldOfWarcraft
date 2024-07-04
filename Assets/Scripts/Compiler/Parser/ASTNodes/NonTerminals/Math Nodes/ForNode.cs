using System.Collections.Generic;
using UnityEngine;

namespace Compiler
{
    public class ForNode : StatementNodes
    {
        public GameObjectReferenceNode GameObject { get; set; }
        public VariableReferenceNode Objetcs { get; }
        public List<ASTNode> Body { get; }
    
        public ForNode(GameObjectReferenceNode gameObject, VariableReferenceNode objects, List<ASTNode> body)
        {
            GameObject = gameObject;
            Objetcs = objects;
            Body = body;
        }

    }
}