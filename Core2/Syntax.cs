
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
        public required Token VariableName { get; init; }
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
        public List<(Token Operator, AST_Expr_And Right)> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprOr(this);
        public override List<ASTNode> Children => [Left, .. Rights.Select(r => r.Right)];
    }

    public class AST_Expr_And : AST_Expression
    {
        public required AST_Expr_Equality Left { get; init; }
        public List<(Token Operator, AST_Expr_Equality Right)> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprAnd(this);
        public override List<ASTNode> Children => [Left, .. Rights.Select(r => r.Right)];
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
        T VisitProgram(AST_Program node);
        T VisitImport(AST_Import node);
        T VisitLabelBlock(AST_LabelBlock node);
        T VisitDialogue(AST_Dialogue node);
        T VisitMenu(AST_Menu node);
        T VisitMenuItem(AST_MenuItem node);
        T VisitJump(AST_Jump node);
        T VisitTour(AST_Tour node);
        T VisitCall(AST_Call node);
        T VisitAssign(AST_Assign node);
        T VisitIf(AST_If node);

        T VisitExprOr(AST_Expr_Or node);
        T VisitExprAnd(AST_Expr_And node);
        T VisitExprEquality(AST_Expr_Equality node);
        T VisitExprComparison(AST_Expr_Comparison node);
        T VisitExprAdditive(AST_Expr_Additive node);
        T VisitExprMultiplicative(AST_Expr_Multiplicative node);
        T VisitExprPower(AST_Expr_Power node);
        T VisitExprUnary(AST_Expr_Unary node);

        T VisitLiteral(AST_Literal node);
        T VisitFString(AST_FString node);
        T VisitEmbedCall(AST_EmbedCall node);
        T VisitEmbedExpr(AST_EmbedExpr node);
    }

    public abstract class SyntaxBaseVisitor<T> : IASTVisitor<T>
    {
        public T Visit(ASTNode node)
        {
            return node switch
            {
                AST_Program n => VisitProgram(n),
                AST_Import n => VisitImport(n),
                AST_LabelBlock n => VisitLabelBlock(n),
                AST_Dialogue n => VisitDialogue(n),
                AST_Menu n => VisitMenu(n),
                AST_MenuItem n => VisitMenuItem(n),
                AST_Jump n => VisitJump(n),
                AST_Tour n => VisitTour(n),
                AST_Call n => VisitCall(n),
                AST_Assign n => VisitAssign(n),
                AST_If n => VisitIf(n),
                AST_Expr_Or n => VisitExprOr(n),
                AST_Expr_And n => VisitExprAnd(n),
                AST_Expr_Equality n => VisitExprEquality(n),
                AST_Expr_Comparison n => VisitExprComparison(n),
                AST_Expr_Additive n => VisitExprAdditive(n),
                AST_Expr_Multiplicative n => VisitExprMultiplicative(n),
                AST_Expr_Power n => VisitExprPower(n),
                AST_Expr_Unary n => VisitExprUnary(n),
                AST_Literal n => VisitLiteral(n),
                AST_FString n => VisitFString(n),
                AST_EmbedCall n => VisitEmbedCall(n),
                AST_EmbedExpr n => VisitEmbedExpr(n),
                _ => throw new NotImplementedException($"No visit method for {node.GetType().Name}")
            };
        }
        protected void VisitAllChildren(ASTNode node)
        {
            foreach (var child in node.Children)
            {
                Visit(child);
            }
        }

        public virtual T VisitProgram(AST_Program node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitImport(AST_Import node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitLabelBlock(AST_LabelBlock node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitDialogue(AST_Dialogue node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitMenu(AST_Menu node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitMenuItem(AST_MenuItem node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitJump(AST_Jump node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitTour(AST_Tour node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitCall(AST_Call node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitAssign(AST_Assign node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitIf(AST_If node)
        {
            VisitAllChildren(node);
            return default!;
        }

        public virtual T VisitExprOr(AST_Expr_Or node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitExprAnd(AST_Expr_And node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitExprEquality(AST_Expr_Equality node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitExprComparison(AST_Expr_Comparison node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitExprAdditive(AST_Expr_Additive node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitExprMultiplicative(AST_Expr_Multiplicative node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitExprPower(AST_Expr_Power node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitExprUnary(AST_Expr_Unary node)
        {
            VisitAllChildren(node);
            return default!;
        }

        public virtual T VisitLiteral(AST_Literal node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitFString(AST_FString node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitEmbedCall(AST_EmbedCall node)
        {
            VisitAllChildren(node);
            return default!;
        }
        public virtual T VisitEmbedExpr(AST_EmbedExpr node)
        {
            VisitAllChildren(node);
            return default!;
        }
    }
}