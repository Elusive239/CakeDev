using System.Text;

namespace Cake;
public class Expr : Stmt { }

public class NotExpr : Expr
{
	public Expr expr = NilLiteral.NIL;
	public override string ToString()
	{
		return $"!({expr})";
	}
}

public class OperatorExpr : Expr
{
	public Expr left = NilLiteral.NIL;
	public Expr right = NilLiteral.NIL;
	public OperatorType opType;
	public override string ToString()
	{
		return $"{left} {Operator.OpToString(opType)} {right}";
	}
}

public class VariableExpr : Expr
{
	public string name = string.Empty;
	public override string ToString()
	{
		return $"`{name}`";
	}
}


public class ArrayExpr : Expr{
	public Expr[] arr = Array.Empty<Expr>();
	public ArrayExpr(Expr[] arr) => this.arr = arr;
	public override string ToString(){
		StringBuilder builder = new();
		builder.Append('[');
		for(int i = 0; i < arr.Length; i++)
			builder.Append($"{arr[i]}, ");
		builder.Append(']');
		return builder.ToString();
	}
}

public class ArrayAccessorExpr : Expr{
	public Expr left;
	public Expr accessor;
	public ArrayAccessorExpr(Expr left, Expr accessor){
		this.left = left;
		this.accessor = accessor;
	}
    public override string ToString()
    {
        return $"{left}[{accessor}]";
    }

	public Expr[] GetAccessors(){
		// int value = (int)((NumberLiteral<int> )accessor).value;
		if(left is not ArrayAccessorExpr){
			return new Expr[]{accessor};
		}else{
			List<Expr> others = new(((ArrayAccessorExpr)left).GetAccessors())
            {
                accessor
            };
			return others.ToArray();
		}
	}
}

public class StructExpr : Expr {
	public Dictionary<string, Expr> values;
	public StructExpr(Dictionary<string, Expr> values){
		this.values = values;
	}
	public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('{');
        foreach(var e in values){
            builder.Append($"{e.Key}:{e.Value}, ");
        }
        builder.Append('}');

        return builder.ToString();
    }
}

public class StructAccessorExpr : Expr{
	public Expr left;
	public Expr right;
	public StructAccessorExpr (Expr left, Expr right){
		this.left = left;
		this.right = right;
	}

    public override string ToString()
    {
        return $"{left}.{right}";
    }
}