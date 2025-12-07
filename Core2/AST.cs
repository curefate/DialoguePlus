
namespace Narratoria.Core
{
    public abstract class ASTNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public abstract T Accept<T>(IASTVisitor<T> visitor);
        public abstract List<ASTNode> Children { get; }
    }

    public class AST_Program : ASTNode
    {
        public List<AST_Import> Imports { get; } = [];
        public List<AST_Statement> TopLevelStatements { get; } = [];
        public List<AST_LabelBlock> Labels { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitProgram(this);
        public override List<ASTNode> Children => [.. Imports, .. TopLevelStatements, .. Labels];
    }

    public class AST_Import : ASTNode
    {
        public required Token Path { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitImport(this);
        public override List<ASTNode> Children => [];
    }

    public class AST_LabelBlock : ASTNode
    {
        public required Token LabelName { get; init; }
        public List<AST_Statement> Statements { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitLabelBlock(this);
        public override List<ASTNode> Children => [.. Statements];
    }

    public abstract class AST_Statement : ASTNode { }

    public class AST_Dialogue : AST_Statement
    {
        public Token? Speaker { get; init; }
        public required AST_FString Text { get; init; }
        // public List<string> Tags { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitDialogue(this);
        public override List<ASTNode> Children => [Text];
    }

    public class AST_Menu : AST_Statement
    {
        public List<AST_MenuItem> Items { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitMenu(this);
        public override List<ASTNode> Children => [.. Items];
    }

    public class AST_MenuItem : ASTNode
    {
        public required AST_FString Text { get; init; }
        public List<AST_Statement> Body { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitMenuItem(this);
        public override List<ASTNode> Children => [Text, .. Body];
    }

    public class AST_Jump : AST_Statement
    {
        public required Token TargetLabel { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitJump(this);
        public override List<ASTNode> Children => [];
    }

    public class AST_Tour : AST_Statement
    {
        public required Token TargetLabel { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitTour(this);
        public override List<ASTNode> Children => [];
    }

    public class AST_Call : AST_Statement
    {
        public required Token FunctionName { get; init; }
        public List<AST_Expression> Arguments { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitCall(this);
        public override List<ASTNode> Children => [.. Arguments];
    }

    public class AST_Assign : AST_Statement
    {
        public required Token Variable { get; init; }
        public required Token Operator { get; init; }
        public required AST_Expression Value { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitAssign(this);
        public override List<ASTNode> Children => [Value];
    }

    public class AST_If : AST_Statement
    {
        public required AST_Expression Condition { get; init; }
        public List<AST_Statement> ThenBlock { get; } = [];
        public List<AST_Statement>? ElseBlock { get; set; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitIf(this);
        public override List<ASTNode> Children => ElseBlock != null ? [Condition, .. ThenBlock, .. ElseBlock!] : [Condition, .. ThenBlock];
    }

    public abstract class AST_Expression : ASTNode { }

    public class AST_Expr_Or : AST_Expression
    {
        public required AST_Expr_And Left { get; init; }
        public List<AST_Expr_And> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprOr(this);
        public override List<ASTNode> Children => [Left, .. Rights];
    }

    public class AST_Expr_And : AST_Expression
    {
        public required AST_Expr_Equality Left { get; init; }
        public List<AST_Expr_Equality> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprAnd(this);
        public override List<ASTNode> Children => [Left, .. Rights];
    }

    public class AST_Expr_Equality : AST_Expression
    {
        public required AST_Expr_Comparison Left { get; init; }
        public required Token? Operator { get; init; }
        public required AST_Expr_Comparison? Right { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprEquality(this);
        public override List<ASTNode> Children => Right != null ? [Left, Right] : [Left];
    }

    public class AST_Expr_Comparison : AST_Expression
    {
        public required AST_Expr_Additive Left { get; init; }
        public required Token? Operator { get; init; }
        public required AST_Expr_Additive? Right { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprComparison(this);
        public override List<ASTNode> Children => Right != null ? [Left, Right] : [Left];
    }

    public class AST_Expr_Additive : AST_Expression
    {
        public required AST_Expr_Multiplicative Left { get; init; }
        public List<(Token Operator, AST_Expr_Multiplicative Right)> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprAdditive(this);
        public override List<ASTNode> Children => [Left, .. Rights.Select(r => r.Right)];
    }

    public class AST_Expr_Multiplicative : AST_Expression
    {
        public required AST_Expr_Power Left { get; init; }
        public List<(Token Operator, AST_Expr_Power Right)> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprMultiplicative(this);
        public override List<ASTNode> Children => [Left, .. Rights.Select(r => r.Right)];
    }

    public class AST_Expr_Power : AST_Expression
    {
        public required AST_Expr_Unary Base { get; init; }
        public List<AST_Expr_Unary> Exponents { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprPower(this);
        public override List<ASTNode> Children => [Base, .. Exponents];
    }

    public class AST_Expr_Unary : AST_Expression
    {
        public required Token? Operator { get; init; }
        public required AST_Expr_Primary Primary { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprUnary(this);
        public override List<ASTNode> Children => [Primary];
    }

    public abstract class AST_Expr_Primary : AST_Expression { }

    public class AST_Literal : AST_Expr_Primary
    {
        public required Token Value { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitLiteral(this);
        public override List<ASTNode> Children => [];
    }

    public class AST_FString : AST_Expr_Primary
    {
        public List<Token> Fragments { get; } = [];
        public List<AST_Expression> Embeds { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitFString(this);
        public override List<ASTNode> Children => [.. Embeds];
    }

    public class AST_EmbedCall : AST_Expr_Primary
    {
        public required AST_Call Call { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitEmbedCall(this);
        public override List<ASTNode> Children => [Call];
    }

    public class AST_EmbedExpr : AST_Expr_Primary
    {
        public required AST_Expression Expression { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitEmbedExpr(this);
        public override List<ASTNode> Children => [Expression];
    }

    // =========================== Visitor ===========================

    public interface IASTVisitor<T>
    {
        T VisitProgram(AST_Program context);
        T VisitImport(AST_Import context);
        T VisitLabelBlock(AST_LabelBlock context);

        T VisitDialogue(AST_Dialogue context);
        T VisitMenu(AST_Menu context);
        T VisitMenuItem(AST_MenuItem context);
        T VisitJump(AST_Jump context);
        T VisitTour(AST_Tour context);
        T VisitCall(AST_Call context);
        T VisitAssign(AST_Assign context);
        T VisitIf(AST_If context);

        T VisitExprOr(AST_Expr_Or context);
        T VisitExprAnd(AST_Expr_And context);
        T VisitExprEquality(AST_Expr_Equality context);
        T VisitExprComparison(AST_Expr_Comparison context);
        T VisitExprAdditive(AST_Expr_Additive context);
        T VisitExprMultiplicative(AST_Expr_Multiplicative context);
        T VisitExprPower(AST_Expr_Power context);
        T VisitExprUnary(AST_Expr_Unary context);

        T VisitLiteral(AST_Literal context);
        T VisitFString(AST_FString context);
        T VisitEmbedCall(AST_EmbedCall context);
        T VisitEmbedExpr(AST_EmbedExpr context);
    }

    public abstract class BaseVisitor<T> : IASTVisitor<T>, IDiagnosticReporter
    {
        protected readonly List<DiagnosticCollector> _collectors = [];
        public virtual void AddCollector(DiagnosticCollector collector)
        {
            _collectors.Add(collector);
        }
        public virtual void Report(Diagnostic diagnostic)
        {
            foreach (var collector in _collectors)
            {
                collector.Add(diagnostic);
            }
        }

        public T Visit(ASTNode context)
        {
            return context switch
            {
                AST_Program c => VisitProgram(c),
                AST_Import c => VisitImport(c),
                AST_LabelBlock c => VisitLabelBlock(c),
                AST_Dialogue c => VisitDialogue(c),
                AST_Menu c => VisitMenu(c),
                AST_MenuItem c => VisitMenuItem(c),
                AST_Jump c => VisitJump(c),
                AST_Tour c => VisitTour(c),
                AST_Call c => VisitCall(c),
                AST_Assign c => VisitAssign(c),
                AST_If c => VisitIf(c),
                AST_Expr_Or c => VisitExprOr(c),
                AST_Expr_And c => VisitExprAnd(c),
                AST_Expr_Equality c => VisitExprEquality(c),
                AST_Expr_Comparison c => VisitExprComparison(c),
                AST_Expr_Additive c => VisitExprAdditive(c),
                AST_Expr_Multiplicative c => VisitExprMultiplicative(c),
                AST_Expr_Power c => VisitExprPower(c),
                AST_Expr_Unary c => VisitExprUnary(c),
                AST_Literal c => VisitLiteral(c),
                AST_FString c => VisitFString(c),
                AST_EmbedCall c => VisitEmbedCall(c),
                AST_EmbedExpr c => VisitEmbedExpr(c),
                _ => throw new NotImplementedException($"No visit method for {context.GetType().Name}")
            };
        }
        protected void VisitAllChildren(ASTNode context)
        {
            foreach (var child in context.Children)
            {
                Visit(child);
            }
        }

        public virtual T VisitProgram(AST_Program context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitImport(AST_Import context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitLabelBlock(AST_LabelBlock context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitDialogue(AST_Dialogue context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitMenu(AST_Menu context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitMenuItem(AST_MenuItem context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitJump(AST_Jump context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitTour(AST_Tour context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitCall(AST_Call context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitAssign(AST_Assign context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitIf(AST_If context)
        {
            VisitAllChildren(context);
            return default!;
        }

        public virtual T VisitExprOr(AST_Expr_Or context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitExprAnd(AST_Expr_And context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitExprEquality(AST_Expr_Equality context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitExprComparison(AST_Expr_Comparison context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitExprAdditive(AST_Expr_Additive context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitExprMultiplicative(AST_Expr_Multiplicative context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitExprPower(AST_Expr_Power context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitExprUnary(AST_Expr_Unary context)
        {
            VisitAllChildren(context);
            return default!;
        }

        public virtual T VisitLiteral(AST_Literal context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitFString(AST_FString context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitEmbedCall(AST_EmbedCall context)
        {
            VisitAllChildren(context);
            return default!;
        }
        public virtual T VisitEmbedExpr(AST_EmbedExpr context)
        {
            VisitAllChildren(context);
            return default!;
        }
    }
}