// Generated from c:/Users/curef/Desktop/DS/DS/Grammar/DSParser.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link DSParser}.
 */
public interface DSParserListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link DSParser#program}.
	 * @param ctx the parse tree
	 */
	void enterProgram(DSParser.ProgramContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#program}.
	 * @param ctx the parse tree
	 */
	void exitProgram(DSParser.ProgramContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#label_block}.
	 * @param ctx the parse tree
	 */
	void enterLabel_block(DSParser.Label_blockContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#label_block}.
	 * @param ctx the parse tree
	 */
	void exitLabel_block(DSParser.Label_blockContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterStatement(DSParser.StatementContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitStatement(DSParser.StatementContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#import_stmt}.
	 * @param ctx the parse tree
	 */
	void enterImport_stmt(DSParser.Import_stmtContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#import_stmt}.
	 * @param ctx the parse tree
	 */
	void exitImport_stmt(DSParser.Import_stmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#dialogue_stmt}.
	 * @param ctx the parse tree
	 */
	void enterDialogue_stmt(DSParser.Dialogue_stmtContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#dialogue_stmt}.
	 * @param ctx the parse tree
	 */
	void exitDialogue_stmt(DSParser.Dialogue_stmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#menu_stmt}.
	 * @param ctx the parse tree
	 */
	void enterMenu_stmt(DSParser.Menu_stmtContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#menu_stmt}.
	 * @param ctx the parse tree
	 */
	void exitMenu_stmt(DSParser.Menu_stmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#menu_item}.
	 * @param ctx the parse tree
	 */
	void enterMenu_item(DSParser.Menu_itemContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#menu_item}.
	 * @param ctx the parse tree
	 */
	void exitMenu_item(DSParser.Menu_itemContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#jump_stmt}.
	 * @param ctx the parse tree
	 */
	void enterJump_stmt(DSParser.Jump_stmtContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#jump_stmt}.
	 * @param ctx the parse tree
	 */
	void exitJump_stmt(DSParser.Jump_stmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#tour_stmt}.
	 * @param ctx the parse tree
	 */
	void enterTour_stmt(DSParser.Tour_stmtContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#tour_stmt}.
	 * @param ctx the parse tree
	 */
	void exitTour_stmt(DSParser.Tour_stmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#call_stmt}.
	 * @param ctx the parse tree
	 */
	void enterCall_stmt(DSParser.Call_stmtContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#call_stmt}.
	 * @param ctx the parse tree
	 */
	void exitCall_stmt(DSParser.Call_stmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#assign_stmt}.
	 * @param ctx the parse tree
	 */
	void enterAssign_stmt(DSParser.Assign_stmtContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#assign_stmt}.
	 * @param ctx the parse tree
	 */
	void exitAssign_stmt(DSParser.Assign_stmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#if_stmt}.
	 * @param ctx the parse tree
	 */
	void enterIf_stmt(DSParser.If_stmtContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#if_stmt}.
	 * @param ctx the parse tree
	 */
	void exitIf_stmt(DSParser.If_stmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterExpression(DSParser.ExpressionContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitExpression(DSParser.ExpressionContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#expr_logical_and}.
	 * @param ctx the parse tree
	 */
	void enterExpr_logical_and(DSParser.Expr_logical_andContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#expr_logical_and}.
	 * @param ctx the parse tree
	 */
	void exitExpr_logical_and(DSParser.Expr_logical_andContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#expr_equality}.
	 * @param ctx the parse tree
	 */
	void enterExpr_equality(DSParser.Expr_equalityContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#expr_equality}.
	 * @param ctx the parse tree
	 */
	void exitExpr_equality(DSParser.Expr_equalityContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#expr_comparison}.
	 * @param ctx the parse tree
	 */
	void enterExpr_comparison(DSParser.Expr_comparisonContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#expr_comparison}.
	 * @param ctx the parse tree
	 */
	void exitExpr_comparison(DSParser.Expr_comparisonContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#expr_term}.
	 * @param ctx the parse tree
	 */
	void enterExpr_term(DSParser.Expr_termContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#expr_term}.
	 * @param ctx the parse tree
	 */
	void exitExpr_term(DSParser.Expr_termContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#expr_factor}.
	 * @param ctx the parse tree
	 */
	void enterExpr_factor(DSParser.Expr_factorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#expr_factor}.
	 * @param ctx the parse tree
	 */
	void exitExpr_factor(DSParser.Expr_factorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#expr_unary}.
	 * @param ctx the parse tree
	 */
	void enterExpr_unary(DSParser.Expr_unaryContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#expr_unary}.
	 * @param ctx the parse tree
	 */
	void exitExpr_unary(DSParser.Expr_unaryContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#expr_primary}.
	 * @param ctx the parse tree
	 */
	void enterExpr_primary(DSParser.Expr_primaryContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#expr_primary}.
	 * @param ctx the parse tree
	 */
	void exitExpr_primary(DSParser.Expr_primaryContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#embedded_expr}.
	 * @param ctx the parse tree
	 */
	void enterEmbedded_expr(DSParser.Embedded_exprContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#embedded_expr}.
	 * @param ctx the parse tree
	 */
	void exitEmbedded_expr(DSParser.Embedded_exprContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#embedded_call}.
	 * @param ctx the parse tree
	 */
	void enterEmbedded_call(DSParser.Embedded_callContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#embedded_call}.
	 * @param ctx the parse tree
	 */
	void exitEmbedded_call(DSParser.Embedded_callContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#block}.
	 * @param ctx the parse tree
	 */
	void enterBlock(DSParser.BlockContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#block}.
	 * @param ctx the parse tree
	 */
	void exitBlock(DSParser.BlockContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#fstring}.
	 * @param ctx the parse tree
	 */
	void enterFstring(DSParser.FstringContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#fstring}.
	 * @param ctx the parse tree
	 */
	void exitFstring(DSParser.FstringContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#string_fragment}.
	 * @param ctx the parse tree
	 */
	void enterString_fragment(DSParser.String_fragmentContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#string_fragment}.
	 * @param ctx the parse tree
	 */
	void exitString_fragment(DSParser.String_fragmentContext ctx);
	/**
	 * Enter a parse tree produced by {@link DSParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition(DSParser.ConditionContext ctx);
	/**
	 * Exit a parse tree produced by {@link DSParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition(DSParser.ConditionContext ctx);
}