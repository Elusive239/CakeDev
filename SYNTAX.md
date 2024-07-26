CAKE
======
Cake borrows alot from other languages, mainly python, ruby, and some 
bash. This file will be updated as the language is changed and updated, 
and some syntax may change. 


## Literals
Evaluated from expressions, literals are the object representations of different types in Cake.

### nil-literal
The object representation of **'nil'** or **'null'**. If an expression is evaluated to no value, it
returns nil. Statements always evaluate to nil, but may 

### number-literal
The object representation of a number. Supports integers, floats, and negatives.
<pre><b>1</b>
<b>-534</b>
<b>100.34</b></pre>

### boolean-literal
The object representations of a boolean. 
<pre>
<b>True</b>
<b>False</b>

if True do 
 <b>body-expr</b>
done
</pre>

### string-literal
The object representations of text, declared in text with double quotes.
<pre>"Hello World!" -> Hello World!</pre>

## Statements
### variable-declaration-stmt
Variables store literals to be used and modified later, already evaluated.
They can be created with the **def** keyword, followed by an identifier.
Variables must be assigned to something when they are created.
<pre>
def w = 0
def x = 32
def y = "w" + x + "z"

w -> 0
x -> 32
y -> w32z
</pre>

### while-stmt
<pre>
while <b>expr</b> do 
 <b>body-expr</b> 
done
</pre>

### if-stmt
<pre>
if <b>expr</b> do 
 <b>body-expr</b>
elif <b>expr</b> do 
 <b>body-expr</b>
else do 
 <b>body-expr</b>
done
</pre>

### return-stmt
The **return** keyword just exits a **body-expr** early, for use in functions, loops, etc.
<pre>return <b>expr</b></pre>

### assert-stmt
The **assert** keyword is used for testing conditions. if the expr evaluates to **False**,
then the script exits early with an exit code of 1.
<pre>assert <b>expr</b></pre>


## Expressions

### operator-expr
Operator expressions are expressions that evaluate one or more expressions and perform an
operation on them. This can be a math, boolean, or assignment operation.

#### math-operator
The following is a list of all currently implemented math operators: 
>\+, \-, \*, /, %, 

All math operators follow the following syntax:
<pre>expr1 <b>operator</b> expr2 -> number-literal</pre>
Example:
<pre>
1 <b>+</b> 5 -> 6
1 <b>-</b> 5 -> -4
1 <b>*</b> 5 -> 5
1 <b>/</b> 5 -> 0.2
1 <b>%</b> 5 -> 1
</pre>

#### boolean-operator
The following is a list of all currently implemented boolean operators: 	
>not, and, or, ==, !=, <, <=, \>, \>=

All boolean operators follow the following syntax but **not**:
<pre>expr1 <b>operator</b> expr2 -> boolean-literal</pre>

The **not** operator only takes one expression, and inverts its result.
<pre>not (True or False) -> not True -> False</pre>

#### assignment-operator
The following is a list of all currently implemented assignment operators: 	
>=, +=, -=, *=, /=, %=

All assignment operators follow the following syntax:
<pre>variable <b>operator</b> expr -> variable-modified</pre>

Example:
<pre>
def x = 10
<b>x -= 2</b>
x -> 8
</pre>

### body-expr
a **body-expr** is a list of statements and expressions that get executed.
a body-expr is started with the **do** keyword and usually ends with the 
**done** keyword.

<pre>
if True do
 <b>
 def y = 0
 y += 10
 </b>
done
</pre>

the resulting body-expr to be evaluated:  [def y = 0, y += 10]


## Arrays
Arrays are sets that only contain literals (including other array-literals), and that array can 
be accessed using **[ number-literal ]**

<pre>[expr, expr, ...]</pre>

<pre>
def x = [21, 23, 45, "word", [32, 14, 25] ]

x[0] -> 21
x[3] -> word
x[4][2] -> 25
</pre>

## Structs
Cake supports the creation of very simple objects, defined here 
as "Structs". Structs are objects that hold different kinds of 
literals, assigned to identifiers called properties.

<pre>
def variable = {ident:expr, ident:expr, ... }
</pre>

<!-- To define a struct, you must do the following:
1. start with an open brace (`{`)
2. define a property. To define a property:
- start with a literal (Example: 'x')
- add a colon (`:`)
- add an expression. (4)
- and add a comma (`,`) to add additional properties.
3. repeat 2 until you have all of the properties you need.
4. and end with a closing brace (`}`) -->
Example:
<pre>
	def variable = {x:0, y:{z:21}, z:"string"}
	variable.x -> 0
	variable.y.z -> 21
	variable.z -> string
</pre>