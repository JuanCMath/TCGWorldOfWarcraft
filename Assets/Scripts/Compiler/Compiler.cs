using System;
using System.Collections.Generic;
#nullable enable

namespace Compiler
{
    public class Compiler
    {
        public static object? ProcessInput(string input)
        {
            Console.WriteLine($"Processing input: {input}");


            Console.WriteLine("Tokenising");
            List<Token> tokens = Lexer.Tokenize(input);

            Console.WriteLine("Parsing");

            Parse parsedExpresion = new Parse(tokens, default);
            MainProgramNode AST = parsedExpresion.Parsing();

            Console.WriteLine("Evaluating");
            Evaluator evaluator = new Evaluator();
            
            Console.WriteLine($"Compilation finished");
            return evaluator.Evaluate(AST);
        }
    }
}