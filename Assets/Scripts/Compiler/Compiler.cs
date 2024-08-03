using System;
using System.Collections.Generic;
using System.Diagnostics;
#nullable enable

namespace Compiler
{
    public class Compiler
    {

        public static object? ProcessInput(string input)
        {
            List<Token> tokens = Lexer.Tokenize(input);


            Parse parsedExpresion = new Parse(tokens);
            ASTNode AST = parsedExpresion.Parsing();

            Evaluator evaluator = new Evaluator();
            
            return evaluator.Evaluate(AST);
        }
    }
}