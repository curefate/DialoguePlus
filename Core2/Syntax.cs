
namespace Narratoria.Core
{
    public abstract class SyntaxNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public abstract T Accept<T>(ISyntaxVisitor<T> visitor);
    }

    public class SyntaxProgram : SyntaxNode
    {
        public List<SyntaxImport> Imports { get; } = [];
        public List<SyntaxStatement> TopLevelStatements { get; } = [];
        public List<SyntaxLabelBlock> Labels { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitProgram(this);
    }

    public class SyntaxImport : SyntaxNode
    {
        public required Token Path { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitImport(this);
    }

    public class SyntaxLabelBlock : SyntaxNode
    {
        public required Token LabelName { get; init; }
        public List<SyntaxStatement> Statements { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitLabelBlock(this);
    }

    public abstract class SyntaxStatement : SyntaxNode { }

    public class SyntaxDialogue : SyntaxStatement
    {
        public Token? Speaker { get; init; }
        public required SyntaxFString Text { get; init; }
        // public List<string> Tags { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitDialogue(this);
    }

    public class SyntaxMenu : SyntaxStatement
    {
        public List<SyntaxMenuItem> Items { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitMenu(this);
    }

    public class SyntaxMenuItem : SyntaxNode
    {
        public required SyntaxFString Text { get; init; }
        public List<SyntaxStatement> Body { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitMenuItem(this);
    }

    public class SyntaxJump : SyntaxStatement
    {
        public required Token TargetLabel { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitJump(this);
    }

    public class SyntaxTour : SyntaxStatement
    {
        public required Token TargetLabel { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitTour(this);
    }

    public class SyntaxCall : SyntaxStatement
    {
        public required Token FunctionName { get; init; }
        public List<SyntaxExpression> Arguments { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitCall(this);
    }

    public class SyntaxAssign : SyntaxStatement
    {
        public required Token VariableName { get; init; }
        public required Token Operator { get; init; }
        public required SyntaxExpression Value { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitAssign(this);
    }

    public class SyntaxIf : SyntaxStatement
    {
        public required SyntaxExpression Condition { get; init; }
        public List<SyntaxStatement> ThenBlock { get; } = [];
        public List<SyntaxStatement>? ElseBlock { get; set; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitIf(this);
    }

    public abstract class SyntaxExpression : SyntaxNode { }

    public class SyntaxExprOr : SyntaxExpression
    {
        public required SyntaxExprAnd Left { get; init; }
        public List<(Token Operator, SyntaxExprAnd Right)> Rights { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitExprOr(this);
    }

    public class SyntaxExprAnd : SyntaxExpression
    {
        public required SyntaxExprEquality Left { get; init; }
        public List<(Token Operator, SyntaxExprEquality Right)> Rights { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitExprAnd(this);
    }

    public class SyntaxExprEquality : SyntaxExpression
    {
        public required SyntaxExprComparison Left { get; init; }
        public required Token? Operator { get; init; }
        public required SyntaxExprComparison? Right { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitExprEquality(this);
    }

    public class SyntaxExprComparison : SyntaxExpression
    {
        public required SyntaxExprAdditive Left { get; init; }
        public required Token? Operator { get; init; }
        public required SyntaxExprAdditive? Right { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitExprComparison(this);
    }

    public class SyntaxExprAdditive : SyntaxExpression
    {
        public required SyntaxExprMultiplicative Left { get; init; }
        public List<(Token Operator, SyntaxExprMultiplicative Right)> Rights { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitExprAdditive(this);
    }

    public class SyntaxExprMultiplicative : SyntaxExpression
    {
        public required SyntaxExprPower Left { get; init; }
        public List<(Token Operator, SyntaxExprPower Right)> Rights { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitExprMultiplicative(this);
    }

    public class SyntaxExprPower : SyntaxExpression
    {
        public required SyntaxExprUnary Base { get; init; }
        public List<SyntaxExprUnary> Exponents { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitExprPower(this);
    }

    public class SyntaxExprUnary : SyntaxExpression
    {
        public required Token? Operator { get; init; }
        public required SyntaxExprPrimary Primary { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitExprUnary(this);
    }

    public abstract class SyntaxExprPrimary : SyntaxExpression { }

    public class SyntaxLiteral : SyntaxExprPrimary
    {
        public required Token Value { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitLiteral(this);
    }

    public class SyntaxFString : SyntaxExprPrimary
    {
        public List<Token> Fragments { get; } = [];
        public Queue<(SyntaxExpression Call, int Index)> Embedded { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitFString(this);
    }

    public class SyntaxEmbedCall : SyntaxExprPrimary
    {
        public required SyntaxCall Call { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitEmbedCall(this);
    }

    public class SyntaxEmbedExpr : SyntaxExprPrimary
    {
        public required SyntaxExpression Expression { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitEmbedExpr(this);
    }

    // ========== Visitor Interface ==========

    public interface ISyntaxVisitor<T>
    {
        T VisitProgram(SyntaxProgram node);
        T VisitImport(SyntaxImport node);
        T VisitLabelBlock(SyntaxLabelBlock node);
        T VisitDialogue(SyntaxDialogue node);
        T VisitMenu(SyntaxMenu node);
        T VisitMenuItem(SyntaxMenuItem node);
        T VisitJump(SyntaxJump node);
        T VisitTour(SyntaxTour node);
        T VisitCall(SyntaxCall node);
        T VisitAssign(SyntaxAssign node);
        T VisitIf(SyntaxIf node);

        T VisitExprOr(SyntaxExprOr node);
        T VisitExprAnd(SyntaxExprAnd node);
        T VisitExprEquality(SyntaxExprEquality node);
        T VisitExprComparison(SyntaxExprComparison node);
        T VisitExprAdditive(SyntaxExprAdditive node);
        T VisitExprMultiplicative(SyntaxExprMultiplicative node);
        T VisitExprPower(SyntaxExprPower node);
        T VisitExprUnary(SyntaxExprUnary node);

        T VisitLiteral(SyntaxLiteral node);
        T VisitFString(SyntaxFString node);
        T VisitEmbedCall(SyntaxEmbedCall node);
        T VisitEmbedExpr(SyntaxEmbedExpr node);
    }
}
