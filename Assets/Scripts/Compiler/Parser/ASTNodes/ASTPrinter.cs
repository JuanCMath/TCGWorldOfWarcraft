using System;
using System.Collections.Generic;

namespace Compiler.Parser.ASTNodes
{
    public class ASTPrinter
    {
        public static void PrintAST(ASTNode root)
        {
            PrintNode(root, 0);
        }

        private static void PrintNode(ASTNode node, int level)
        {
            if (node == null)
                return;

            // Imprimir el nodo actual
            Console.WriteLine($"{GetIndentation(level)}{node}");

            // Imprimir los hijos recursivamente
            foreach (var child in node.Children)
            {
                PrintNode(child, level + 1);
            }
        }

        private static string GetIndentation(int level)
        {
            return new string(' ', level * 4);
        }
    }

    public abstract class ASTNode
    {
        public List<ASTNode> Children { get; } = new List<ASTNode>();

        public override string ToString()
        {
            return GetType().Name;
        }
    }

    // Aquí puedes definir tus propias clases de nodos del AST
    // por ejemplo:
    public class BinaryExpressionNode : ASTNode
    {
        public ASTNode Left { get; set; }
        public ASTNode Right { get; set; }

        public override string ToString()
        {
            return "BinaryExpression";
        }
    }
}