using System;
using System.Linq;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Parsing;
using SlangLang.Binding;
using SlangLang.Evaluation;
using SlangLang.Lowering;

namespace SlangLang.Drivers
{
    public sealed class Compilation
    {
        readonly Diagnostics diags;
        readonly CompilationOptions options;

        CompilationUnit compilationUnit;
        BoundGlobalScope globalScope;
        Compilation previous;

        internal BoundGlobalScope GlobalScope
        { 
            get
            {
                if (globalScope == null)
                {
                    globalScope = Binder.BindGlobalScope(previous?.globalScope, compilationUnit);
                    System.Threading.Interlocked.CompareExchange(ref globalScope, globalScope, null);
                }
                return globalScope;
            }
        }
        
        public Compilation(TextStore source, CompilationOptions compOptions)
        {
            options = compOptions;
            diags = new Diagnostics(DateTime.Now);

            Parser parser = new Parser(diags, source);
            compilationUnit = parser.ParseCompilationUnit();
            if (options.printParserOutput)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                PrettyPrintParsedTree(compilationUnit.statement);
                Console.ResetColor();
            }
        }

        private Compilation(Compilation previous, TextStore source, CompilationOptions compOptions)
        {
            options = compOptions;
            diags = new Diagnostics(DateTime.Now);
            this.previous = previous;

            Parser parser = new Parser(diags, source);
            compilationUnit = parser.ParseCompilationUnit();
            if (options.printParserOutput)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                PrettyPrintParsedTree(compilationUnit.statement);
                Console.ResetColor();
            }
        }

        public Compilation ContinueWith(TextStore source, CompilationOptions newOptions)
        {
            return new Compilation(this, source, newOptions);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            if (options.printBinderOutput)
            {
                //get this before we call GetStatement() to print before lowering takes place
                PrettyPrintBoundTree(GlobalScope.statement);
            }
            BoundStatement statement = GetStatement();
            if (options.printLoweredOutput)
            {
                PrettyPrintBoundTree(statement);
            }
            
            diags.Aggregate(GlobalScope.diagnostics);
            Evaluator eval = new Evaluator(diags, statement, variables);
            if (diags.HasErrors)
                return new EvaluationResult(null, diags); //just return the error, not the value
            return new EvaluationResult(eval.Evaluate(), diags);
        }

        private BoundStatement GetStatement()
        {
            BoundStatement result = GlobalScope.statement;
            return Lowerer.Lower(result);
        }

        private static void PrettyPrintTokenStream(LanguageToken[] tokens)
        {
            int currLineLength = 0;
            int terminalWidth = Console.WindowWidth;
            foreach (LanguageToken token in tokens)
            {
                string tokenStr = token.ToString() + " ";
                currLineLength += tokenStr.Length;
                if (currLineLength >= terminalWidth)
                {
                    Console.WriteLine();
                    currLineLength = tokenStr.Length;
                }
                
                Console.Write(tokenStr);
            }
        }

        private static void PrettyPrintParsedTree(ParseNode node, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";
            Console.Write(indent);
            Console.Write(marker);
            Console.WriteLine(node.ToString());

            indent += isLast ? "   " : "│  ";
            ParseNode lastChild = node.GetChildren().LastOrDefault();
            foreach (ParseNode child in node.GetChildren())
            {
                PrettyPrintParsedTree(child, indent, child == lastChild);
            }
        }

        private static void PrettyPrintBoundTree(BoundNode node, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(indent);
            Console.Write(marker);
            Console.ForegroundColor = GetNodeTypeColor(node.nodeType);
            Console.WriteLine(node.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;

            indent += isLast ? "   " : "│  ";
            BoundNode lastChild = node.GetChildren().LastOrDefault();
            foreach (BoundNode child in node.GetChildren())
            {
                PrettyPrintBoundTree(child, indent, child == lastChild);
            }
        }

        private static ConsoleColor GetNodeTypeColor(BoundNodeType type)
        {
            switch (type)
            {
                case BoundNodeType.AssignmentExpression:
                case BoundNodeType.VariableExpression:
                    return ConsoleColor.Blue;
                case BoundNodeType.VariableDeclarationStatement:
                    return ConsoleColor.DarkBlue;
                case BoundNodeType.LiteralExpression:
                    return ConsoleColor.Green;
                case BoundNodeType.UnaryExpression:
                case BoundNodeType.BinaryExpression:
                    return ConsoleColor.Yellow;
                case BoundNodeType.IfStatement:
                case BoundNodeType.WhileStatement:
                case BoundNodeType.ForStatement:
                    return ConsoleColor.DarkMagenta;
                default:
                    return ConsoleColor.Gray;
            }
        }
    }
}