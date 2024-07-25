namespace Cake
{
	public enum TokenType
	{
		IDENT, COMMA, PERIOD,
		//Keywords
		DEF, IF, ELIF, ELSE, DO, DONE, RETURN, ASSERT, WHILE,
		//Literals
		NUM_LIT, STR_LIT, NIL_LIT, BOOL_LIT,
		//Operators
		MATH_OP, ASS_OP, BOOL_OP,
		PAREN_LEFT, PAREN_RIGHT,
		BRACK_LEFT, BRACK_RIGHT,
		//End
		EOL, EOF
	}

	public class Token
	{
		public TokenType typ;
		public ITokenLiteral val;
		public int lineNumber;
		public Token(TokenType typ, ITokenLiteral val, int lineNumber)
		{
			this.typ = typ;
			this.val = val;
			this.lineNumber = lineNumber;
		}

		public Token Copy(int lineNumber = -1){
			if(lineNumber < 0)
				lineNumber = this.lineNumber;
			return new Token(this.typ, this.val, lineNumber);
		}

		public override string ToString()
		{
			return $"Line: {lineNumber} => Type: {typ}, Value: {val}";
		}
	}
}