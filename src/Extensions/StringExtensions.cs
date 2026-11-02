using System.Text;
using CodeCrafters.Shell.Classes;

namespace CodeCrafters.Shell.Extensions;

internal static class StringExtensions
{
    
    enum TokenizerStateMachine
    {
        Normal,
        InsideSingleQuote,
        InsideDoubleQuote,
        EndOfToken,
        EndOfInput,
        NextCharacterLiteral,
        DoubleQuoteNextCharacterLiteralMaybe
    }
    
    extension(string value)
    {
        
        internal Pipeline Tokenize()
        {
            string rawInvocation = value;
            List<PipelineStage> pipelineResult = [];
            
            // we're going to shortcut the background job process and look for a raw " &" at the end of the string
            bool backgroundJob = false;
            
            TokenizerStateMachine stateMachine = TokenizerStateMachine.Normal;
            int index = 0;
            bool done = false;
            StringBuilder stringBuilder = new();
            List<Token> pipelineStageResult = new();

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
                        else if (nextChar.Equals('\\'))
                        {
                            stateMachine = TokenizerStateMachine.NextCharacterLiteral;
                        }
                        else if (nextChar.Equals('"'))
                        {
                            stateMachine = TokenizerStateMachine.InsideDoubleQuote;
                        }
                        else if (nextChar.Equals('\''))
                        {
                            stateMachine = TokenizerStateMachine.InsideSingleQuote;
                        }
                        else if (nextChar.Equals('|'))
                        {
                            // we received a non-quoted PIPE, so we flush the existing pipeline stage 
                            // to the result set and start a new pipeline stage
                            string newPipelineStageString = stringBuilder.ToString();
                            Token newPipelineStageToken = new(newPipelineStageString);
                            if (!string.IsNullOrEmpty(newPipelineStageString))
                            {
                                pipelineStageResult.Add(newPipelineStageToken);
                            }
                            stringBuilder.Clear();
                            pipelineResult.Add(new PipelineStage(pipelineStageResult.ToArray()));
                            pipelineStageResult = new List<Token>();
                        }
                        else if (nextChar.Equals('&'))
                        {    
                            if (!string.IsNullOrWhiteSpace(value[(index + 1)..])) 
                                throw new InvalidOperationException("Background operator must be at the end of the command.");
                            backgroundJob = true;
                            stateMachine = TokenizerStateMachine.EndOfInput;
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
                    case TokenizerStateMachine.InsideDoubleQuote:
                        char nextCharInDoubleQuote = value[index];
                        if (nextCharInDoubleQuote.Equals('\\'))
                        {
                            stateMachine = TokenizerStateMachine.DoubleQuoteNextCharacterLiteralMaybe;
                        }
                        else if (nextCharInDoubleQuote.Equals('"'))
                        {
                            stateMachine = TokenizerStateMachine.Normal;
                        }
                        else
                        {
                            stringBuilder.Append(nextCharInDoubleQuote);
                        }

                        index++;
                        break;
                    case TokenizerStateMachine.EndOfToken:
                        string newString = stringBuilder.ToString();
                        Token newToken = new(newString);
                        if (!string.IsNullOrEmpty(newString))
                        {
                            pipelineStageResult.Add(newToken);
                        }
                        stringBuilder.Clear();
                        stateMachine = TokenizerStateMachine.Normal;
                        break;
                    case TokenizerStateMachine.EndOfInput:
                        string endOfInputNewString = stringBuilder.ToString();
                        Token endOfInputNewToken = new(endOfInputNewString);
                        if (!string.IsNullOrEmpty(endOfInputNewString))
                        {
                            pipelineStageResult.Add(endOfInputNewToken);
                        }
                        done = true;
                        break;
                    case TokenizerStateMachine.NextCharacterLiteral:
                        stringBuilder.Append(value[index]);
                        stateMachine = TokenizerStateMachine.Normal;
                        index++;
                        break;
                    case TokenizerStateMachine.DoubleQuoteNextCharacterLiteralMaybe:
                        char nextCharInDoubleQuoteMaybeLiteral = value[index];
                        if (nextCharInDoubleQuoteMaybeLiteral.Equals('\"'))
                        {
                            stringBuilder.Append(nextCharInDoubleQuoteMaybeLiteral);
                        }
                        else if (nextCharInDoubleQuoteMaybeLiteral.Equals('\\'))
                        {
                            stringBuilder.Append(nextCharInDoubleQuoteMaybeLiteral);
                        }
                        else
                        {
                            stringBuilder.Append("\\");
                            stringBuilder.Append(nextCharInDoubleQuoteMaybeLiteral);
                        }
                        stateMachine = TokenizerStateMachine.InsideDoubleQuote;
                        index++;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown state");
                }
            }
            
            pipelineResult.Add(new PipelineStage(pipelineStageResult.ToArray()));
            return new Pipeline(pipelineResult)
            {
                BackgroundJob = backgroundJob,
                Invocation = rawInvocation
            };
        }   
    }
}