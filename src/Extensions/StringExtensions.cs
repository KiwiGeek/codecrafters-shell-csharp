using System.Text;
using CodeCrafters.Shell.Classes;

namespace CodeCrafters.Shell.Extensions;

internal static class StringExtensions
{
    
    enum TokenizerStateMachine
    {
        Normal,
        InsideSingleQuote,
        EndOfToken,
        EndOfInput
    }
    
    extension(string value)
    {
        
        internal Token[] Tokenize()
        {
            
            // we're going to use a finite state machine to tokenize the string
            // the states are:
            // 1) normal
            // 2) inside single quote
            // 3) End of token
            // 4) End of input
            // 
            // Normal reads the next character, and determines if it's a single quote. if it is, we transition to Inside single quote
            // if it's whitespace, we transition to end of token.
            // if it's anything else, we append the character to the end of the current StringBuilder, and then loop.
            // if we're in end of token, we commit the token to the collection, move the pointer to the start of the whitespace after it, 
            //   move back to the normal state, and loop.
            // if we're in the inside single quote state, we read the next character. if it's another single quote, then we move on one character
            //   and move back to the normal state. for anything else, we append the next character to the current token and loop.
            
            TokenizerStateMachine stateMachine = TokenizerStateMachine.Normal;
            int index = 0;
            bool done = false;
            StringBuilder stringBuilder = new();
            List<Token> results = new();

            while (!done)
            {
                if (index >= value.Length) { stateMachine = TokenizerStateMachine.EndOfInput; }
                
                switch (stateMachine)
                {
                    case TokenizerStateMachine.Normal:
                        char nextChar = value[index];
                        if (char.IsWhiteSpace(nextChar))
                        {
                            stateMachine = TokenizerStateMachine.EndOfToken;
                        }
                        else if (nextChar.Equals('\''))
                        {
                            stateMachine = TokenizerStateMachine.InsideSingleQuote;
                        }
                        else
                        {
                            stringBuilder.Append(nextChar);
                        }
                        index++;
                        break;
                    case TokenizerStateMachine.InsideSingleQuote:
                        char nextCharInSingleQuote = value[index];
                        if (nextCharInSingleQuote.Equals('\''))
                        {
                            stateMachine = TokenizerStateMachine.Normal;
                        }
                        else
                        {
                            stringBuilder.Append(nextCharInSingleQuote);
                        }

                        index++;
                        break;
                    case TokenizerStateMachine.EndOfToken:
                        string newString = stringBuilder.ToString();
                        Token newToken = new(newString);
                        if (!string.IsNullOrEmpty(newString))
                        {
                            results.Add(newToken);
                        }
                        stringBuilder.Clear();
                        stateMachine = TokenizerStateMachine.Normal;
                        break;
                    case TokenizerStateMachine.EndOfInput:
                        string endOfInputNewString = stringBuilder.ToString();
                        Token endOfInputNewToken = new(endOfInputNewString);
                        if (!string.IsNullOrEmpty(endOfInputNewString))
                        {
                            results.Add(endOfInputNewToken);
                        }
                        done = true;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown state");
                }
            }
            return results.ToArray();
        }   
    }
}