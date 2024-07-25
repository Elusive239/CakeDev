using System.Numerics;
using System.Text;
namespace Cake;
public interface ITokenLiteral { }

public class NumberLiteral<T> : Expr, ITokenLiteral where T : INumber<T>
{
    public INumber<T> value;
    public NumberLiteral(INumber<T> val)
    {
        value = val;
    }
    public override string ToString()
    {
        return $"{value}";
    }
    public override bool Equals(object? obj)
    {
        if (obj is NumberLiteral<int> intVal)
        {
            return intVal.value == this.value;
        }
        else if (obj is NumberLiteral<float> floatVal)
        {
            return floatVal.value == this.value;
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

public class StringLiteral : Expr, ITokenLiteral
{
    public string value = string.Empty;
    public StringLiteral(string val)
    {
        value = val;
    }
    public override string ToString()
    {
        return $"\'{value}\'";
    }
}

public class NilLiteral : Expr, ITokenLiteral
{
    public static readonly NilLiteral NIL = new();

    public override string ToString() => "Nil";
    public static NumberLiteral<int> ToNumber() => new(0);
    public static BooleanLiteral ToBool() => BooleanLiteral.FALSE;

    public override bool Equals(object? obj)
    {
        return obj == null || obj.GetType() == this.GetType();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

public class BooleanLiteral : Expr, ITokenLiteral
{
    public static readonly BooleanLiteral TRUE;
    public static readonly BooleanLiteral FALSE;
    static BooleanLiteral(){
        TRUE = new BooleanLiteral(true){};
        FALSE = new BooleanLiteral(false){};
    }

    public BooleanLiteral(bool value){
        this.value = value;
    }

    public bool value = false;
    public override string ToString() => value ? "True" : "False";
    public NumberLiteral<int> ToNumber() => value ? new NumberLiteral<int>(1) : new NumberLiteral<int>(0);
    public override bool Equals(object? obj)
    {
        return obj != null && 
        obj is BooleanLiteral literal && 
        this.value == literal.value ;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

public class ArrayLiteral : ITokenLiteral{
	public ITokenLiteral[] literals = Array.Empty<ITokenLiteral>();
	public ArrayLiteral(ITokenLiteral[] lits){
		literals = lits;
	}
    public override string ToString(){
		StringBuilder builder = new();
		builder.Append('[');
		for(int i = 0; i < literals.Length; i++)
			builder.Append($"{literals[i]}, ");
		builder.Append(']');
		return builder.ToString();
	}
}

public class StructLiteral : ITokenLiteral {
	public Dictionary<string, ITokenLiteral> values;
	public StructLiteral(Dictionary<string, ITokenLiteral> values){
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