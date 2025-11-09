
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
        public required string Path { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitImport(this);
    }

    public class SyntaxLabelBlock : SyntaxNode
    {
        public required string LabelName { get; init; }
        public List<SyntaxStatement> Statements { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitLabelBlock(this);
    }

    public abstract class SyntaxStatement : SyntaxNode { }

    public class SyntaxDialogue : SyntaxStatement
    {
        public string? Speaker { get; init; }
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
        public required string TargetLabel { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitJump(this);
    }

    public class SyntaxTour : SyntaxStatement
    {
        public required string TargetLabel { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitTour(this);
    }

    public class SyntaxCall : SyntaxStatement
    {
        public required string FunctionName { get; init; }
        public List<SyntaxExpression> Arguments { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitCall(this);
    }

    public class SyntaxAssign : SyntaxStatement
    {
        public required string VariableName { get; init; }
        public required string Operator { get; init; }
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

    public class SyntaxBinaryExpr : SyntaxExpression
    {
        public required SyntaxExpression Left { get; init; }
        public required string Operator { get; init; }
        public required SyntaxExpression Right { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
    }

    public class SyntaxUnaryExpr : SyntaxExpression
    {
        public required string Operator { get; init; }
        public required SyntaxExpression Operand { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
    }

    public class SyntaxLiteral : SyntaxExpression
    {
        public required string Value { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitLiteral(this);
    }

    public class SyntaxVariable : SyntaxExpression
    {
        public required string Name { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitVariable(this);
    }

    public class SyntaxFString : SyntaxExpression
    {
        public List<string> Fragments { get; } = [];
        public List<SyntaxExpression> Embedded { get; } = [];
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitFString(this);
    }

    public class SyntaxEmbedCall : SyntaxExpression
    {
        public required SyntaxCall Call { get; init; }
        public override T Accept<T>(ISyntaxVisitor<T> visitor) => visitor.VisitFunctionCall(this);
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

        T VisitBinaryExpr(SyntaxBinaryExpr node);
        T VisitUnaryExpr(SyntaxUnaryExpr node);
        T VisitLiteral(SyntaxLiteral node);
        T VisitVariable(SyntaxVariable node);
        T VisitFString(SyntaxFString node);
        T VisitFunctionCall(SyntaxEmbedCall node);
    }
}
