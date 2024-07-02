using System.Collections;
using System.Collections.Generic;
using System;

namespace Compiler
{
    public class TokenEnumerator : IEnumerable<Token>
    {
        private List<Token> tokens;
        private int currentTokenIndex;
        public int Position { get { return currentTokenIndex; } }

        public TokenEnumerator(IEnumerable<Token> tokens)
        {
            this.tokens = new List<Token>(tokens);
            currentTokenIndex = 0;
        }
    
        public IEnumerator<Token> GetEnumerator()
        {
            for (int i = currentTokenIndex; i < tokens.Count; i++)
                yield return tokens[i];
        }
    
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    
        private Token Consume(TokenType expectedType, string errorMessage)
        {
            if (Check(expectedType))
            {
                return Advance();
            }
            else
            {
               throw new Exception(errorMessage);
           }
        }
    
        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
            else
            {
                return false;
            }
        }
    
        private bool Check(TokenType type)
        {
           if (tokens[currentTokenIndex].type != TokenType.EOF)
           {
                return false;
            }
            else
            {
                return Peek().type == type;
            }
        }
    
        private Token Advance()
        {
            if (tokens[currentTokenIndex].type != TokenType.EOF)
            {
                currentTokenIndex++;
            }
            return Previous();
        }
    
        private Token Peek()
        {
            return tokens[currentTokenIndex];
        }
    
        private Token Previous()
        {
            return tokens[currentTokenIndex - 1];
        }
    
        public bool CanLookAhead(int k = 0)
        {
            return tokens.Count - currentTokenIndex > k;
        }
    
        public Token LookAhead(int k = 0)
        {
            return tokens[currentTokenIndex + k];
        }
    }
}