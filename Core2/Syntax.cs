
namespace Narratoria.Core
{
    public abstract class ASTNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public abstract T Accept<T>(IASTVisitor<T> visitor);
    }

    public class ASTRoot : ASTNode
    {
        public List<AST_Import> Imports { get; } = [];
        public List<AST_Statement> TopLevelStatements { get; } = [];
        public List<AST_LabelBlock> Labels { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitProgram(this);
    }

    public class AST_Import : ASTNode
    {
        public required Token Path { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitImport(this);
    }

    public class AST_LabelBlock : ASTNode
    {
        public required Token LabelName { get; init; }
        public List<AST_Statement> Statements { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitLabelBlock(this);
    }

    public abstract class AST_Statement : ASTNode { }

    public class AST_Dialogue : AST_Statement
    {
        public Token? Speaker { get; init; }
        public required AST_FString Text { get; init; }
        // public List<string> Tags { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitDialogue(this);
    }

    public class AST_Menu : AST_Statement
    {
        public List<AST_MenuItem> Items { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitMenu(this);
    }

    public class AST_MenuItem : ASTNode
    {
        public required AST_FString Text { get; init; }
        public List<AST_Statement> Body { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitMenuItem(this);
    }

    public class AST_Jump : AST_Statement
    {
        public required Token TargetLabel { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitJump(this);
    }

    public class AST_Tour : AST_Statement
    {
        public required Token TargetLabel { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitTour(this);
    }

    public class AST_Call : AST_Statement
    {
        public required Token FunctionName { get; init; }
        public List<AST_Expression> Arguments { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitCall(this);
    }

    public class AST_Assign : AST_Statement
    {
        public required Token VariableName { get; init; }
        public required Token Operator { get; init; }
        public required AST_Expression Value { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitAssign(this);
    }

    public class AST_If : AST_Statement
    {
        public required AST_Expression Condition { get; init; }
        public List<AST_Statement> ThenBlock { get; } = [];
        public List<AST_Statement>? ElseBlock { get; set; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitIf(this);
    }

    public abstract class AST_Expression : ASTNode { }

    public class AST_Expr_Or : AST_Expression
    {
        public required AST_Expr_And Left { get; init; }
        public List<(Token Operator, AST_Expr_And Right)> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprOr(this);
    }

    public class AST_Expr_And : AST_Expression
    {
        public required AST_Expr_Equality Left { get; init; }
        public List<(Token Operator, AST_Expr_Equality Right)> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprAnd(this);
    }

    public class AST_Expr_Equality : AST_Expression
    {
        public required AST_Expr_Comparison Left { get; init; }
        public required Token? Operator { get; init; }
        public required AST_Expr_Comparison? Right { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprEquality(this);
    }

    public class AST_Expr_Comparison : AST_Expression
    {
        public required AST_Expr_Additive Left { get; init; }
        public required Token? Operator { get; init; }
        public required AST_Expr_Additive? Right { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprComparison(this);
    }

    public class AST_Expr_Additive : AST_Expression
    {
        public required AST_Expr_Multiplicative Left { get; init; }
        public List<(Token Operator, AST_Expr_Multiplicative Right)> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprAdditive(this);
    }

    public class AST_Expr_Multiplicative : AST_Expression
    {
        public required AST_Expr_Power Left { get; init; }
        public List<(Token Operator, AST_Expr_Power Right)> Rights { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprMultiplicative(this);
    }

    public class AST_Expr_Power : AST_Expression
    {
        public required AST_Expr_Unary Base { get; init; }
        public List<AST_Expr_Unary> Exponents { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprPower(this);
    }

    public class AST_Expr_Unary : AST_Expression
    {
        public required Token? Operator { get; init; }
        public required AST_Expr_Primary Primary { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitExprUnary(this);
    }

    public abstract class AST_Expr_Primary : AST_Expression { }

    public class AST_Literal : AST_Expr_Primary
    {
        public required Token Value { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitLiteral(this);
    }

    public class AST_FString : AST_Expr_Primary
    {
        public List<Token> Fragments { get; } = [];
        public Queue<(AST_Expression Call, int Index)> Embedded { get; } = [];
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitFString(this);
    }

    public class AST_EmbedCall : AST_Expr_Primary
    {
        public required AST_Call Call { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitEmbedCall(this);
    }

    public class AST_EmbedExpr : AST_Expr_Primary
    {
        public required AST_Expression Expression { get; init; }
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.VisitEmbedExpr(this);
    }

    // =========================== Visitor ===========================

    public interface IASTVisitor<T>
    {
        T VisitProgram(ASTRoot node);
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

    public abstract class SyntaxVisitorBase<T> : IASTVisitor<T>
    {
        public virtual T Visit(ASTNode node) => node.Accept(this);
        public virtual T VisitProgram(ASTRoot node) => default!;
        public virtual T VisitImport(AST_Import node) => default!;
        public virtual T VisitLabelBlock(AST_LabelBlock node) => default!;
        public virtual T VisitDialogue(AST_Dialogue node) => default!;
        public virtual T VisitMenu(AST_Menu node) => default!;
        public virtual T VisitMenuItem(AST_MenuItem node) => default!;
        public virtual T VisitJump(AST_Jump node) => default!;
        public virtual T VisitTour(AST_Tour node) => default!;
        public virtual T VisitCall(AST_Call node) => default!;
        public virtual T VisitAssign(AST_Assign node) => default!;
        public virtual T VisitIf(AST_If node) => default!;

        public virtual T VisitExprOr(AST_Expr_Or node) => default!;
        public virtual T VisitExprAnd(AST_Expr_And node) => default!;
        public virtual T VisitExprEquality(AST_Expr_Equality node) => default!;
        public virtual T VisitExprComparison(AST_Expr_Comparison node) => default!;
        public virtual T VisitExprAdditive(AST_Expr_Additive node) => default!;
        public virtual T VisitExprMultiplicative(AST_Expr_Multiplicative node) => default!;
        public virtual T VisitExprPower(AST_Expr_Power node) => default!;
        public virtual T VisitExprUnary(AST_Expr_Unary node) => default!;

        public virtual T VisitLiteral(AST_Literal node) => default!;
        public virtual T VisitFString(AST_FString node) => default!;
        public virtual T VisitEmbedCall(AST_EmbedCall node) => default!;
        public virtual T VisitEmbedExpr(AST_EmbedExpr node) => default!;
    }

    // =========================== Syntax Definitions ===========================

    internal abstract class SyntaxElement { }

    internal class SyntaxSequence : SyntaxElement
    {
        public List<SyntaxElement> Elements { get; } = [];
        public SyntaxSequence(params SyntaxElement[] elements) => Elements = [.. elements];
    }

    internal class SyntaxOr : SyntaxElement
    {
        public List<SyntaxElement> Options { get; } = [];
        public SyntaxOr(params SyntaxElement[] options) => Options = [.. options];
    }

    internal class SyntaxOptional : SyntaxElement // ? zero or one
    {
        public SyntaxElement Element { get; }
        public SyntaxOptional(SyntaxElement element) => Element = element;
    }

    internal class SyntaxStar : SyntaxElement // * zero or more
    {
        public SyntaxElement Element { get; }
        public SyntaxStar(SyntaxElement element) => Element = element;
    }

    internal class SyntaxPlus : SyntaxElement // + one or more
    {
        public SyntaxElement Element { get; }
        public SyntaxPlus(SyntaxElement element) => Element = element;
    }

    internal class SyntaxTokenType : SyntaxElement
    {
        public TokenType Type { get; }
        public Action<ASTNode, Token>? Setter { get; init; }
        public SyntaxTokenType(TokenType type, Action<ASTNode, Token>? setter = null)
        {
            Type = type;
            Setter = setter;
        }
    }

    internal class SyntaxNodeElement : SyntaxElement
    {
        public Type NodeType { get; }
        public Action<ASTNode, ASTNode>? Setter { get; init; }
        public SyntaxNodeElement(Type nodeType, Action<ASTNode, ASTNode>? setter = null)
        {
            NodeType = nodeType;
            Setter = setter;
        }
    }
}
