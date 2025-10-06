// Generated from c:/Users/curef/Desktop/DS/DS/Grammar/DSParser.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue"})
public class DSParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.13.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		INDENT=1, DEDENT=2, NL=3, STRING_START=4, STRING_CONTEXT=5, STRING_ESCAPE=6, 
		STRING_END=7, PATH=8, LPAR=9, RPAR=10, LBRACE=11, RBRACE=12, EXCLAMATION=13, 
		PLUS=14, MINUS=15, STAR=16, SLASH=17, LESS=18, GREATER=19, EQUAL=20, PERCENT=21, 
		EQEQUAL=22, NOTEQUAL=23, LESSEQUAL=24, GREATEREQUAL=25, PLUSEQUAL=26, 
		MINEQUAL=27, STAREQUAL=28, SLASHEQUAL=29, PERCENTEQUAL=30, AND=31, OR=32, 
		COLON=33, COMMA=34, CALL=35, IF=36, NOT=37, ELIF=38, ELSE=39, JUMP=40, 
		TOUR=41, LABEL=42, IMPORT=43, BOOL=44, TRUE=45, FALSE=46, NUMBER=47, ID=48, 
		TAG=49, VARIABLE=50, WS=51, LINE_COMMENT=52, ERROR_CHAR=53, NEWLINE=54, 
		EMBED_WS=55, PATH_WS=56;
	public static final int
		RULE_program = 0, RULE_label_block = 1, RULE_statement = 2, RULE_import_stmt = 3, 
		RULE_dialogue_stmt = 4, RULE_menu_stmt = 5, RULE_menu_item = 6, RULE_jump_stmt = 7, 
		RULE_tour_stmt = 8, RULE_call_stmt = 9, RULE_assign_stmt = 10, RULE_if_stmt = 11, 
		RULE_expression = 12, RULE_expr_logical_and = 13, RULE_expr_equality = 14, 
		RULE_expr_comparison = 15, RULE_expr_term = 16, RULE_expr_factor = 17, 
		RULE_expr_unary = 18, RULE_expr_primary = 19, RULE_embedded_expr = 20, 
		RULE_embedded_call = 21, RULE_block = 22, RULE_fstring = 23, RULE_string_fragment = 24, 
		RULE_condition = 25;
	private static String[] makeRuleNames() {
		return new String[] {
			"program", "label_block", "statement", "import_stmt", "dialogue_stmt", 
			"menu_stmt", "menu_item", "jump_stmt", "tour_stmt", "call_stmt", "assign_stmt", 
			"if_stmt", "expression", "expr_logical_and", "expr_equality", "expr_comparison", 
			"expr_term", "expr_factor", "expr_unary", "expr_primary", "embedded_expr", 
			"embedded_call", "block", "fstring", "string_fragment", "condition"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, null, null, null, null, null, null, null, null, "'('", "')'", "'{'", 
			"'}'", "'!'", "'+'", "'-'", "'*'", "'/'", "'<'", "'>'", "'='", "'%'", 
			"'=='", "'!='", "'<='", "'>='", "'+='", "'-='", "'*='", "'/='", "'%='", 
			null, null, "':'", "','", "'call'", "'if'", "'not'", "'elif'", "'else'", 
			null, null, null, "'import'", null, "'true'", "'false'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, "INDENT", "DEDENT", "NL", "STRING_START", "STRING_CONTEXT", "STRING_ESCAPE", 
			"STRING_END", "PATH", "LPAR", "RPAR", "LBRACE", "RBRACE", "EXCLAMATION", 
			"PLUS", "MINUS", "STAR", "SLASH", "LESS", "GREATER", "EQUAL", "PERCENT", 
			"EQEQUAL", "NOTEQUAL", "LESSEQUAL", "GREATEREQUAL", "PLUSEQUAL", "MINEQUAL", 
			"STAREQUAL", "SLASHEQUAL", "PERCENTEQUAL", "AND", "OR", "COLON", "COMMA", 
			"CALL", "IF", "NOT", "ELIF", "ELSE", "JUMP", "TOUR", "LABEL", "IMPORT", 
			"BOOL", "TRUE", "FALSE", "NUMBER", "ID", "TAG", "VARIABLE", "WS", "LINE_COMMENT", 
			"ERROR_CHAR", "NEWLINE", "EMBED_WS", "PATH_WS"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "DSParser.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public DSParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ProgramContext extends ParserRuleContext {
		public TerminalNode EOF() { return getToken(DSParser.EOF, 0); }
		public List<Import_stmtContext> import_stmt() {
			return getRuleContexts(Import_stmtContext.class);
		}
		public Import_stmtContext import_stmt(int i) {
			return getRuleContext(Import_stmtContext.class,i);
		}
		public List<Label_blockContext> label_block() {
			return getRuleContexts(Label_blockContext.class);
		}
		public Label_blockContext label_block(int i) {
			return getRuleContext(Label_blockContext.class,i);
		}
		public ProgramContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_program; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterProgram(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitProgram(this);
		}
	}

	public final ProgramContext program() throws RecognitionException {
		ProgramContext _localctx = new ProgramContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_program);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(55);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==IMPORT) {
				{
				{
				setState(52);
				import_stmt();
				}
				}
				setState(57);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(61);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==LABEL) {
				{
				{
				setState(58);
				label_block();
				}
				}
				setState(63);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(64);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Label_blockContext extends ParserRuleContext {
		public Token label;
		public TerminalNode LABEL() { return getToken(DSParser.LABEL, 0); }
		public TerminalNode COLON() { return getToken(DSParser.COLON, 0); }
		public TerminalNode NEWLINE() { return getToken(DSParser.NEWLINE, 0); }
		public TerminalNode ID() { return getToken(DSParser.ID, 0); }
		public List<StatementContext> statement() {
			return getRuleContexts(StatementContext.class);
		}
		public StatementContext statement(int i) {
			return getRuleContext(StatementContext.class,i);
		}
		public List<TerminalNode> INDENT() { return getTokens(DSParser.INDENT); }
		public TerminalNode INDENT(int i) {
			return getToken(DSParser.INDENT, i);
		}
		public List<TerminalNode> DEDENT() { return getTokens(DSParser.DEDENT); }
		public TerminalNode DEDENT(int i) {
			return getToken(DSParser.DEDENT, i);
		}
		public Label_blockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_label_block; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterLabel_block(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitLabel_block(this);
		}
	}

	public final Label_blockContext label_block() throws RecognitionException {
		Label_blockContext _localctx = new Label_blockContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_label_block);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(66);
			match(LABEL);
			setState(67);
			((Label_blockContext)_localctx).label = match(ID);
			setState(68);
			match(COLON);
			setState(69);
			match(NEWLINE);
			setState(83); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(73);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==INDENT) {
					{
					{
					setState(70);
					match(INDENT);
					}
					}
					setState(75);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(76);
				statement();
				setState(80);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==DEDENT) {
					{
					{
					setState(77);
					match(DEDENT);
					}
					}
					setState(82);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				}
				setState(85); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & 1410776497651730L) != 0) );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class StatementContext extends ParserRuleContext {
		public Dialogue_stmtContext dialogue_stmt() {
			return getRuleContext(Dialogue_stmtContext.class,0);
		}
		public Menu_stmtContext menu_stmt() {
			return getRuleContext(Menu_stmtContext.class,0);
		}
		public Jump_stmtContext jump_stmt() {
			return getRuleContext(Jump_stmtContext.class,0);
		}
		public Tour_stmtContext tour_stmt() {
			return getRuleContext(Tour_stmtContext.class,0);
		}
		public Call_stmtContext call_stmt() {
			return getRuleContext(Call_stmtContext.class,0);
		}
		public Assign_stmtContext assign_stmt() {
			return getRuleContext(Assign_stmtContext.class,0);
		}
		public If_stmtContext if_stmt() {
			return getRuleContext(If_stmtContext.class,0);
		}
		public StatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statement; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterStatement(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitStatement(this);
		}
	}

	public final StatementContext statement() throws RecognitionException {
		StatementContext _localctx = new StatementContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_statement);
		try {
			setState(94);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,5,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(87);
				dialogue_stmt();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(88);
				menu_stmt();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(89);
				jump_stmt();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(90);
				tour_stmt();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(91);
				call_stmt();
				}
				break;
			case 6:
				enterOuterAlt(_localctx, 6);
				{
				setState(92);
				assign_stmt();
				}
				break;
			case 7:
				enterOuterAlt(_localctx, 7);
				{
				setState(93);
				if_stmt();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Import_stmtContext extends ParserRuleContext {
		public Token path;
		public TerminalNode IMPORT() { return getToken(DSParser.IMPORT, 0); }
		public TerminalNode NEWLINE() { return getToken(DSParser.NEWLINE, 0); }
		public TerminalNode PATH() { return getToken(DSParser.PATH, 0); }
		public Import_stmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_import_stmt; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterImport_stmt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitImport_stmt(this);
		}
	}

	public final Import_stmtContext import_stmt() throws RecognitionException {
		Import_stmtContext _localctx = new Import_stmtContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_import_stmt);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(96);
			match(IMPORT);
			setState(97);
			((Import_stmtContext)_localctx).path = match(PATH);
			setState(98);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Dialogue_stmtContext extends ParserRuleContext {
		public Token speaker;
		public FstringContext text;
		public Token TAG;
		public List<Token> tags = new ArrayList<Token>();
		public TerminalNode NEWLINE() { return getToken(DSParser.NEWLINE, 0); }
		public FstringContext fstring() {
			return getRuleContext(FstringContext.class,0);
		}
		public TerminalNode ID() { return getToken(DSParser.ID, 0); }
		public List<TerminalNode> TAG() { return getTokens(DSParser.TAG); }
		public TerminalNode TAG(int i) {
			return getToken(DSParser.TAG, i);
		}
		public Dialogue_stmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_dialogue_stmt; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterDialogue_stmt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitDialogue_stmt(this);
		}
	}

	public final Dialogue_stmtContext dialogue_stmt() throws RecognitionException {
		Dialogue_stmtContext _localctx = new Dialogue_stmtContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_dialogue_stmt);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(101);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ID) {
				{
				setState(100);
				((Dialogue_stmtContext)_localctx).speaker = match(ID);
				}
			}

			setState(103);
			((Dialogue_stmtContext)_localctx).text = fstring();
			setState(107);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==TAG) {
				{
				{
				setState(104);
				((Dialogue_stmtContext)_localctx).TAG = match(TAG);
				((Dialogue_stmtContext)_localctx).tags.add(((Dialogue_stmtContext)_localctx).TAG);
				}
				}
				setState(109);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(110);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Menu_stmtContext extends ParserRuleContext {
		public Menu_itemContext menu_item;
		public List<Menu_itemContext> options = new ArrayList<Menu_itemContext>();
		public List<Menu_itemContext> menu_item() {
			return getRuleContexts(Menu_itemContext.class);
		}
		public Menu_itemContext menu_item(int i) {
			return getRuleContext(Menu_itemContext.class,i);
		}
		public Menu_stmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_menu_stmt; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterMenu_stmt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitMenu_stmt(this);
		}
	}

	public final Menu_stmtContext menu_stmt() throws RecognitionException {
		Menu_stmtContext _localctx = new Menu_stmtContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_menu_stmt);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(113); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(112);
					((Menu_stmtContext)_localctx).menu_item = menu_item();
					((Menu_stmtContext)_localctx).options.add(((Menu_stmtContext)_localctx).menu_item);
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(115); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,8,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Menu_itemContext extends ParserRuleContext {
		public FstringContext text;
		public TerminalNode COLON() { return getToken(DSParser.COLON, 0); }
		public TerminalNode NEWLINE() { return getToken(DSParser.NEWLINE, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public FstringContext fstring() {
			return getRuleContext(FstringContext.class,0);
		}
		public Menu_itemContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_menu_item; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterMenu_item(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitMenu_item(this);
		}
	}

	public final Menu_itemContext menu_item() throws RecognitionException {
		Menu_itemContext _localctx = new Menu_itemContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_menu_item);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(117);
			((Menu_itemContext)_localctx).text = fstring();
			setState(118);
			match(COLON);
			setState(119);
			match(NEWLINE);
			setState(120);
			block();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Jump_stmtContext extends ParserRuleContext {
		public Token label;
		public TerminalNode JUMP() { return getToken(DSParser.JUMP, 0); }
		public TerminalNode NEWLINE() { return getToken(DSParser.NEWLINE, 0); }
		public TerminalNode ID() { return getToken(DSParser.ID, 0); }
		public Jump_stmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_jump_stmt; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterJump_stmt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitJump_stmt(this);
		}
	}

	public final Jump_stmtContext jump_stmt() throws RecognitionException {
		Jump_stmtContext _localctx = new Jump_stmtContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_jump_stmt);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(122);
			match(JUMP);
			setState(123);
			((Jump_stmtContext)_localctx).label = match(ID);
			setState(124);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Tour_stmtContext extends ParserRuleContext {
		public Token label;
		public TerminalNode TOUR() { return getToken(DSParser.TOUR, 0); }
		public TerminalNode NEWLINE() { return getToken(DSParser.NEWLINE, 0); }
		public TerminalNode ID() { return getToken(DSParser.ID, 0); }
		public Tour_stmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_tour_stmt; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterTour_stmt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitTour_stmt(this);
		}
	}

	public final Tour_stmtContext tour_stmt() throws RecognitionException {
		Tour_stmtContext _localctx = new Tour_stmtContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_tour_stmt);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(126);
			match(TOUR);
			setState(127);
			((Tour_stmtContext)_localctx).label = match(ID);
			setState(128);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Call_stmtContext extends ParserRuleContext {
		public Token func_name;
		public ExpressionContext expression;
		public List<ExpressionContext> args = new ArrayList<ExpressionContext>();
		public TerminalNode CALL() { return getToken(DSParser.CALL, 0); }
		public TerminalNode LPAR() { return getToken(DSParser.LPAR, 0); }
		public TerminalNode RPAR() { return getToken(DSParser.RPAR, 0); }
		public TerminalNode NEWLINE() { return getToken(DSParser.NEWLINE, 0); }
		public TerminalNode ID() { return getToken(DSParser.ID, 0); }
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(DSParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(DSParser.COMMA, i);
		}
		public Call_stmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_call_stmt; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterCall_stmt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitCall_stmt(this);
		}
	}

	public final Call_stmtContext call_stmt() throws RecognitionException {
		Call_stmtContext _localctx = new Call_stmtContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_call_stmt);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(130);
			match(CALL);
			setState(131);
			((Call_stmtContext)_localctx).func_name = match(ID);
			setState(132);
			match(LPAR);
			setState(141);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 1284229581302288L) != 0)) {
				{
				setState(133);
				((Call_stmtContext)_localctx).expression = expression();
				((Call_stmtContext)_localctx).args.add(((Call_stmtContext)_localctx).expression);
				setState(138);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==COMMA) {
					{
					{
					setState(134);
					match(COMMA);
					setState(135);
					((Call_stmtContext)_localctx).expression = expression();
					((Call_stmtContext)_localctx).args.add(((Call_stmtContext)_localctx).expression);
					}
					}
					setState(140);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
			}

			setState(143);
			match(RPAR);
			setState(144);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Assign_stmtContext extends ParserRuleContext {
		public Token symbol;
		public ExpressionContext value;
		public TerminalNode VARIABLE() { return getToken(DSParser.VARIABLE, 0); }
		public TerminalNode NEWLINE() { return getToken(DSParser.NEWLINE, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode EQUAL() { return getToken(DSParser.EQUAL, 0); }
		public TerminalNode PLUSEQUAL() { return getToken(DSParser.PLUSEQUAL, 0); }
		public TerminalNode MINEQUAL() { return getToken(DSParser.MINEQUAL, 0); }
		public TerminalNode STAREQUAL() { return getToken(DSParser.STAREQUAL, 0); }
		public TerminalNode SLASHEQUAL() { return getToken(DSParser.SLASHEQUAL, 0); }
		public TerminalNode PERCENTEQUAL() { return getToken(DSParser.PERCENTEQUAL, 0); }
		public Assign_stmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assign_stmt; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterAssign_stmt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitAssign_stmt(this);
		}
	}

	public final Assign_stmtContext assign_stmt() throws RecognitionException {
		Assign_stmtContext _localctx = new Assign_stmtContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_assign_stmt);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(146);
			match(VARIABLE);
			setState(147);
			((Assign_stmtContext)_localctx).symbol = _input.LT(1);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 2081423360L) != 0)) ) {
				((Assign_stmtContext)_localctx).symbol = (Token)_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			setState(148);
			((Assign_stmtContext)_localctx).value = expression();
			setState(149);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class If_stmtContext extends ParserRuleContext {
		public ConditionContext condition;
		public List<ConditionContext> conditions = new ArrayList<ConditionContext>();
		public BlockContext block;
		public List<BlockContext> blocks = new ArrayList<BlockContext>();
		public TerminalNode IF() { return getToken(DSParser.IF, 0); }
		public List<TerminalNode> COLON() { return getTokens(DSParser.COLON); }
		public TerminalNode COLON(int i) {
			return getToken(DSParser.COLON, i);
		}
		public List<TerminalNode> NEWLINE() { return getTokens(DSParser.NEWLINE); }
		public TerminalNode NEWLINE(int i) {
			return getToken(DSParser.NEWLINE, i);
		}
		public List<ConditionContext> condition() {
			return getRuleContexts(ConditionContext.class);
		}
		public ConditionContext condition(int i) {
			return getRuleContext(ConditionContext.class,i);
		}
		public List<BlockContext> block() {
			return getRuleContexts(BlockContext.class);
		}
		public BlockContext block(int i) {
			return getRuleContext(BlockContext.class,i);
		}
		public List<TerminalNode> ELIF() { return getTokens(DSParser.ELIF); }
		public TerminalNode ELIF(int i) {
			return getToken(DSParser.ELIF, i);
		}
		public TerminalNode ELSE() { return getToken(DSParser.ELSE, 0); }
		public If_stmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_if_stmt; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterIf_stmt(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitIf_stmt(this);
		}
	}

	public final If_stmtContext if_stmt() throws RecognitionException {
		If_stmtContext _localctx = new If_stmtContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_if_stmt);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(151);
			match(IF);
			setState(152);
			((If_stmtContext)_localctx).condition = condition();
			((If_stmtContext)_localctx).conditions.add(((If_stmtContext)_localctx).condition);
			setState(153);
			match(COLON);
			setState(154);
			match(NEWLINE);
			setState(155);
			((If_stmtContext)_localctx).block = block();
			((If_stmtContext)_localctx).blocks.add(((If_stmtContext)_localctx).block);
			setState(164);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==ELIF) {
				{
				{
				setState(156);
				match(ELIF);
				setState(157);
				((If_stmtContext)_localctx).condition = condition();
				((If_stmtContext)_localctx).conditions.add(((If_stmtContext)_localctx).condition);
				setState(158);
				match(COLON);
				setState(159);
				match(NEWLINE);
				setState(160);
				((If_stmtContext)_localctx).block = block();
				((If_stmtContext)_localctx).blocks.add(((If_stmtContext)_localctx).block);
				}
				}
				setState(166);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(171);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ELSE) {
				{
				setState(167);
				match(ELSE);
				setState(168);
				match(COLON);
				setState(169);
				match(NEWLINE);
				setState(170);
				((If_stmtContext)_localctx).block = block();
				((If_stmtContext)_localctx).blocks.add(((If_stmtContext)_localctx).block);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ExpressionContext extends ParserRuleContext {
		public List<Expr_logical_andContext> expr_logical_and() {
			return getRuleContexts(Expr_logical_andContext.class);
		}
		public Expr_logical_andContext expr_logical_and(int i) {
			return getRuleContext(Expr_logical_andContext.class,i);
		}
		public List<TerminalNode> OR() { return getTokens(DSParser.OR); }
		public TerminalNode OR(int i) {
			return getToken(DSParser.OR, i);
		}
		public ExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expression; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitExpression(this);
		}
	}

	public final ExpressionContext expression() throws RecognitionException {
		ExpressionContext _localctx = new ExpressionContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(173);
			expr_logical_and();
			setState(178);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==OR) {
				{
				{
				setState(174);
				match(OR);
				setState(175);
				expr_logical_and();
				}
				}
				setState(180);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Expr_logical_andContext extends ParserRuleContext {
		public List<Expr_equalityContext> expr_equality() {
			return getRuleContexts(Expr_equalityContext.class);
		}
		public Expr_equalityContext expr_equality(int i) {
			return getRuleContext(Expr_equalityContext.class,i);
		}
		public List<TerminalNode> AND() { return getTokens(DSParser.AND); }
		public TerminalNode AND(int i) {
			return getToken(DSParser.AND, i);
		}
		public Expr_logical_andContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr_logical_and; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterExpr_logical_and(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitExpr_logical_and(this);
		}
	}

	public final Expr_logical_andContext expr_logical_and() throws RecognitionException {
		Expr_logical_andContext _localctx = new Expr_logical_andContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_expr_logical_and);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(181);
			expr_equality();
			setState(186);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==AND) {
				{
				{
				setState(182);
				match(AND);
				setState(183);
				expr_equality();
				}
				}
				setState(188);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Expr_equalityContext extends ParserRuleContext {
		public List<Expr_comparisonContext> expr_comparison() {
			return getRuleContexts(Expr_comparisonContext.class);
		}
		public Expr_comparisonContext expr_comparison(int i) {
			return getRuleContext(Expr_comparisonContext.class,i);
		}
		public List<TerminalNode> EQEQUAL() { return getTokens(DSParser.EQEQUAL); }
		public TerminalNode EQEQUAL(int i) {
			return getToken(DSParser.EQEQUAL, i);
		}
		public List<TerminalNode> NOTEQUAL() { return getTokens(DSParser.NOTEQUAL); }
		public TerminalNode NOTEQUAL(int i) {
			return getToken(DSParser.NOTEQUAL, i);
		}
		public Expr_equalityContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr_equality; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterExpr_equality(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitExpr_equality(this);
		}
	}

	public final Expr_equalityContext expr_equality() throws RecognitionException {
		Expr_equalityContext _localctx = new Expr_equalityContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_expr_equality);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(189);
			expr_comparison();
			setState(194);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==EQEQUAL || _la==NOTEQUAL) {
				{
				{
				setState(190);
				_la = _input.LA(1);
				if ( !(_la==EQEQUAL || _la==NOTEQUAL) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(191);
				expr_comparison();
				}
				}
				setState(196);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Expr_comparisonContext extends ParserRuleContext {
		public List<Expr_termContext> expr_term() {
			return getRuleContexts(Expr_termContext.class);
		}
		public Expr_termContext expr_term(int i) {
			return getRuleContext(Expr_termContext.class,i);
		}
		public List<TerminalNode> GREATER() { return getTokens(DSParser.GREATER); }
		public TerminalNode GREATER(int i) {
			return getToken(DSParser.GREATER, i);
		}
		public List<TerminalNode> LESS() { return getTokens(DSParser.LESS); }
		public TerminalNode LESS(int i) {
			return getToken(DSParser.LESS, i);
		}
		public List<TerminalNode> GREATEREQUAL() { return getTokens(DSParser.GREATEREQUAL); }
		public TerminalNode GREATEREQUAL(int i) {
			return getToken(DSParser.GREATEREQUAL, i);
		}
		public List<TerminalNode> LESSEQUAL() { return getTokens(DSParser.LESSEQUAL); }
		public TerminalNode LESSEQUAL(int i) {
			return getToken(DSParser.LESSEQUAL, i);
		}
		public Expr_comparisonContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr_comparison; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterExpr_comparison(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitExpr_comparison(this);
		}
	}

	public final Expr_comparisonContext expr_comparison() throws RecognitionException {
		Expr_comparisonContext _localctx = new Expr_comparisonContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_expr_comparison);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(197);
			expr_term();
			setState(202);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 51118080L) != 0)) {
				{
				{
				setState(198);
				_la = _input.LA(1);
				if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 51118080L) != 0)) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(199);
				expr_term();
				}
				}
				setState(204);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Expr_termContext extends ParserRuleContext {
		public List<Expr_factorContext> expr_factor() {
			return getRuleContexts(Expr_factorContext.class);
		}
		public Expr_factorContext expr_factor(int i) {
			return getRuleContext(Expr_factorContext.class,i);
		}
		public List<TerminalNode> PLUS() { return getTokens(DSParser.PLUS); }
		public TerminalNode PLUS(int i) {
			return getToken(DSParser.PLUS, i);
		}
		public List<TerminalNode> MINUS() { return getTokens(DSParser.MINUS); }
		public TerminalNode MINUS(int i) {
			return getToken(DSParser.MINUS, i);
		}
		public Expr_termContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr_term; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterExpr_term(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitExpr_term(this);
		}
	}

	public final Expr_termContext expr_term() throws RecognitionException {
		Expr_termContext _localctx = new Expr_termContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_expr_term);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(205);
			expr_factor();
			setState(210);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==PLUS || _la==MINUS) {
				{
				{
				setState(206);
				_la = _input.LA(1);
				if ( !(_la==PLUS || _la==MINUS) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(207);
				expr_factor();
				}
				}
				setState(212);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Expr_factorContext extends ParserRuleContext {
		public List<Expr_unaryContext> expr_unary() {
			return getRuleContexts(Expr_unaryContext.class);
		}
		public Expr_unaryContext expr_unary(int i) {
			return getRuleContext(Expr_unaryContext.class,i);
		}
		public List<TerminalNode> STAR() { return getTokens(DSParser.STAR); }
		public TerminalNode STAR(int i) {
			return getToken(DSParser.STAR, i);
		}
		public List<TerminalNode> SLASH() { return getTokens(DSParser.SLASH); }
		public TerminalNode SLASH(int i) {
			return getToken(DSParser.SLASH, i);
		}
		public List<TerminalNode> PERCENT() { return getTokens(DSParser.PERCENT); }
		public TerminalNode PERCENT(int i) {
			return getToken(DSParser.PERCENT, i);
		}
		public Expr_factorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr_factor; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterExpr_factor(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitExpr_factor(this);
		}
	}

	public final Expr_factorContext expr_factor() throws RecognitionException {
		Expr_factorContext _localctx = new Expr_factorContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_expr_factor);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(213);
			expr_unary();
			setState(218);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 2293760L) != 0)) {
				{
				{
				setState(214);
				_la = _input.LA(1);
				if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 2293760L) != 0)) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(215);
				expr_unary();
				}
				}
				setState(220);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Expr_unaryContext extends ParserRuleContext {
		public Expr_primaryContext expr_primary() {
			return getRuleContext(Expr_primaryContext.class,0);
		}
		public TerminalNode PLUS() { return getToken(DSParser.PLUS, 0); }
		public TerminalNode MINUS() { return getToken(DSParser.MINUS, 0); }
		public TerminalNode EXCLAMATION() { return getToken(DSParser.EXCLAMATION, 0); }
		public Expr_unaryContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr_unary; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterExpr_unary(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitExpr_unary(this);
		}
	}

	public final Expr_unaryContext expr_unary() throws RecognitionException {
		Expr_unaryContext _localctx = new Expr_unaryContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_expr_unary);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(222);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 57344L) != 0)) {
				{
				setState(221);
				_la = _input.LA(1);
				if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 57344L) != 0)) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
			}

			setState(224);
			expr_primary();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Expr_primaryContext extends ParserRuleContext {
		public TerminalNode VARIABLE() { return getToken(DSParser.VARIABLE, 0); }
		public TerminalNode NUMBER() { return getToken(DSParser.NUMBER, 0); }
		public TerminalNode BOOL() { return getToken(DSParser.BOOL, 0); }
		public FstringContext fstring() {
			return getRuleContext(FstringContext.class,0);
		}
		public TerminalNode LPAR() { return getToken(DSParser.LPAR, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode RPAR() { return getToken(DSParser.RPAR, 0); }
		public Embedded_callContext embedded_call() {
			return getRuleContext(Embedded_callContext.class,0);
		}
		public Expr_primaryContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr_primary; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterExpr_primary(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitExpr_primary(this);
		}
	}

	public final Expr_primaryContext expr_primary() throws RecognitionException {
		Expr_primaryContext _localctx = new Expr_primaryContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_expr_primary);
		try {
			setState(235);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case VARIABLE:
				enterOuterAlt(_localctx, 1);
				{
				setState(226);
				match(VARIABLE);
				}
				break;
			case NUMBER:
				enterOuterAlt(_localctx, 2);
				{
				setState(227);
				match(NUMBER);
				}
				break;
			case BOOL:
				enterOuterAlt(_localctx, 3);
				{
				setState(228);
				match(BOOL);
				}
				break;
			case STRING_START:
				enterOuterAlt(_localctx, 4);
				{
				setState(229);
				fstring();
				}
				break;
			case LPAR:
				enterOuterAlt(_localctx, 5);
				{
				setState(230);
				match(LPAR);
				setState(231);
				expression();
				setState(232);
				match(RPAR);
				}
				break;
			case LBRACE:
				enterOuterAlt(_localctx, 6);
				{
				setState(234);
				embedded_call();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Embedded_exprContext extends ParserRuleContext {
		public Embedded_callContext embedded_call() {
			return getRuleContext(Embedded_callContext.class,0);
		}
		public TerminalNode LBRACE() { return getToken(DSParser.LBRACE, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode RBRACE() { return getToken(DSParser.RBRACE, 0); }
		public Embedded_exprContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_embedded_expr; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterEmbedded_expr(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitEmbedded_expr(this);
		}
	}

	public final Embedded_exprContext embedded_expr() throws RecognitionException {
		Embedded_exprContext _localctx = new Embedded_exprContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_embedded_expr);
		try {
			setState(242);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,21,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(237);
				embedded_call();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(238);
				match(LBRACE);
				setState(239);
				expression();
				setState(240);
				match(RBRACE);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Embedded_callContext extends ParserRuleContext {
		public Token func_name;
		public ExpressionContext expression;
		public List<ExpressionContext> args = new ArrayList<ExpressionContext>();
		public TerminalNode LBRACE() { return getToken(DSParser.LBRACE, 0); }
		public TerminalNode CALL() { return getToken(DSParser.CALL, 0); }
		public TerminalNode LPAR() { return getToken(DSParser.LPAR, 0); }
		public TerminalNode RPAR() { return getToken(DSParser.RPAR, 0); }
		public TerminalNode RBRACE() { return getToken(DSParser.RBRACE, 0); }
		public TerminalNode ID() { return getToken(DSParser.ID, 0); }
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(DSParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(DSParser.COMMA, i);
		}
		public Embedded_callContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_embedded_call; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterEmbedded_call(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitEmbedded_call(this);
		}
	}

	public final Embedded_callContext embedded_call() throws RecognitionException {
		Embedded_callContext _localctx = new Embedded_callContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_embedded_call);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(244);
			match(LBRACE);
			setState(245);
			match(CALL);
			setState(246);
			((Embedded_callContext)_localctx).func_name = match(ID);
			setState(247);
			match(LPAR);
			setState(256);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 1284229581302288L) != 0)) {
				{
				setState(248);
				((Embedded_callContext)_localctx).expression = expression();
				((Embedded_callContext)_localctx).args.add(((Embedded_callContext)_localctx).expression);
				setState(253);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==COMMA) {
					{
					{
					setState(249);
					match(COMMA);
					setState(250);
					((Embedded_callContext)_localctx).expression = expression();
					((Embedded_callContext)_localctx).args.add(((Embedded_callContext)_localctx).expression);
					}
					}
					setState(255);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
			}

			setState(258);
			match(RPAR);
			setState(259);
			match(RBRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class BlockContext extends ParserRuleContext {
		public List<TerminalNode> INDENT() { return getTokens(DSParser.INDENT); }
		public TerminalNode INDENT(int i) {
			return getToken(DSParser.INDENT, i);
		}
		public List<StatementContext> statement() {
			return getRuleContexts(StatementContext.class);
		}
		public StatementContext statement(int i) {
			return getRuleContext(StatementContext.class,i);
		}
		public List<TerminalNode> DEDENT() { return getTokens(DSParser.DEDENT); }
		public TerminalNode DEDENT(int i) {
			return getToken(DSParser.DEDENT, i);
		}
		public BlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_block; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterBlock(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitBlock(this);
		}
	}

	public final BlockContext block() throws RecognitionException {
		BlockContext _localctx = new BlockContext(_ctx, getState());
		enterRule(_localctx, 44, RULE_block);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(262); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(261);
				match(INDENT);
				}
				}
				setState(264); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==INDENT );
			setState(267); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(266);
				statement();
				}
				}
				setState(269); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & 1410776497651728L) != 0) );
			setState(272); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(271);
					match(DEDENT);
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(274); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,26,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class FstringContext extends ParserRuleContext {
		public String_fragmentContext string_fragment;
		public List<String_fragmentContext> frag = new ArrayList<String_fragmentContext>();
		public Embedded_exprContext embedded_expr;
		public List<Embedded_exprContext> embed = new ArrayList<Embedded_exprContext>();
		public TerminalNode STRING_START() { return getToken(DSParser.STRING_START, 0); }
		public TerminalNode STRING_END() { return getToken(DSParser.STRING_END, 0); }
		public List<String_fragmentContext> string_fragment() {
			return getRuleContexts(String_fragmentContext.class);
		}
		public String_fragmentContext string_fragment(int i) {
			return getRuleContext(String_fragmentContext.class,i);
		}
		public List<Embedded_exprContext> embedded_expr() {
			return getRuleContexts(Embedded_exprContext.class);
		}
		public Embedded_exprContext embedded_expr(int i) {
			return getRuleContext(Embedded_exprContext.class,i);
		}
		public FstringContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fstring; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterFstring(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitFstring(this);
		}
	}

	public final FstringContext fstring() throws RecognitionException {
		FstringContext _localctx = new FstringContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_fstring);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(276);
			match(STRING_START);
			setState(281);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 2144L) != 0)) {
				{
				setState(279);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case STRING_CONTEXT:
				case STRING_ESCAPE:
					{
					setState(277);
					((FstringContext)_localctx).string_fragment = string_fragment();
					((FstringContext)_localctx).frag.add(((FstringContext)_localctx).string_fragment);
					}
					break;
				case LBRACE:
					{
					setState(278);
					((FstringContext)_localctx).embedded_expr = embedded_expr();
					((FstringContext)_localctx).embed.add(((FstringContext)_localctx).embedded_expr);
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				setState(283);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(284);
			match(STRING_END);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class String_fragmentContext extends ParserRuleContext {
		public TerminalNode STRING_CONTEXT() { return getToken(DSParser.STRING_CONTEXT, 0); }
		public TerminalNode STRING_ESCAPE() { return getToken(DSParser.STRING_ESCAPE, 0); }
		public String_fragmentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_string_fragment; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterString_fragment(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitString_fragment(this);
		}
	}

	public final String_fragmentContext string_fragment() throws RecognitionException {
		String_fragmentContext _localctx = new String_fragmentContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_string_fragment);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(286);
			_la = _input.LA(1);
			if ( !(_la==STRING_CONTEXT || _la==STRING_ESCAPE) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ConditionContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode NOT() { return getToken(DSParser.NOT, 0); }
		public ConditionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_condition; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).enterCondition(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DSParserListener ) ((DSParserListener)listener).exitCondition(this);
		}
	}

	public final ConditionContext condition() throws RecognitionException {
		ConditionContext _localctx = new ConditionContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_condition);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(289);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==NOT) {
				{
				setState(288);
				match(NOT);
				}
			}

			setState(291);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static final String _serializedATN =
		"\u0004\u00018\u0126\u0002\u0000\u0007\u0000\u0002\u0001\u0007\u0001\u0002"+
		"\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002\u0004\u0007\u0004\u0002"+
		"\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0002\u0007\u0007\u0007\u0002"+
		"\b\u0007\b\u0002\t\u0007\t\u0002\n\u0007\n\u0002\u000b\u0007\u000b\u0002"+
		"\f\u0007\f\u0002\r\u0007\r\u0002\u000e\u0007\u000e\u0002\u000f\u0007\u000f"+
		"\u0002\u0010\u0007\u0010\u0002\u0011\u0007\u0011\u0002\u0012\u0007\u0012"+
		"\u0002\u0013\u0007\u0013\u0002\u0014\u0007\u0014\u0002\u0015\u0007\u0015"+
		"\u0002\u0016\u0007\u0016\u0002\u0017\u0007\u0017\u0002\u0018\u0007\u0018"+
		"\u0002\u0019\u0007\u0019\u0001\u0000\u0005\u00006\b\u0000\n\u0000\f\u0000"+
		"9\t\u0000\u0001\u0000\u0005\u0000<\b\u0000\n\u0000\f\u0000?\t\u0000\u0001"+
		"\u0000\u0001\u0000\u0001\u0001\u0001\u0001\u0001\u0001\u0001\u0001\u0001"+
		"\u0001\u0005\u0001H\b\u0001\n\u0001\f\u0001K\t\u0001\u0001\u0001\u0001"+
		"\u0001\u0005\u0001O\b\u0001\n\u0001\f\u0001R\t\u0001\u0004\u0001T\b\u0001"+
		"\u000b\u0001\f\u0001U\u0001\u0002\u0001\u0002\u0001\u0002\u0001\u0002"+
		"\u0001\u0002\u0001\u0002\u0001\u0002\u0003\u0002_\b\u0002\u0001\u0003"+
		"\u0001\u0003\u0001\u0003\u0001\u0003\u0001\u0004\u0003\u0004f\b\u0004"+
		"\u0001\u0004\u0001\u0004\u0005\u0004j\b\u0004\n\u0004\f\u0004m\t\u0004"+
		"\u0001\u0004\u0001\u0004\u0001\u0005\u0004\u0005r\b\u0005\u000b\u0005"+
		"\f\u0005s\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006"+
		"\u0001\u0007\u0001\u0007\u0001\u0007\u0001\u0007\u0001\b\u0001\b\u0001"+
		"\b\u0001\b\u0001\t\u0001\t\u0001\t\u0001\t\u0001\t\u0001\t\u0005\t\u0089"+
		"\b\t\n\t\f\t\u008c\t\t\u0003\t\u008e\b\t\u0001\t\u0001\t\u0001\t\u0001"+
		"\n\u0001\n\u0001\n\u0001\n\u0001\n\u0001\u000b\u0001\u000b\u0001\u000b"+
		"\u0001\u000b\u0001\u000b\u0001\u000b\u0001\u000b\u0001\u000b\u0001\u000b"+
		"\u0001\u000b\u0001\u000b\u0005\u000b\u00a3\b\u000b\n\u000b\f\u000b\u00a6"+
		"\t\u000b\u0001\u000b\u0001\u000b\u0001\u000b\u0001\u000b\u0003\u000b\u00ac"+
		"\b\u000b\u0001\f\u0001\f\u0001\f\u0005\f\u00b1\b\f\n\f\f\f\u00b4\t\f\u0001"+
		"\r\u0001\r\u0001\r\u0005\r\u00b9\b\r\n\r\f\r\u00bc\t\r\u0001\u000e\u0001"+
		"\u000e\u0001\u000e\u0005\u000e\u00c1\b\u000e\n\u000e\f\u000e\u00c4\t\u000e"+
		"\u0001\u000f\u0001\u000f\u0001\u000f\u0005\u000f\u00c9\b\u000f\n\u000f"+
		"\f\u000f\u00cc\t\u000f\u0001\u0010\u0001\u0010\u0001\u0010\u0005\u0010"+
		"\u00d1\b\u0010\n\u0010\f\u0010\u00d4\t\u0010\u0001\u0011\u0001\u0011\u0001"+
		"\u0011\u0005\u0011\u00d9\b\u0011\n\u0011\f\u0011\u00dc\t\u0011\u0001\u0012"+
		"\u0003\u0012\u00df\b\u0012\u0001\u0012\u0001\u0012\u0001\u0013\u0001\u0013"+
		"\u0001\u0013\u0001\u0013\u0001\u0013\u0001\u0013\u0001\u0013\u0001\u0013"+
		"\u0001\u0013\u0003\u0013\u00ec\b\u0013\u0001\u0014\u0001\u0014\u0001\u0014"+
		"\u0001\u0014\u0001\u0014\u0003\u0014\u00f3\b\u0014\u0001\u0015\u0001\u0015"+
		"\u0001\u0015\u0001\u0015\u0001\u0015\u0001\u0015\u0001\u0015\u0005\u0015"+
		"\u00fc\b\u0015\n\u0015\f\u0015\u00ff\t\u0015\u0003\u0015\u0101\b\u0015"+
		"\u0001\u0015\u0001\u0015\u0001\u0015\u0001\u0016\u0004\u0016\u0107\b\u0016"+
		"\u000b\u0016\f\u0016\u0108\u0001\u0016\u0004\u0016\u010c\b\u0016\u000b"+
		"\u0016\f\u0016\u010d\u0001\u0016\u0004\u0016\u0111\b\u0016\u000b\u0016"+
		"\f\u0016\u0112\u0001\u0017\u0001\u0017\u0001\u0017\u0005\u0017\u0118\b"+
		"\u0017\n\u0017\f\u0017\u011b\t\u0017\u0001\u0017\u0001\u0017\u0001\u0018"+
		"\u0001\u0018\u0001\u0019\u0003\u0019\u0122\b\u0019\u0001\u0019\u0001\u0019"+
		"\u0001\u0019\u0000\u0000\u001a\u0000\u0002\u0004\u0006\b\n\f\u000e\u0010"+
		"\u0012\u0014\u0016\u0018\u001a\u001c\u001e \"$&(*,.02\u0000\u0007\u0002"+
		"\u0000\u0014\u0014\u001a\u001e\u0001\u0000\u0016\u0017\u0002\u0000\u0012"+
		"\u0013\u0018\u0019\u0001\u0000\u000e\u000f\u0002\u0000\u0010\u0011\u0015"+
		"\u0015\u0001\u0000\r\u000f\u0001\u0000\u0005\u0006\u0132\u00007\u0001"+
		"\u0000\u0000\u0000\u0002B\u0001\u0000\u0000\u0000\u0004^\u0001\u0000\u0000"+
		"\u0000\u0006`\u0001\u0000\u0000\u0000\be\u0001\u0000\u0000\u0000\nq\u0001"+
		"\u0000\u0000\u0000\fu\u0001\u0000\u0000\u0000\u000ez\u0001\u0000\u0000"+
		"\u0000\u0010~\u0001\u0000\u0000\u0000\u0012\u0082\u0001\u0000\u0000\u0000"+
		"\u0014\u0092\u0001\u0000\u0000\u0000\u0016\u0097\u0001\u0000\u0000\u0000"+
		"\u0018\u00ad\u0001\u0000\u0000\u0000\u001a\u00b5\u0001\u0000\u0000\u0000"+
		"\u001c\u00bd\u0001\u0000\u0000\u0000\u001e\u00c5\u0001\u0000\u0000\u0000"+
		" \u00cd\u0001\u0000\u0000\u0000\"\u00d5\u0001\u0000\u0000\u0000$\u00de"+
		"\u0001\u0000\u0000\u0000&\u00eb\u0001\u0000\u0000\u0000(\u00f2\u0001\u0000"+
		"\u0000\u0000*\u00f4\u0001\u0000\u0000\u0000,\u0106\u0001\u0000\u0000\u0000"+
		".\u0114\u0001\u0000\u0000\u00000\u011e\u0001\u0000\u0000\u00002\u0121"+
		"\u0001\u0000\u0000\u000046\u0003\u0006\u0003\u000054\u0001\u0000\u0000"+
		"\u000069\u0001\u0000\u0000\u000075\u0001\u0000\u0000\u000078\u0001\u0000"+
		"\u0000\u00008=\u0001\u0000\u0000\u000097\u0001\u0000\u0000\u0000:<\u0003"+
		"\u0002\u0001\u0000;:\u0001\u0000\u0000\u0000<?\u0001\u0000\u0000\u0000"+
		"=;\u0001\u0000\u0000\u0000=>\u0001\u0000\u0000\u0000>@\u0001\u0000\u0000"+
		"\u0000?=\u0001\u0000\u0000\u0000@A\u0005\u0000\u0000\u0001A\u0001\u0001"+
		"\u0000\u0000\u0000BC\u0005*\u0000\u0000CD\u00050\u0000\u0000DE\u0005!"+
		"\u0000\u0000ES\u00056\u0000\u0000FH\u0005\u0001\u0000\u0000GF\u0001\u0000"+
		"\u0000\u0000HK\u0001\u0000\u0000\u0000IG\u0001\u0000\u0000\u0000IJ\u0001"+
		"\u0000\u0000\u0000JL\u0001\u0000\u0000\u0000KI\u0001\u0000\u0000\u0000"+
		"LP\u0003\u0004\u0002\u0000MO\u0005\u0002\u0000\u0000NM\u0001\u0000\u0000"+
		"\u0000OR\u0001\u0000\u0000\u0000PN\u0001\u0000\u0000\u0000PQ\u0001\u0000"+
		"\u0000\u0000QT\u0001\u0000\u0000\u0000RP\u0001\u0000\u0000\u0000SI\u0001"+
		"\u0000\u0000\u0000TU\u0001\u0000\u0000\u0000US\u0001\u0000\u0000\u0000"+
		"UV\u0001\u0000\u0000\u0000V\u0003\u0001\u0000\u0000\u0000W_\u0003\b\u0004"+
		"\u0000X_\u0003\n\u0005\u0000Y_\u0003\u000e\u0007\u0000Z_\u0003\u0010\b"+
		"\u0000[_\u0003\u0012\t\u0000\\_\u0003\u0014\n\u0000]_\u0003\u0016\u000b"+
		"\u0000^W\u0001\u0000\u0000\u0000^X\u0001\u0000\u0000\u0000^Y\u0001\u0000"+
		"\u0000\u0000^Z\u0001\u0000\u0000\u0000^[\u0001\u0000\u0000\u0000^\\\u0001"+
		"\u0000\u0000\u0000^]\u0001\u0000\u0000\u0000_\u0005\u0001\u0000\u0000"+
		"\u0000`a\u0005+\u0000\u0000ab\u0005\b\u0000\u0000bc\u00056\u0000\u0000"+
		"c\u0007\u0001\u0000\u0000\u0000df\u00050\u0000\u0000ed\u0001\u0000\u0000"+
		"\u0000ef\u0001\u0000\u0000\u0000fg\u0001\u0000\u0000\u0000gk\u0003.\u0017"+
		"\u0000hj\u00051\u0000\u0000ih\u0001\u0000\u0000\u0000jm\u0001\u0000\u0000"+
		"\u0000ki\u0001\u0000\u0000\u0000kl\u0001\u0000\u0000\u0000ln\u0001\u0000"+
		"\u0000\u0000mk\u0001\u0000\u0000\u0000no\u00056\u0000\u0000o\t\u0001\u0000"+
		"\u0000\u0000pr\u0003\f\u0006\u0000qp\u0001\u0000\u0000\u0000rs\u0001\u0000"+
		"\u0000\u0000sq\u0001\u0000\u0000\u0000st\u0001\u0000\u0000\u0000t\u000b"+
		"\u0001\u0000\u0000\u0000uv\u0003.\u0017\u0000vw\u0005!\u0000\u0000wx\u0005"+
		"6\u0000\u0000xy\u0003,\u0016\u0000y\r\u0001\u0000\u0000\u0000z{\u0005"+
		"(\u0000\u0000{|\u00050\u0000\u0000|}\u00056\u0000\u0000}\u000f\u0001\u0000"+
		"\u0000\u0000~\u007f\u0005)\u0000\u0000\u007f\u0080\u00050\u0000\u0000"+
		"\u0080\u0081\u00056\u0000\u0000\u0081\u0011\u0001\u0000\u0000\u0000\u0082"+
		"\u0083\u0005#\u0000\u0000\u0083\u0084\u00050\u0000\u0000\u0084\u008d\u0005"+
		"\t\u0000\u0000\u0085\u008a\u0003\u0018\f\u0000\u0086\u0087\u0005\"\u0000"+
		"\u0000\u0087\u0089\u0003\u0018\f\u0000\u0088\u0086\u0001\u0000\u0000\u0000"+
		"\u0089\u008c\u0001\u0000\u0000\u0000\u008a\u0088\u0001\u0000\u0000\u0000"+
		"\u008a\u008b\u0001\u0000\u0000\u0000\u008b\u008e\u0001\u0000\u0000\u0000"+
		"\u008c\u008a\u0001\u0000\u0000\u0000\u008d\u0085\u0001\u0000\u0000\u0000"+
		"\u008d\u008e\u0001\u0000\u0000\u0000\u008e\u008f\u0001\u0000\u0000\u0000"+
		"\u008f\u0090\u0005\n\u0000\u0000\u0090\u0091\u00056\u0000\u0000\u0091"+
		"\u0013\u0001\u0000\u0000\u0000\u0092\u0093\u00052\u0000\u0000\u0093\u0094"+
		"\u0007\u0000\u0000\u0000\u0094\u0095\u0003\u0018\f\u0000\u0095\u0096\u0005"+
		"6\u0000\u0000\u0096\u0015\u0001\u0000\u0000\u0000\u0097\u0098\u0005$\u0000"+
		"\u0000\u0098\u0099\u00032\u0019\u0000\u0099\u009a\u0005!\u0000\u0000\u009a"+
		"\u009b\u00056\u0000\u0000\u009b\u00a4\u0003,\u0016\u0000\u009c\u009d\u0005"+
		"&\u0000\u0000\u009d\u009e\u00032\u0019\u0000\u009e\u009f\u0005!\u0000"+
		"\u0000\u009f\u00a0\u00056\u0000\u0000\u00a0\u00a1\u0003,\u0016\u0000\u00a1"+
		"\u00a3\u0001\u0000\u0000\u0000\u00a2\u009c\u0001\u0000\u0000\u0000\u00a3"+
		"\u00a6\u0001\u0000\u0000\u0000\u00a4\u00a2\u0001\u0000\u0000\u0000\u00a4"+
		"\u00a5\u0001\u0000\u0000\u0000\u00a5\u00ab\u0001\u0000\u0000\u0000\u00a6"+
		"\u00a4\u0001\u0000\u0000\u0000\u00a7\u00a8\u0005\'\u0000\u0000\u00a8\u00a9"+
		"\u0005!\u0000\u0000\u00a9\u00aa\u00056\u0000\u0000\u00aa\u00ac\u0003,"+
		"\u0016\u0000\u00ab\u00a7\u0001\u0000\u0000\u0000\u00ab\u00ac\u0001\u0000"+
		"\u0000\u0000\u00ac\u0017\u0001\u0000\u0000\u0000\u00ad\u00b2\u0003\u001a"+
		"\r\u0000\u00ae\u00af\u0005 \u0000\u0000\u00af\u00b1\u0003\u001a\r\u0000"+
		"\u00b0\u00ae\u0001\u0000\u0000\u0000\u00b1\u00b4\u0001\u0000\u0000\u0000"+
		"\u00b2\u00b0\u0001\u0000\u0000\u0000\u00b2\u00b3\u0001\u0000\u0000\u0000"+
		"\u00b3\u0019\u0001\u0000\u0000\u0000\u00b4\u00b2\u0001\u0000\u0000\u0000"+
		"\u00b5\u00ba\u0003\u001c\u000e\u0000\u00b6\u00b7\u0005\u001f\u0000\u0000"+
		"\u00b7\u00b9\u0003\u001c\u000e\u0000\u00b8\u00b6\u0001\u0000\u0000\u0000"+
		"\u00b9\u00bc\u0001\u0000\u0000\u0000\u00ba\u00b8\u0001\u0000\u0000\u0000"+
		"\u00ba\u00bb\u0001\u0000\u0000\u0000\u00bb\u001b\u0001\u0000\u0000\u0000"+
		"\u00bc\u00ba\u0001\u0000\u0000\u0000\u00bd\u00c2\u0003\u001e\u000f\u0000"+
		"\u00be\u00bf\u0007\u0001\u0000\u0000\u00bf\u00c1\u0003\u001e\u000f\u0000"+
		"\u00c0\u00be\u0001\u0000\u0000\u0000\u00c1\u00c4\u0001\u0000\u0000\u0000"+
		"\u00c2\u00c0\u0001\u0000\u0000\u0000\u00c2\u00c3\u0001\u0000\u0000\u0000"+
		"\u00c3\u001d\u0001\u0000\u0000\u0000\u00c4\u00c2\u0001\u0000\u0000\u0000"+
		"\u00c5\u00ca\u0003 \u0010\u0000\u00c6\u00c7\u0007\u0002\u0000\u0000\u00c7"+
		"\u00c9\u0003 \u0010\u0000\u00c8\u00c6\u0001\u0000\u0000\u0000\u00c9\u00cc"+
		"\u0001\u0000\u0000\u0000\u00ca\u00c8\u0001\u0000\u0000\u0000\u00ca\u00cb"+
		"\u0001\u0000\u0000\u0000\u00cb\u001f\u0001\u0000\u0000\u0000\u00cc\u00ca"+
		"\u0001\u0000\u0000\u0000\u00cd\u00d2\u0003\"\u0011\u0000\u00ce\u00cf\u0007"+
		"\u0003\u0000\u0000\u00cf\u00d1\u0003\"\u0011\u0000\u00d0\u00ce\u0001\u0000"+
		"\u0000\u0000\u00d1\u00d4\u0001\u0000\u0000\u0000\u00d2\u00d0\u0001\u0000"+
		"\u0000\u0000\u00d2\u00d3\u0001\u0000\u0000\u0000\u00d3!\u0001\u0000\u0000"+
		"\u0000\u00d4\u00d2\u0001\u0000\u0000\u0000\u00d5\u00da\u0003$\u0012\u0000"+
		"\u00d6\u00d7\u0007\u0004\u0000\u0000\u00d7\u00d9\u0003$\u0012\u0000\u00d8"+
		"\u00d6\u0001\u0000\u0000\u0000\u00d9\u00dc\u0001\u0000\u0000\u0000\u00da"+
		"\u00d8\u0001\u0000\u0000\u0000\u00da\u00db\u0001\u0000\u0000\u0000\u00db"+
		"#\u0001\u0000\u0000\u0000\u00dc\u00da\u0001\u0000\u0000\u0000\u00dd\u00df"+
		"\u0007\u0005\u0000\u0000\u00de\u00dd\u0001\u0000\u0000\u0000\u00de\u00df"+
		"\u0001\u0000\u0000\u0000\u00df\u00e0\u0001\u0000\u0000\u0000\u00e0\u00e1"+
		"\u0003&\u0013\u0000\u00e1%\u0001\u0000\u0000\u0000\u00e2\u00ec\u00052"+
		"\u0000\u0000\u00e3\u00ec\u0005/\u0000\u0000\u00e4\u00ec\u0005,\u0000\u0000"+
		"\u00e5\u00ec\u0003.\u0017\u0000\u00e6\u00e7\u0005\t\u0000\u0000\u00e7"+
		"\u00e8\u0003\u0018\f\u0000\u00e8\u00e9\u0005\n\u0000\u0000\u00e9\u00ec"+
		"\u0001\u0000\u0000\u0000\u00ea\u00ec\u0003*\u0015\u0000\u00eb\u00e2\u0001"+
		"\u0000\u0000\u0000\u00eb\u00e3\u0001\u0000\u0000\u0000\u00eb\u00e4\u0001"+
		"\u0000\u0000\u0000\u00eb\u00e5\u0001\u0000\u0000\u0000\u00eb\u00e6\u0001"+
		"\u0000\u0000\u0000\u00eb\u00ea\u0001\u0000\u0000\u0000\u00ec\'\u0001\u0000"+
		"\u0000\u0000\u00ed\u00f3\u0003*\u0015\u0000\u00ee\u00ef\u0005\u000b\u0000"+
		"\u0000\u00ef\u00f0\u0003\u0018\f\u0000\u00f0\u00f1\u0005\f\u0000\u0000"+
		"\u00f1\u00f3\u0001\u0000\u0000\u0000\u00f2\u00ed\u0001\u0000\u0000\u0000"+
		"\u00f2\u00ee\u0001\u0000\u0000\u0000\u00f3)\u0001\u0000\u0000\u0000\u00f4"+
		"\u00f5\u0005\u000b\u0000\u0000\u00f5\u00f6\u0005#\u0000\u0000\u00f6\u00f7"+
		"\u00050\u0000\u0000\u00f7\u0100\u0005\t\u0000\u0000\u00f8\u00fd\u0003"+
		"\u0018\f\u0000\u00f9\u00fa\u0005\"\u0000\u0000\u00fa\u00fc\u0003\u0018"+
		"\f\u0000\u00fb\u00f9\u0001\u0000\u0000\u0000\u00fc\u00ff\u0001\u0000\u0000"+
		"\u0000\u00fd\u00fb\u0001\u0000\u0000\u0000\u00fd\u00fe\u0001\u0000\u0000"+
		"\u0000\u00fe\u0101\u0001\u0000\u0000\u0000\u00ff\u00fd\u0001\u0000\u0000"+
		"\u0000\u0100\u00f8\u0001\u0000\u0000\u0000\u0100\u0101\u0001\u0000\u0000"+
		"\u0000\u0101\u0102\u0001\u0000\u0000\u0000\u0102\u0103\u0005\n\u0000\u0000"+
		"\u0103\u0104\u0005\f\u0000\u0000\u0104+\u0001\u0000\u0000\u0000\u0105"+
		"\u0107\u0005\u0001\u0000\u0000\u0106\u0105\u0001\u0000\u0000\u0000\u0107"+
		"\u0108\u0001\u0000\u0000\u0000\u0108\u0106\u0001\u0000\u0000\u0000\u0108"+
		"\u0109\u0001\u0000\u0000\u0000\u0109\u010b\u0001\u0000\u0000\u0000\u010a"+
		"\u010c\u0003\u0004\u0002\u0000\u010b\u010a\u0001\u0000\u0000\u0000\u010c"+
		"\u010d\u0001\u0000\u0000\u0000\u010d\u010b\u0001\u0000\u0000\u0000\u010d"+
		"\u010e\u0001\u0000\u0000\u0000\u010e\u0110\u0001\u0000\u0000\u0000\u010f"+
		"\u0111\u0005\u0002\u0000\u0000\u0110\u010f\u0001\u0000\u0000\u0000\u0111"+
		"\u0112\u0001\u0000\u0000\u0000\u0112\u0110\u0001\u0000\u0000\u0000\u0112"+
		"\u0113\u0001\u0000\u0000\u0000\u0113-\u0001\u0000\u0000\u0000\u0114\u0119"+
		"\u0005\u0004\u0000\u0000\u0115\u0118\u00030\u0018\u0000\u0116\u0118\u0003"+
		"(\u0014\u0000\u0117\u0115\u0001\u0000\u0000\u0000\u0117\u0116\u0001\u0000"+
		"\u0000\u0000\u0118\u011b\u0001\u0000\u0000\u0000\u0119\u0117\u0001\u0000"+
		"\u0000\u0000\u0119\u011a\u0001\u0000\u0000\u0000\u011a\u011c\u0001\u0000"+
		"\u0000\u0000\u011b\u0119\u0001\u0000\u0000\u0000\u011c\u011d\u0005\u0007"+
		"\u0000\u0000\u011d/\u0001\u0000\u0000\u0000\u011e\u011f\u0007\u0006\u0000"+
		"\u0000\u011f1\u0001\u0000\u0000\u0000\u0120\u0122\u0005%\u0000\u0000\u0121"+
		"\u0120\u0001\u0000\u0000\u0000\u0121\u0122\u0001\u0000\u0000\u0000\u0122"+
		"\u0123\u0001\u0000\u0000\u0000\u0123\u0124\u0003\u0018\f\u0000\u01243"+
		"\u0001\u0000\u0000\u0000\u001e7=IPU^eks\u008a\u008d\u00a4\u00ab\u00b2"+
		"\u00ba\u00c2\u00ca\u00d2\u00da\u00de\u00eb\u00f2\u00fd\u0100\u0108\u010d"+
		"\u0112\u0117\u0119\u0121";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}