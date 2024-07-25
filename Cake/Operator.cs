using static Cake.Util;
namespace Cake;

public enum OperatorType
{
	ADD, SUB, MUL, DIV, MOD,
	EQUIVALENT, NOT_EQUIVALENT, NOT,
	GREATER, LESSER, GREATER_EQUAL, LESSER_EQUAL,
	AND, OR,
	EQUALS, ADD_EQUALS, SUB_EQUALS, MUL_EQUALS, DIV_EQUALS, MOD_EQUALS,
	NOT_IMPLEMENTED
}
public static class Operator{
	public static OperatorType OpFromString(string op){
		switch (op)
		{
			case "+": return OperatorType.ADD;
			case "+=": return OperatorType.ADD_EQUALS;
			case "-": return OperatorType.SUB;
			case "-=": return OperatorType.SUB_EQUALS;
			case "/": return OperatorType.DIV;
			case "/=": return OperatorType.DIV_EQUALS;
			case "*": return OperatorType.MUL;
			case "*=": return OperatorType.MUL_EQUALS;
			case "%": return OperatorType.MOD;
			case "%=": return OperatorType.MOD_EQUALS;
			case "==": return OperatorType.EQUIVALENT;
			case "!=": return OperatorType.NOT_EQUIVALENT;
			case "not": return OperatorType.NOT;
			case "and": return OperatorType.AND;
			case "or": return OperatorType.OR;
			case "=": return OperatorType.EQUALS;
			case ">": return OperatorType.GREATER;
			case ">=": return OperatorType.GREATER_EQUAL;
			case "<": return OperatorType.LESSER;
			case "<=": return OperatorType.LESSER_EQUAL;
			default: break;
		}
		return OperatorType.NOT_IMPLEMENTED;
	}

	public static string OpToString(OperatorType typ){
		switch (typ)
		{
			case OperatorType.ADD: return "+";
			case OperatorType.ADD_EQUALS: return "+=";
			case OperatorType.SUB: return "-";
			case OperatorType.SUB_EQUALS: return "-=";
			case OperatorType.DIV: return "/";
			case OperatorType.DIV_EQUALS: return "/=";
			case OperatorType.MUL: return "*";
			case OperatorType.MUL_EQUALS: return "*=";
			case OperatorType.MOD: return "%";
			case OperatorType.MOD_EQUALS: return "%=";
			case OperatorType.EQUIVALENT: return "==";
			case OperatorType.NOT_EQUIVALENT: return "!=";
			case OperatorType.NOT: return "not";
			case OperatorType.AND: return "and";
			case OperatorType.OR: return "or";
			case OperatorType.EQUALS: return "=";
			case OperatorType.GREATER: return ">";
			case  OperatorType.GREATER_EQUAL: return ">=";
			case OperatorType.LESSER: return "<";
			case  OperatorType.LESSER_EQUAL: return "<=";
			default: break;
		}

		throw ERROR($"Operator \'{typ}\' invalid or not yet implemented.");
	}
}
