using System.Text;
using static Cake.Util;
namespace Cake;
public class Evaluator
{
    private readonly Dictionary<string, ITokenLiteral> variables;
	private bool exit = false;
	private int exitCode = 0;
	public Evaluator()
	{
		variables = new();
	}

    public override string ToString()
    {
		StringBuilder stringBuilder = new();
		stringBuilder.Append ("Variables: {");
		foreach(KeyValuePair<string, ITokenLiteral> pair in variables){
			stringBuilder.Append ($"{pair.Key}:{pair.Value},");
		} 
		stringBuilder.Append ('}');
        return stringBuilder.ToString();
    }

    public int EvaluateMain(Main main)
	{
		/*
		ITokenLiteral literal = NilLiteral.NIL;
		foreach (var item in main.expr.body)
		{
			literal = Evaluate(item);
			INFO($"{item}: {literal}");
		}
		*/
		Evaluate(main.expr);
		return exitCode;
	}

	private ITokenLiteral Evaluate(Stmt stmt)
	{
		if(exit) return NilLiteral.NIL;		
		
		return EvaluateStmt(stmt);
	}
	
	private ITokenLiteral EvaluateStmt(Stmt stmt){
		if(stmt is AssertStmt assert){
			if(Evaluate(assert.condition).Equals(BooleanLiteral.FALSE)){
				exit = true;
				exitCode = 1;
				return BooleanLiteral.FALSE;
			}else return BooleanLiteral.TRUE;
		}
		else if(stmt is ReturnStmt ret){
			return Evaluate(ret.returnValue);
		}else if(stmt is IfStmt ifStmt){
			if(Evaluate(ifStmt.condition).Equals(BooleanLiteral.TRUE)){
				return EvaluateBody(ifStmt.body);
			}else return Evaluate(ifStmt.next);
		}
		else if (stmt is BodyStmt body)
		{
			return EvaluateBody(body);
		}
		else if (stmt is WhileStmt whil){
			while(Evaluate(whil.cond).Equals(BooleanLiteral.TRUE)){
				Evaluate(whil.body);
			}
			return NilLiteral.NIL;
		}
		return EvaluateVariable(stmt);
	}

	private ITokenLiteral EvaluateVariable(Stmt stmt){
		if (stmt is VariableDeclarationStmt varDec)
		{
			variables.Add(varDec.name, Evaluate(varDec.expr));
			return NilLiteral.NIL;
		}
		else if (stmt is VariableExpr varExpr)
		{
			return variables[varExpr.name];
		}
		return EvaluateOperator(stmt);
	}

	private ITokenLiteral EvaluateOperator(Stmt stmt){
		if (stmt is NotExpr notExpr){
			bool value = ((BooleanLiteral)Evaluate(notExpr.expr)).value;
			return !value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE;
		}
		else if (stmt is OperatorExpr opExpr)
		{
			if (opExpr.opType == OperatorType.ADD || opExpr.opType == OperatorType.SUB || opExpr.opType == OperatorType.MUL || opExpr.opType == OperatorType.DIV || opExpr.opType == OperatorType.MOD)
			{
				return EvaluateMath(opExpr);
			}
			else if (opExpr.opType == OperatorType.EQUIVALENT || opExpr.opType == OperatorType.NOT_EQUIVALENT || opExpr.opType == OperatorType.NOT || opExpr.opType == OperatorType.GREATER || opExpr.opType == OperatorType.GREATER_EQUAL || opExpr.opType == OperatorType.LESSER || opExpr.opType == OperatorType.LESSER_EQUAL || opExpr.opType == OperatorType.OR || opExpr.opType == OperatorType.AND)
			{
				return EvaluateBool(opExpr);
			}
			else if ( opExpr.opType == OperatorType.EQUALS || opExpr.opType == OperatorType.ADD_EQUALS || opExpr.opType == OperatorType.SUB_EQUALS || opExpr.opType == OperatorType.MUL_EQUALS || opExpr.opType == OperatorType.DIV_EQUALS || opExpr.opType == OperatorType.MOD_EQUALS)
			{
				return EvaluateAssign(opExpr);
			}
		}
		return EvaluateArray(stmt);
	}

	private ITokenLiteral EvaluateArray(Stmt stmt){
		if(stmt is ArrayExpr arrExpr){
			List<ITokenLiteral> lits = new();
			foreach(Expr expr in arrExpr.arr){
				lits.Add(Evaluate(expr));
			}
			return new ArrayLiteral(lits.ToArray());
		}else if (stmt is ArrayAccessorExpr arrEvalExpr){
			int index = ToInt(arrEvalExpr.accessor);
			if(arrEvalExpr.left is VariableExpr var){
				return ((ArrayLiteral)variables[var.name]).literals[index];
			}
			ArrayLiteral ex = (ArrayLiteral) Evaluate(arrEvalExpr.left);
			return ex.literals[index];
		}
		return EvaluateStruct(stmt);
	}

	private ITokenLiteral EvaluateStruct(Stmt stmt){
		if (stmt is StructExpr strut){
			Dictionary<string, ITokenLiteral> lits = new();
			foreach(KeyValuePair<string, Expr> pair in strut.values){
				lits.Add( pair.Key, Evaluate(pair.Value) );
			}
			return new StructLiteral(lits);
		}if (stmt is StructAccessorExpr accesor){
			StructLiteral stru = (StructLiteral)Evaluate(accesor.left);
			StringLiteral lit = (StringLiteral) Evaluate(accesor.right);
			return stru.values[lit.value];
		}
		return EvaluateLiterals(stmt);
	}

	private ITokenLiteral EvaluateLiterals(Stmt stmt){
		if (stmt is ITokenLiteral)
		{
			if (stmt is NumberLiteral<int> intLit)
				return intLit;
			else if (stmt is NumberLiteral<float> floatLit)
				return floatLit;
			else if (stmt is StringLiteral strLit)
				return strLit;
			else if (stmt is BooleanLiteral boolLit)
				return boolLit;
			else if (stmt is NilLiteral nilLit)
				return nilLit;
		}
		throw ERROR($"Unimplemented stmt \'{stmt}\'");
	}

	private ITokenLiteral EvaluateBody(BodyStmt expr)
	{
		ITokenLiteral literal = NilLiteral.NIL;
		foreach (var item in expr.body)
		{
			if(exit) return NilLiteral.NIL;
			literal = Evaluate(item);
			if(item is ReturnStmt){
				break;
			}
			INFO($"{item} => {literal}");
		}
		return literal;
	}

	private ITokenLiteral EvaluateMath(OperatorExpr expr)
	{
		if(exit) return NilLiteral.NIL;
		if (expr.left == null || expr.right == null) return NilLiteral.NIL;

		dynamic a = Evaluate(expr.left);
		if (a.Equals(NilLiteral.NIL))
			a = new NumberLiteral<int>(0);
		else if (a is BooleanLiteral aBoolLit)
			a = new NumberLiteral<int>(aBoolLit.value ? 1 : 0);
		dynamic b = Evaluate(expr.right);
		if (b.Equals(NilLiteral.NIL))
			b = new NumberLiteral<int>(0);
		else if (b is BooleanLiteral bBoolLit)
			b = new NumberLiteral<int>(bBoolLit.value ? 1 : 0);

        ITokenLiteral lit;
        if (a is NumberLiteral<float> || b is NumberLiteral<float>)
			lit = expr.opType switch
			{
				OperatorType.ADD => new NumberLiteral<float>(a.value + b.value),
				OperatorType.SUB => new NumberLiteral<float>(a.value - b.value),
				OperatorType.MUL => new NumberLiteral<float>(a.value * b.value),
				OperatorType.DIV => new NumberLiteral<float>(a.value / b.value),
				OperatorType.MOD => new NumberLiteral<float>(a.value % b.value),
				_ => throw ERROR($"Unimplemented math operator {expr.opType}"),
			};
		else lit = expr.opType switch
		{
			OperatorType.ADD => a.value is string || b.value is string ? new StringLiteral($"{a.value}{b.value}") : new NumberLiteral<int>(a.value + b.value),
			OperatorType.SUB => new NumberLiteral<int>(a.value - b.value),
			OperatorType.MUL => new NumberLiteral<int>(a.value * b.value),
			OperatorType.DIV => new NumberLiteral<int>(a.value / b.value),
			OperatorType.MOD => new NumberLiteral<int>(a.value % b.value),
			_ => throw ERROR($"Unimplemented math operator {expr.opType}"),
		};
		return lit;
	}
	
	private ITokenLiteral EvaluateBool(OperatorExpr expr)
	{
		if(exit) return NilLiteral.NIL;
		dynamic a = Evaluate(expr.left);
		if (a.Equals(NilLiteral.NIL))
			a = a.ToNumber();
		dynamic b = Evaluate(expr.right);
		if (b.Equals(NilLiteral.NIL))
			b = b.ToNumber();

		switch (expr.opType)
		{
			case OperatorType.EQUIVALENT: return a.value == b.value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE;
			case OperatorType.NOT_EQUIVALENT: return a.value != b.value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE;
			case OperatorType.AND: return a.value && b.value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE;
			case OperatorType.OR: return a.value || b.value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE;
			default: break;
		}

		if (a is BooleanLiteral aBoolLit) a = aBoolLit.ToNumber();
		if (b is BooleanLiteral bBoolLit) b = bBoolLit.ToNumber();

        return expr.opType switch
        {
            OperatorType.GREATER => a.value > b.value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE,
            OperatorType.LESSER => a.value < b.value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE,
            OperatorType.GREATER_EQUAL => a.value >= b.value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE,
            OperatorType.LESSER_EQUAL => a.value <= b.value ? BooleanLiteral.TRUE : BooleanLiteral.FALSE,
            _ => throw ERROR("Unimplemented or Incorrect operation."),
        };
        //return NilLiteral.NIL;
	}
	
	private ITokenLiteral EvaluateAssign(OperatorExpr expr)
	{
		if(exit) return NilLiteral.NIL;
		bool isArr = expr.left is ArrayAccessorExpr;
        string? assigne;
        if (isArr){
			Expr assign = expr.left;
			while(assign is not VariableExpr )
				assign = ((ArrayAccessorExpr)assign).left;
			assigne = ((VariableExpr)assign).name;
		} else assigne = ((StringLiteral)Evaluate(expr.left)).value;

		if(assigne == null || !variables.ContainsKey(assigne))
			throw ERROR($"Assigne \'{assigne}\' is not defined before it is assigned.");

		if(expr.opType == OperatorType.EQUALS && isArr)
				return AssignExprToIndex(assigne, (ArrayAccessorExpr) expr.left, Evaluate(expr.right));
		else if (expr.opType == OperatorType.EQUALS){
			variables[assigne] = Evaluate(expr.right);
		}else{
            OperatorExpr op = new(){
				left = isArr ? (Expr) Evaluate(expr.left) : (Expr) variables[assigne],
				right = expr.right,
				opType = expr.opType switch{
    				OperatorType.ADD_EQUALS => OperatorType.ADD,
    				OperatorType.SUB_EQUALS => OperatorType.SUB,
    				OperatorType.MUL_EQUALS => OperatorType.MUL,
    				OperatorType.DIV_EQUALS => OperatorType.DIV,
    				OperatorType.MOD_EQUALS => OperatorType.MOD,
    				_ => expr.opType,
				}
			};

			if(isArr) 
				return AssignExprToIndex(assigne, (ArrayAccessorExpr) expr.left, Evaluate(op));
			else 
				variables[assigne] = Evaluate(op);
		}

		return NilLiteral.NIL;
	}
	
	public ITokenLiteral AssignExprToIndex(string array, ArrayAccessorExpr accessor, ITokenLiteral value){
		Expr[] arr = accessor.GetAccessors().Reverse<Expr>().ToArray();
		ArrayLiteral lit = (ArrayLiteral) variables[array];
		for(int i = arr.Length-1; i > 0; i--){
			int ind = ToInt(arr[i]);
			if(ind > arr.Length || ind < 0){
				throw ERROR("Index out of bounds of the array.");
			}

			lit = (ArrayLiteral) lit.literals[ind];
		}
		int index = ToInt(arr[0]);
		if(index > lit.literals.Length || index < 0){
			throw ERROR("Index out of bounds of the array.");
		}
		lit.literals[index] = value;
		return NilLiteral.NIL;
	}

	public int ToInt(Expr expr){
		return (int) ((NumberLiteral<int>) Evaluate(expr)).value;
	}
}
