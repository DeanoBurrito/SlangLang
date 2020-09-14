using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SlangLang.Debug;
using SlangLang.Binding;

namespace SlangLang.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int labelCount = 0;
        
        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            Lowerer lowerer = new Lowerer();
            BoundStatement result = lowerer.RewriteStatement(statement);
            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            ImmutableArray<BoundStatement>.Builder builder = ImmutableArray.CreateBuilder<BoundStatement>();
            Stack<BoundStatement> stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while (stack.Count > 0)
            {
                BoundStatement current = stack.Pop();

                if (current is BoundBlockStatement block)
                {
                    foreach (BoundStatement s in block.statements.Reverse())
                    {
                        stack.Push(s);
                    }
                }
                else
                {
                    builder.Add(current);
                }
            }

            return new BoundBlockStatement(builder.ToImmutable(), TextSpan.NoText);
        }
        
        private Lowerer()
        {

        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.elseStatement == null)
            {
                LabelSymbol endLabel = GenerateLabel();

                BoundConditionalGoto gotoFalse = new BoundConditionalGoto(endLabel, node.condition, TextSpan.NoText, false);
                BoundLabelStatement endLabelStatement = new BoundLabelStatement(endLabel, TextSpan.NoText);

                BoundBlockStatement result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                        gotoFalse, 
                        node.body, 
                        endLabelStatement), 
                    node.textLocation);
                
                return RewriteStatement(result);
            }
            else
            {
                LabelSymbol endLabel = GenerateLabel();
                LabelSymbol elseLabel = GenerateLabel();

                BoundConditionalGoto gotoElseFalse = new BoundConditionalGoto(elseLabel, node.condition, TextSpan.NoText, false);
                BoundGotoStatement gotoEnd = new BoundGotoStatement(endLabel, TextSpan.NoText);
                BoundLabelStatement boundElseLabel = new BoundLabelStatement(elseLabel, TextSpan.NoText);
                BoundLabelStatement boundEndLabel = new BoundLabelStatement(endLabel, TextSpan.NoText);

                BoundBlockStatement result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(   
                        gotoElseFalse,
                        node.body,
                        gotoEnd,
                        boundElseLabel,
                        node.elseStatement,
                        boundEndLabel), 
                    node.textLocation);
                
                return RewriteStatement(result);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            LabelSymbol topLabel = GenerateLabel();
            LabelSymbol endLabel = GenerateLabel();
            
            BoundConditionalGoto gotoEndFalse = new BoundConditionalGoto(endLabel, node.condition, TextSpan.NoText, false);
            BoundGotoStatement gotoTop = new BoundGotoStatement(topLabel, TextSpan.NoText);
            BoundLabelStatement boundTopLabel = new BoundLabelStatement(topLabel, TextSpan.NoText);
            BoundLabelStatement boundEndLabel = new BoundLabelStatement(endLabel, TextSpan.NoText);

            BoundBlockStatement result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    boundTopLabel,
                    gotoEndFalse,
                    node.body,
                    gotoTop,
                    boundEndLabel), 
                node.textLocation);
            
            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            BoundBlockStatement newBody = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.body, node.postStatement), node.textLocation);
            BoundWhileStatement whileStatement = new BoundWhileStatement(node.condition, newBody, node.textLocation);
            BoundBlockStatement result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.setupStatement, whileStatement), node.textLocation);

            return RewriteStatement(result);
        }

        private LabelSymbol GenerateLabel()
        {
            labelCount++;
            return new LabelSymbol("GeneratedLabel_" + labelCount);
        }
    }
}