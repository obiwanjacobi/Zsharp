//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from c:\Users\marc\Documents\MyProjects\Zalt\Zlang.NET\Zlang.NET\test.g4_ by ANTLR 4.8

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
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class testLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		ALPHA=1, PARENopen=2, PARENclose=3, COLON=4, COMMA=5, SP=6, EOL=7;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"ALPHA", "PARENopen", "PARENclose", "COLON", "COMMA", "SP", "EOL"
	};


	public testLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public testLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

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

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static testLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\t', '$', '\b', '\x1', '\x4', '\x2', '\t', '\x2', '\x4', 
		'\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', 
		'\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', '\t', 
		'\b', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x5', '\x3', '\x5', '\x3', '\x6', '\x3', '\x6', 
		'\x3', '\a', '\x3', '\a', '\x3', '\b', '\x5', '\b', '\x1F', '\n', '\b', 
		'\x3', '\b', '\x3', '\b', '\x5', '\b', '#', '\n', '\b', '\x2', '\x2', 
		'\t', '\x3', '\x3', '\x5', '\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', 
		'\r', '\b', '\xF', '\t', '\x3', '\x2', '\x3', '\x4', '\x2', '\x43', '\\', 
		'\x63', '|', '\x2', '%', '\x2', '\x3', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x2', '\xF', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '\x11', '\x3', '\x2', '\x2', '\x2', '\x5', '\x13', 
		'\x3', '\x2', '\x2', '\x2', '\a', '\x15', '\x3', '\x2', '\x2', '\x2', 
		'\t', '\x17', '\x3', '\x2', '\x2', '\x2', '\v', '\x19', '\x3', '\x2', 
		'\x2', '\x2', '\r', '\x1B', '\x3', '\x2', '\x2', '\x2', '\xF', '\"', '\x3', 
		'\x2', '\x2', '\x2', '\x11', '\x12', '\t', '\x2', '\x2', '\x2', '\x12', 
		'\x4', '\x3', '\x2', '\x2', '\x2', '\x13', '\x14', '\a', '*', '\x2', '\x2', 
		'\x14', '\x6', '\x3', '\x2', '\x2', '\x2', '\x15', '\x16', '\a', '+', 
		'\x2', '\x2', '\x16', '\b', '\x3', '\x2', '\x2', '\x2', '\x17', '\x18', 
		'\a', '<', '\x2', '\x2', '\x18', '\n', '\x3', '\x2', '\x2', '\x2', '\x19', 
		'\x1A', '\a', '.', '\x2', '\x2', '\x1A', '\f', '\x3', '\x2', '\x2', '\x2', 
		'\x1B', '\x1C', '\a', '\"', '\x2', '\x2', '\x1C', '\xE', '\x3', '\x2', 
		'\x2', '\x2', '\x1D', '\x1F', '\a', '\xF', '\x2', '\x2', '\x1E', '\x1D', 
		'\x3', '\x2', '\x2', '\x2', '\x1E', '\x1F', '\x3', '\x2', '\x2', '\x2', 
		'\x1F', ' ', '\x3', '\x2', '\x2', '\x2', ' ', '#', '\a', '\f', '\x2', 
		'\x2', '!', '#', '\a', '\xF', '\x2', '\x2', '\"', '\x1E', '\x3', '\x2', 
		'\x2', '\x2', '\"', '!', '\x3', '\x2', '\x2', '\x2', '#', '\x10', '\x3', 
		'\x2', '\x2', '\x2', '\x5', '\x2', '\x1E', '\"', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
