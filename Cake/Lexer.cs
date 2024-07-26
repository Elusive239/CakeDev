using System.Text;
using static Cake.Util;
namespace Cake;

public class Lexer
{
	private readonly List<Token> tokens = new();
	private const string nums = "0123456789";
	private readonly string[] mathOperators = new string[] { "+", "-", "*", "/", "%" };
	private readonly string[] assignmentOperators = new string[] { "+=", "-=", "*=", "/=", "%=", "=" };
	private readonly string[] booleanOperators = new string[] { "==", "!=", "<", "<=", ">", ">=", "and", "or", "not" };
	//special keywords implemented like this so we dont need a  big chain of if statements!
	public Dictionary<string, Token> keywords = new(){ 
		{"True", new Token(TokenType.BOOL_LIT, BooleanLiteral.TRUE, 0)},
		{"False", new Token(TokenType.BOOL_LIT, BooleanLiteral.FALSE, 0)},
		{"Nil", new Token(TokenType.NIL_LIT, NilLiteral.NIL, 0)},
		{"def", new Token(TokenType.DEF, new StringLiteral("def"), 0)},
		{"not", new Token(TokenType.BOOL_OP, new StringLiteral("not"), 0)},
		{"and", new Token(TokenType.BOOL_OP, new StringLiteral("and"), 0)},
		{"or", new Token(TokenType.BOOL_OP, new StringLiteral("or"), 0)},
		{"if", new Token(TokenType.IF, new StringLiteral("if"), 0)},
		{"elif", new Token(TokenType.ELIF, new StringLiteral("elif"), 0)},
		{"else", new Token(TokenType.ELSE, new StringLiteral("else"), 0)},
		{"do", new Token(TokenType.DO, new StringLiteral("do"), 0)},
		{"while", new Token(TokenType.WHILE, new StringLiteral("while"), 0)},
		{"done", new Token(TokenType.DONE, new StringLiteral("done"), 0)},
		{"return", new Token(TokenType.RETURN, new StringLiteral("return"), 0)},
		{"assert", new Token(TokenType.ASSERT, new StringLiteral("assert"), 0)},
		{"fn", new Token(TokenType.FN, new StringLiteral("fn"), 0)},
		{"(" , new Token(TokenType.PAREN_LEFT, new StringLiteral("("), 0)},
		{")" , new Token(TokenType.PAREN_RIGHT, new StringLiteral(")"), 0)},
		{"[" , new Token(TokenType.BRACK_LEFT, new StringLiteral("["), 0)},
		{"]" , new Token(TokenType.BRACK_RIGHT, new StringLiteral("]"), 0)},
		{"{" , new Token(TokenType.BRACE_LEFT, new StringLiteral("{"), 0)},
		{"}" , new Token(TokenType.BRACE_RIGHT, new StringLiteral("}"), 0)},
		{".", new Token(TokenType.PERIOD, new StringLiteral("."), 0)},
		{",", new Token(TokenType.COMMA, new StringLiteral(","), 0)},
		{":", new Token(TokenType.COLON, new StringLiteral(":"), 0)},
		{"/", new Token(TokenType.IDENT,  new StringLiteral("SHOULDN'T GET HERE"), 0)}
	};
	public int lineNumber = 1;
	public Span<Token> Tokenize(ReadOnlySpan<char> span)
	{
		lineNumber = 1;
		for (int index = 0; index < span.Length; index++)
		{
			StringBuilder builder = new();
			if (span[index] == '/' && index < span.Length - 1 && span[index + 1] == '/')
			{
				while (index < span.Length && span[index] != '\n')
					index++;
			}
			else if (span[index] == ' ' || span[index] == '\t')
				continue;
			else if (span[index] == '\n')
			{
				lineNumber++;
				tokens.Add(new Token(TokenType.EOL, new StringLiteral("\\n"), lineNumber));
				continue;
			}
			else if (span[index] == '\r')
				continue;
			else if (nums.Contains(span[index]))
			{
				bool isFloat = false;
				char current = span[index];
				while (nums.Contains(current) && index < span.Length)
				{
					if (!isFloat && current == '.')
					{
						isFloat = true;
						index++;
						continue;
					}
					else if (isFloat && current == '.')
					{
						throw ERROR("Number Literal contains excess decimal points.");
					}
					builder.Append(current);
					index++;
					if (index >= span.Length) break;

					current = span[index];
				}
				if (isFloat)
					tokens.Add(NewNumber(float.Parse(builder.ToString())));
				else
					tokens.Add(NewNumber(int.Parse(builder.ToString())));
				index--;
			}
			else if (span[index].Equals('\"'))
			{
				index++;
				while (index < span.Length && span[index] != '\"')
				{
					char current = span[index++];
					builder.Append(current);
				}
				tokens.Add(NewString(builder.ToString()));
			}
			else if (IsOperator($"{span[index]}") || index + 1 < span.Length && IsOperator($"{span[index]}{span[index + 1]}"))
			{
				string op = $"{span[index]}";
				if (index < span.Length && IsOperator($"{op}{span[index + 1]}"))
				{
					index++;
					op += span[index++];
				}
				if (mathOperators.Contains(op))
					tokens.Add(new Token(TokenType.MATH_OP, new StringLiteral(op), lineNumber));
				else if (assignmentOperators.Contains(op))
					tokens.Add(new Token(TokenType.ASS_OP, new StringLiteral(op), lineNumber));
				else if (booleanOperators.Contains(op))
					tokens.Add(new Token(TokenType.BOOL_OP, new StringLiteral(op), lineNumber));
				else
					throw ERROR($"Unknown operator {op} at {lineNumber}.");
			}
			else if (keywords.ContainsKey(span[index]+"")){
				tokens.Add(keywords[span[index]+""].Copy(lineNumber) );
			}
			else
			{
				//handle identifiers
				//if not a keyword (def, for, etc) add as identifier
				//otherwise if literal add as specific literal (Nil)
				//otherwise add as identifier

				while (index < span.Length && span[index] != ' ' && span[index] != '\n' 
				&& span[index] != '\r' && !keywords.ContainsKey($"{span[index]}"))
				{
					builder.Append(span[index++]);
				}
				index--;

				string str = builder.ToString();

				if (keywords.ContainsKey(str)){
					tokens.Add(keywords[str].Copy(lineNumber) );
					continue;
				}
				else
				{
					tokens.Add(new Token(TokenType.IDENT, new StringLiteral(str), lineNumber));
					continue;
				}
				throw ERROR($"\'{span[index]}\' at Line {lineNumber} is an invalid token or is unimplemented.");
			}
		}
		tokens.Add(NewEOF());
		return tokens.ToArray().AsSpan();
	}

	public bool IsOperator(string v) => mathOperators.Contains(v) || assignmentOperators.Contains(v) || booleanOperators.Contains(v);

	public Token NewNumber(int num) => new(TokenType.NUM_LIT, new NumberLiteral<int>(num), lineNumber);

	public Token NewNumber(float num) => new(TokenType.NUM_LIT, new NumberLiteral<float>(num), lineNumber);

	public Token NewString(string str) => new(TokenType.STR_LIT, new StringLiteral(str), lineNumber);

	public Token NewEOF() => new(TokenType.EOF, new StringLiteral("End Of File"), lineNumber);

}