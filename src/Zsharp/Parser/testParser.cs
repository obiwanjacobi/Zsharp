//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from c:\Users\marc\Documents\MyProjects\Zalt\Zsharp\Zsharp\test.g4_ by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class testParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		ALPHA=1, PARENopen=2, PARENclose=3, COLON=4, COMMA=5, SP=6, EOL=7;
	public const int
		RULE_run = 0, RULE_code = 1, RULE_fn_def = 2, RULE_fn_type = 3, RULE_fn_param_list = 4, 
		RULE_fn_param = 5, RULE_var_def = 6, RULE_type = 7, RULE_identifier = 8;
	public static readonly string[] ruleNames = {
		"run", "code", "fn_def", "fn_type", "fn_param_list", "fn_param", "var_def", 
		"type", "identifier"
	};

	private static readonly string[] _LiteralNames = {
		null, null, "'('", "')'", "':'", "','", "' '"
	};
	private static readonly string[] _SymbolicNames = {
		null, "ALPHA", "PARENopen", "PARENclose", "COLON", "COMMA", "SP", "EOL"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "test.g4_"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static testParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public testParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public testParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class RunContext : ParserRuleContext {
		public CodeContext[] code() {
			return GetRuleContexts<CodeContext>();
		}
		public CodeContext code(int i) {
			return GetRuleContext<CodeContext>(i);
		}
		public ITerminalNode[] EOL() { return GetTokens(testParser.EOL); }
		public ITerminalNode EOL(int i) {
			return GetToken(testParser.EOL, i);
		}
		public ITerminalNode Eof() { return GetToken(testParser.Eof, 0); }
		public RunContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_run; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitRun(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public RunContext run() {
		RunContext _localctx = new RunContext(Context, State);
		EnterRule(_localctx, 0, RULE_run);
		int _la;
		try {
			State = 27;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,1,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 23;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==ALPHA) {
					{
					{
					State = 18; code();
					State = 19; Match(EOL);
					}
					}
					State = 25;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 26; Match(Eof);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class CodeContext : ParserRuleContext {
		public Fn_defContext fn_def() {
			return GetRuleContext<Fn_defContext>(0);
		}
		public Var_defContext var_def() {
			return GetRuleContext<Var_defContext>(0);
		}
		public CodeContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_code; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitCode(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public CodeContext code() {
		CodeContext _localctx = new CodeContext(Context, State);
		EnterRule(_localctx, 2, RULE_code);
		try {
			State = 31;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,2,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 29; fn_def();
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 30; var_def();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Fn_defContext : ParserRuleContext {
		public IdentifierContext identifier() {
			return GetRuleContext<IdentifierContext>(0);
		}
		public ITerminalNode COLON() { return GetToken(testParser.COLON, 0); }
		public ITerminalNode SP() { return GetToken(testParser.SP, 0); }
		public Fn_typeContext fn_type() {
			return GetRuleContext<Fn_typeContext>(0);
		}
		public Fn_defContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_fn_def; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitFn_def(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Fn_defContext fn_def() {
		Fn_defContext _localctx = new Fn_defContext(Context, State);
		EnterRule(_localctx, 4, RULE_fn_def);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 33; identifier();
			State = 34; Match(COLON);
			State = 35; Match(SP);
			State = 36; fn_type();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Fn_typeContext : ParserRuleContext {
		public ITerminalNode PARENopen() { return GetToken(testParser.PARENopen, 0); }
		public ITerminalNode PARENclose() { return GetToken(testParser.PARENclose, 0); }
		public Fn_param_listContext fn_param_list() {
			return GetRuleContext<Fn_param_listContext>(0);
		}
		public ITerminalNode COLON() { return GetToken(testParser.COLON, 0); }
		public ITerminalNode SP() { return GetToken(testParser.SP, 0); }
		public TypeContext type() {
			return GetRuleContext<TypeContext>(0);
		}
		public Fn_typeContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_fn_type; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitFn_type(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Fn_typeContext fn_type() {
		Fn_typeContext _localctx = new Fn_typeContext(Context, State);
		EnterRule(_localctx, 6, RULE_fn_type);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 38; Match(PARENopen);
			State = 40;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==ALPHA) {
				{
				State = 39; fn_param_list();
				}
			}

			State = 42; Match(PARENclose);
			State = 46;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==COLON) {
				{
				State = 43; Match(COLON);
				State = 44; Match(SP);
				State = 45; type();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Fn_param_listContext : ParserRuleContext {
		public Fn_paramContext[] fn_param() {
			return GetRuleContexts<Fn_paramContext>();
		}
		public Fn_paramContext fn_param(int i) {
			return GetRuleContext<Fn_paramContext>(i);
		}
		public ITerminalNode[] COMMA() { return GetTokens(testParser.COMMA); }
		public ITerminalNode COMMA(int i) {
			return GetToken(testParser.COMMA, i);
		}
		public ITerminalNode[] SP() { return GetTokens(testParser.SP); }
		public ITerminalNode SP(int i) {
			return GetToken(testParser.SP, i);
		}
		public Fn_param_listContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_fn_param_list; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitFn_param_list(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Fn_param_listContext fn_param_list() {
		Fn_param_listContext _localctx = new Fn_param_listContext(Context, State);
		EnterRule(_localctx, 8, RULE_fn_param_list);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 48; fn_param();
			State = 54;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==COMMA) {
				{
				{
				State = 49; Match(COMMA);
				State = 50; Match(SP);
				State = 51; fn_param();
				}
				}
				State = 56;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Fn_paramContext : ParserRuleContext {
		public IdentifierContext identifier() {
			return GetRuleContext<IdentifierContext>(0);
		}
		public ITerminalNode COLON() { return GetToken(testParser.COLON, 0); }
		public ITerminalNode SP() { return GetToken(testParser.SP, 0); }
		public TypeContext type() {
			return GetRuleContext<TypeContext>(0);
		}
		public Fn_paramContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_fn_param; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitFn_param(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Fn_paramContext fn_param() {
		Fn_paramContext _localctx = new Fn_paramContext(Context, State);
		EnterRule(_localctx, 10, RULE_fn_param);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 57; identifier();
			State = 58; Match(COLON);
			State = 59; Match(SP);
			State = 60; type();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Var_defContext : ParserRuleContext {
		public IdentifierContext identifier() {
			return GetRuleContext<IdentifierContext>(0);
		}
		public ITerminalNode COLON() { return GetToken(testParser.COLON, 0); }
		public ITerminalNode SP() { return GetToken(testParser.SP, 0); }
		public TypeContext type() {
			return GetRuleContext<TypeContext>(0);
		}
		public Var_defContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_var_def; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitVar_def(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Var_defContext var_def() {
		Var_defContext _localctx = new Var_defContext(Context, State);
		EnterRule(_localctx, 12, RULE_var_def);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 62; identifier();
			State = 63; Match(COLON);
			State = 64; Match(SP);
			State = 65; type();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class TypeContext : ParserRuleContext {
		public ITerminalNode[] ALPHA() { return GetTokens(testParser.ALPHA); }
		public ITerminalNode ALPHA(int i) {
			return GetToken(testParser.ALPHA, i);
		}
		public TypeContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_type; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitType(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public TypeContext type() {
		TypeContext _localctx = new TypeContext(Context, State);
		EnterRule(_localctx, 14, RULE_type);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 68;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			do {
				{
				{
				State = 67; Match(ALPHA);
				}
				}
				State = 70;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			} while ( _la==ALPHA );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class IdentifierContext : ParserRuleContext {
		public ITerminalNode[] ALPHA() { return GetTokens(testParser.ALPHA); }
		public ITerminalNode ALPHA(int i) {
			return GetToken(testParser.ALPHA, i);
		}
		public IdentifierContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_identifier; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ItestVisitor<TResult> typedVisitor = visitor as ItestVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitIdentifier(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public IdentifierContext identifier() {
		IdentifierContext _localctx = new IdentifierContext(Context, State);
		EnterRule(_localctx, 16, RULE_identifier);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 73;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			do {
				{
				{
				State = 72; Match(ALPHA);
				}
				}
				State = 75;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			} while ( _la==ALPHA );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\t', 'P', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', '\t', 
		'\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x4', '\x6', 
		'\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', '\t', '\b', '\x4', 
		'\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x3', '\x2', '\x3', '\x2', 
		'\x3', '\x2', '\a', '\x2', '\x18', '\n', '\x2', '\f', '\x2', '\xE', '\x2', 
		'\x1B', '\v', '\x2', '\x3', '\x2', '\x5', '\x2', '\x1E', '\n', '\x2', 
		'\x3', '\x3', '\x3', '\x3', '\x5', '\x3', '\"', '\n', '\x3', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', 
		'\x3', '\x5', '\x5', '\x5', '+', '\n', '\x5', '\x3', '\x5', '\x3', '\x5', 
		'\x3', '\x5', '\x3', '\x5', '\x5', '\x5', '\x31', '\n', '\x5', '\x3', 
		'\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\a', '\x6', '\x37', 
		'\n', '\x6', '\f', '\x6', '\xE', '\x6', ':', '\v', '\x6', '\x3', '\a', 
		'\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\b', '\x3', 
		'\b', '\x3', '\b', '\x3', '\b', '\x3', '\b', '\x3', '\t', '\x6', '\t', 
		'G', '\n', '\t', '\r', '\t', '\xE', '\t', 'H', '\x3', '\n', '\x6', '\n', 
		'L', '\n', '\n', '\r', '\n', '\xE', '\n', 'M', '\x3', '\n', '\x2', '\x2', 
		'\v', '\x2', '\x4', '\x6', '\b', '\n', '\f', '\xE', '\x10', '\x12', '\x2', 
		'\x2', '\x2', 'N', '\x2', '\x1D', '\x3', '\x2', '\x2', '\x2', '\x4', '!', 
		'\x3', '\x2', '\x2', '\x2', '\x6', '#', '\x3', '\x2', '\x2', '\x2', '\b', 
		'(', '\x3', '\x2', '\x2', '\x2', '\n', '\x32', '\x3', '\x2', '\x2', '\x2', 
		'\f', ';', '\x3', '\x2', '\x2', '\x2', '\xE', '@', '\x3', '\x2', '\x2', 
		'\x2', '\x10', '\x46', '\x3', '\x2', '\x2', '\x2', '\x12', 'K', '\x3', 
		'\x2', '\x2', '\x2', '\x14', '\x15', '\x5', '\x4', '\x3', '\x2', '\x15', 
		'\x16', '\a', '\t', '\x2', '\x2', '\x16', '\x18', '\x3', '\x2', '\x2', 
		'\x2', '\x17', '\x14', '\x3', '\x2', '\x2', '\x2', '\x18', '\x1B', '\x3', 
		'\x2', '\x2', '\x2', '\x19', '\x17', '\x3', '\x2', '\x2', '\x2', '\x19', 
		'\x1A', '\x3', '\x2', '\x2', '\x2', '\x1A', '\x1E', '\x3', '\x2', '\x2', 
		'\x2', '\x1B', '\x19', '\x3', '\x2', '\x2', '\x2', '\x1C', '\x1E', '\a', 
		'\x2', '\x2', '\x3', '\x1D', '\x19', '\x3', '\x2', '\x2', '\x2', '\x1D', 
		'\x1C', '\x3', '\x2', '\x2', '\x2', '\x1E', '\x3', '\x3', '\x2', '\x2', 
		'\x2', '\x1F', '\"', '\x5', '\x6', '\x4', '\x2', ' ', '\"', '\x5', '\xE', 
		'\b', '\x2', '!', '\x1F', '\x3', '\x2', '\x2', '\x2', '!', ' ', '\x3', 
		'\x2', '\x2', '\x2', '\"', '\x5', '\x3', '\x2', '\x2', '\x2', '#', '$', 
		'\x5', '\x12', '\n', '\x2', '$', '%', '\a', '\x6', '\x2', '\x2', '%', 
		'&', '\a', '\b', '\x2', '\x2', '&', '\'', '\x5', '\b', '\x5', '\x2', '\'', 
		'\a', '\x3', '\x2', '\x2', '\x2', '(', '*', '\a', '\x4', '\x2', '\x2', 
		')', '+', '\x5', '\n', '\x6', '\x2', '*', ')', '\x3', '\x2', '\x2', '\x2', 
		'*', '+', '\x3', '\x2', '\x2', '\x2', '+', ',', '\x3', '\x2', '\x2', '\x2', 
		',', '\x30', '\a', '\x5', '\x2', '\x2', '-', '.', '\a', '\x6', '\x2', 
		'\x2', '.', '/', '\a', '\b', '\x2', '\x2', '/', '\x31', '\x5', '\x10', 
		'\t', '\x2', '\x30', '-', '\x3', '\x2', '\x2', '\x2', '\x30', '\x31', 
		'\x3', '\x2', '\x2', '\x2', '\x31', '\t', '\x3', '\x2', '\x2', '\x2', 
		'\x32', '\x38', '\x5', '\f', '\a', '\x2', '\x33', '\x34', '\a', '\a', 
		'\x2', '\x2', '\x34', '\x35', '\a', '\b', '\x2', '\x2', '\x35', '\x37', 
		'\x5', '\f', '\a', '\x2', '\x36', '\x33', '\x3', '\x2', '\x2', '\x2', 
		'\x37', ':', '\x3', '\x2', '\x2', '\x2', '\x38', '\x36', '\x3', '\x2', 
		'\x2', '\x2', '\x38', '\x39', '\x3', '\x2', '\x2', '\x2', '\x39', '\v', 
		'\x3', '\x2', '\x2', '\x2', ':', '\x38', '\x3', '\x2', '\x2', '\x2', ';', 
		'<', '\x5', '\x12', '\n', '\x2', '<', '=', '\a', '\x6', '\x2', '\x2', 
		'=', '>', '\a', '\b', '\x2', '\x2', '>', '?', '\x5', '\x10', '\t', '\x2', 
		'?', '\r', '\x3', '\x2', '\x2', '\x2', '@', '\x41', '\x5', '\x12', '\n', 
		'\x2', '\x41', '\x42', '\a', '\x6', '\x2', '\x2', '\x42', '\x43', '\a', 
		'\b', '\x2', '\x2', '\x43', '\x44', '\x5', '\x10', '\t', '\x2', '\x44', 
		'\xF', '\x3', '\x2', '\x2', '\x2', '\x45', 'G', '\a', '\x3', '\x2', '\x2', 
		'\x46', '\x45', '\x3', '\x2', '\x2', '\x2', 'G', 'H', '\x3', '\x2', '\x2', 
		'\x2', 'H', '\x46', '\x3', '\x2', '\x2', '\x2', 'H', 'I', '\x3', '\x2', 
		'\x2', '\x2', 'I', '\x11', '\x3', '\x2', '\x2', '\x2', 'J', 'L', '\a', 
		'\x3', '\x2', '\x2', 'K', 'J', '\x3', '\x2', '\x2', '\x2', 'L', 'M', '\x3', 
		'\x2', '\x2', '\x2', 'M', 'K', '\x3', '\x2', '\x2', '\x2', 'M', 'N', '\x3', 
		'\x2', '\x2', '\x2', 'N', '\x13', '\x3', '\x2', '\x2', '\x2', '\n', '\x19', 
		'\x1D', '!', '*', '\x30', '\x38', 'H', 'M',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
