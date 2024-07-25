namespace Cake;

public class Stmt { }

public class Main : Stmt
{
    public readonly BodyStmt expr;
	public Main()
	{
		expr = new BodyStmt(Array.Empty<Stmt>());
	}
	public Main(Stmt[] body)
	{
		this.expr = new BodyStmt(body);
	}
	public override string ToString()
	{
		return expr.ToString();
	}
}

public class ReturnStmt : Stmt {
	public Expr returnValue;
	public ReturnStmt() => returnValue = NilLiteral.NIL;
	public ReturnStmt(Expr returnValue) => this.returnValue = returnValue;

    public override string ToString()
    {
        return $"return {returnValue}";
    }
}

public class AssertStmt : Stmt{
	public Expr condition;
	public Expr message;
	public AssertStmt(Expr? expr){
		condition = expr ?? NilLiteral.NIL;
		message = NilLiteral.NIL;
	}
    public override string ToString()
    {
		if(message != null && !message.Equals(NilLiteral.NIL) && !((StringLiteral)message).value.Equals(string.Empty))
			return $"Assertion | {condition} | {message}";
        return $"Assertion | {condition} ";
    }
}

public class VariableDeclarationStmt : Stmt
{
	public string name = string.Empty;
	public Expr expr = NilLiteral.NIL;
	public override string? ToString()
	{
		return $"def {name}: {expr}";
	}
}

public class IfStmt : Stmt{
	public Expr condition;
	public BodyStmt body;
	public Stmt next;

	public IfStmt(BodyStmt body, Expr? condition, Stmt? next = null){
		this.next = next ?? NilLiteral.NIL;
		this.condition = condition ?? NilLiteral.NIL;
		this.body = body;
	}

    public override string ToString()
    {
		if(next == NilLiteral.NIL){
			return $"if {condition} do {body}";
		}

		return $"if {condition} do {body} -- el{next}";
    }
}

public class WhileStmt : Stmt{
	public Expr cond;
	public BodyStmt body;
	public WhileStmt(Expr cond, BodyStmt body){
		this.cond = cond;
		this.body = body;
	}
}

public class BodyStmt : Stmt{
	public Stmt[] body;
	public BodyStmt(Stmt[] _body) => body = _body;

	public override string ToString()
	{
		System.Text.StringBuilder builder = new();

		foreach (var item in body)
		{
			builder.Append(item);
			if (item != body[^1])
				builder.Append(", ");
		}

		return builder.ToString();
	}
}