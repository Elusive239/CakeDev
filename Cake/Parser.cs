using static Cake.Util;
using System.Text;
using System.Security.Cryptography.X509Certificates;
namespace Cake;

public class Parser
{

	Token[] Tokens { get; }
	int index = 0;
	public Parser(Token[] tokens)
	{
		Tokens = tokens;
	}

	Token Peek()
	{
		return Tokens[index];
	}

	Token Consume()
	{
		return Tokens[index++];
	}

	public Main Parse()
	{
		if (Tokens.Length <= 1) return new Main();

		List<Stmt> body = new();

		while (Peek().typ != TokenType.EOF)
		{
			if(Peek().typ == TokenType.EOL){
				Consume();
				continue;
			}			
			body.Add(ParseStmt());
		}

		return new Main(body.ToArray());
	}

	Stmt ParseStmt()
	{
		if(Peek().typ == TokenType.EOL){
			Consume();
			return ParseStmt();
		}

		if (Peek().typ == TokenType.DEF)
		{
			Consume();
			if (Peek().typ != TokenType.IDENT)
			{
				throw ERROR("Keyword \'def\' used without corresponding identifier.");
			}
			Token ident = Consume();
			if (Peek().typ != TokenType.ASS_OP && !Peek().val.Equals("="))
			{
				throw ERROR("Variable declared but not assigned");
			}
			Consume();
			Expr expr = ParseExpr();
			return new VariableDeclarationExpr()
			{
				expr = expr,
				name = ((StringLiteral)ident.val).value
			};

		}

		if(Peek().typ == TokenType.RETURN){
			Consume();
			if(Peek().typ == TokenType.EOL)
				return new ReturnStmt();
			return new ReturnStmt(ParseExpr());
		}
		
		if(Peek().typ == TokenType.IF){
			return ParseIfStmt();
		}

		if(Peek().typ == TokenType.ASSERT){
			Consume();
			AssertStmt stmt =  new(ParseExpr());
			if(Peek().typ == TokenType.COMMA){
				Consume();
				stmt.message = ParseExpr();
			}
			return stmt;
		}

		if(Peek().typ == TokenType.WHILE){
			Consume();
			Expr condition = ParseExpr();
			if(Peek().typ != TokenType.DO){
				throw ERROR("No do after loop declaration found.");
			} Consume();
			
			//parse while section
			BodyStmt body = (BodyStmt) ParseBody();

			if(Peek().typ != TokenType.DONE){
				throw ERROR("Loop is never closed.");
			} Consume();
			return new WhileStmt(condition, body);
		}

		return ParseExpr();
	}

	Stmt ParseIfStmt(){
		Token token = Consume();
		Expr condition = BooleanLiteral.TRUE;// = token.typ != TokenType.ELSE ? (OperatorExpr)ParseOperatorExpr() : NilLiteral.NIL;
		
		if(token.typ == TokenType.IF || token.typ == TokenType.ELIF){
			condition = ParseExpr();
		}

		//parse body
		if(Peek().typ != TokenType.DO){
			throw ERROR("if statement not opened properly; missing \'do\' after if statement.");
		} Consume();

		while(Peek().typ == TokenType.EOL) Consume();
		
		BodyStmt body = (BodyStmt) ParseBody();

		if(token.typ == TokenType.IF || token.typ == TokenType.ELIF){
			if(Peek().typ == TokenType.ELIF || Peek().typ == TokenType.ELSE){
				IfStmt ifstmt = new(body, condition, ParseIfStmt());
				return ifstmt;
			}
			else if (Peek().typ != TokenType.DONE){
				throw ERROR("if statement not closed properly; missing \'done\' at end of if statement chain.");
			}
			Consume();
			IfStmt stmt = new( body, condition, NilLiteral.NIL);
			return stmt;
		}else if(token.typ == TokenType.ELSE){
			if (Peek().typ != TokenType.DONE){
				throw ERROR("if statement not closed properly; missing \'done\' at end of if statement chain.");
			}
			Consume();
			IfStmt stmt = new( body, condition, NilLiteral.NIL);
			return stmt;
		}
		return NilLiteral.NIL;
	}

	Stmt ParseBody(){
		List<Stmt> body = new();

		while (true )
		{
			while(Peek().typ == TokenType.EOL) Consume();
			if(Peek().typ == TokenType.EOF || Peek().typ == TokenType.ELIF ||Peek().typ == TokenType.ELSE || Peek().typ == TokenType.DONE)
				break;
			Stmt stmt = ParseStmt();
			body.Add(stmt);
		}

		return new BodyStmt(body.ToArray());
	}

	Expr ParseExpr()
	{
		return ParseOperatorExpr();
	}

	Expr ParseOperatorExpr()
	{
		if (Peek().val is StringLiteral literal && literal.value.Equals("not"))
		{
			Consume();
			NotExpr expr = new()
			{
				expr = ParseExpr()
			};
			return expr;
		}

		Expr left = ParseValueExpr();

		while (Peek().typ != TokenType.EOL)
			if (Peek().typ == TokenType.MATH_OP || Peek().typ == TokenType.ASS_OP || Peek().typ == TokenType.BOOL_OP && !Peek().val.Equals("not"))
			{
				Token token = Consume();
				Expr right = ParseOperatorExpr();
				left = new OperatorExpr()
				{
					left = left,
					right = right,
					opType = Operator.OpFromString(((StringLiteral)token.val).value)
				};
			}
			else break;
		if (Peek().typ == TokenType.EOL)
			Consume();

		return left;
	}

	Expr ParseValueExpr()
	{
		Token token = Consume();

		switch (token.typ)
		{
			case TokenType.BRACK_LEFT:
				List<Expr> expressions = new();
				while(Peek().typ != TokenType.EOF && Peek().typ != TokenType.BRACK_RIGHT){
					expressions.Add(ParseExpr());
					if(Peek().typ == TokenType.COMMA){
						Consume();
						continue;
					}
					break;
				}
				if(Peek().typ != TokenType.BRACK_RIGHT){
					throw ERROR("Improperly closed Array.");
				}Consume();
				Expr left = new ArrayExpr(expressions.ToArray());

				if(Peek().typ == TokenType.BRACK_LEFT){
					while(Peek().typ == TokenType.BRACK_LEFT){
						Consume();
						Expr accessor = ParseExpr();
						if(Peek().typ != TokenType.BRACK_RIGHT){
							throw ERROR("Improperly closed Array accessor.");
						}Consume();
						left = new ArrayAccessorExpr(left, accessor);
					}
				}

				return left;
			case TokenType.PAREN_LEFT:
				Expr val = ParseExpr();

				if (Peek().typ != TokenType.PAREN_RIGHT)
				{
					throw ERROR($"Token {token.val} never closed at line {token.lineNumber}");
				}
				Consume();
				return val;
			case TokenType.NUM_LIT:
				return (Expr)token.val;
			case TokenType.STR_LIT:
				return (StringLiteral)token.val;
			case TokenType.NIL_LIT:
				return NilLiteral.NIL;
			case TokenType.BOOL_LIT:
				return (BooleanLiteral)token.val;
			case TokenType.IDENT:
				if(Peek().typ == TokenType.ASS_OP){
					OperatorExpr expr = new(){
						left = (StringLiteral)token.val,
						opType = Operator.OpFromString(((StringLiteral)Consume().val).value),
						right = ParseExpr(),
					};
					return expr;
				}
				Expr lft = new VariableExpr() { name = ((StringLiteral)token.val).value };
				while(Peek().typ == TokenType.BRACK_LEFT){
					Consume();
					Expr accessor = ParseExpr();
					if(Peek().typ != TokenType.BRACK_RIGHT){
						throw ERROR("Improperly closed Array accessor.");
					}Consume();
					lft = new ArrayAccessorExpr(lft, accessor);
				}
				return lft;
			default:
				break;
		}
		throw ERROR($"Unimplemented Token {token} in \'ParseValueExpr\'");
	}
}
