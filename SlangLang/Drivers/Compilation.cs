using System;
using System.Linq;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Parsing;
using SlangLang.Binding;
using SlangLang.Evaluation;

namespace SlangLang.Drivers
{
    public sealed class Compilation
    {
        readonly Diagnostics diags;
        readonly CompilationOptions options;

        ExpressionNode parseTree;
        
        public Compilation(string[] sourceCode, CompilationOptions compOptions)
        {
            options = compOptions;
            diags = new Diagnostics(DateTime.Now);

            Lexer lexer = new Lexer(diags, sourceCode, "Intepreter");
            LanguageToken[] lexerOutput = lexer.LexAll();
            if (options.printLexerOutput)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                PrettyPrintTokenStream(lexerOutput);
                Console.ResetColor();
            }

            Parser parser = new Parser(diags, lexerOutput);
            parseTree = parser.ParseAll();
            if (options.printParserOutput)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                PrettyPrintParsedTree(parseTree);
                Console.ResetColor();
            }
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            Binder binder = new Binder(diags, parseTree, variables);
            BoundExpression boundTree = binder.BindAll();
            if (options.printBinderOutput)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                PrettyPrintBoundTree(boundTree);
                Console.ResetColor();
            }
            
            Evaluator eval = new Evaluator(diags, boundTree, variables);
            if (diags.HasErrors)
                return new EvaluationResult(null, diags); //just return the error, not the value
            return new EvaluationResult(eval.Evaluate(), diags);
        }

        private static void PrettyPrintTokenStream(LanguageToken[] tokens)
        {}

        private static void PrettyPrintParsedTree(ExpressionNode node, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";
            Console.Write(indent);
            Console.Write(marker);
            Console.WriteLine(node.ToString());

            indent += isLast ? "   " : "│  ";
            ExpressionNode lastChild = node.GetChildren().LastOrDefault();
            foreach (ExpressionNode child in node.GetChildren())
            {
                PrettyPrintParsedTree(child, indent, child == lastChild);
            }
        }

        private static void PrettyPrintBoundTree(BoundExpression node, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";
            Console.Write(indent);
            Console.Write(marker);
            Console.WriteLine(node.ToString());

            indent += isLast ? "   " : "│  ";
            BoundExpression lastChild = node.GetChildren().LastOrDefault();
            foreach (BoundExpression child in node.GetChildren())
            {
                PrettyPrintBoundTree(child, indent, child == lastChild);
            }
        }
    }
}