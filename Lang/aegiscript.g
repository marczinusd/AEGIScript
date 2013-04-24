grammar aegiscript;

options {
  language = CSharp3;
  output = AST;
  ASTLabelType=CommonTree;
}

@header {

}

@lexer::header {

}

@lexer::namespace {
    AEGIScript.Lang
}

@parser::namespace {
    AEGIScript.Lang
}

@members {int HIDDEN = Hidden;}


@lexer::members {int HIDDEN = Hidden;}

public
program
	:
		'begin'^
		statement*
		'end' ';' EOF
	;

constant
	:	'constant' type IDENT '=' expression ';'
	;

variable
	:	'var' IDENT (',' IDENT)* ':' type ('=' expression)? ';'
	;
	
type
	:	'Integer'
	;
	
ifStatement
	: 'if'^ expression ':' statement* elsif* else_g? endif
	;
elsif
	: 'elsif'^ expression ':' statement+
	;
else_g
	: ('else'^ ':' statement+)
	;
endif
	: 'endif' ';'
	;
	
whileStatement
	:	'while'^ whileBody /*expression ':'
		statement*
		'end' 'while' ';'*/
	;

whileBody
	: expression ':' statement* 'end' 'while' ';' -> expression statement* 'end';
	

funCallStatement
	: name = IDENT '(' a = expression (',' b += expression)* ')'  -> ^(FUNC $name $a $b*)
	| emptyArgsFun
	;
	
FUNC
	: 'FUNC'
	;
	
	
emptyArgsFun
	: name = IDENT '(' ')'  -> ^(FUNC $name)
	;
	
statement
	:	assignmentStatement
	|	whileStatement
	|	ifStatement
	|	funCallStatement ';'
	|	accessorStatement ';'
	;

assignmentStatement
	:	IDENT '='^ expression ';'
//	|	IDENT '='^ funCallStatement ';'
//	| 	IDENT '='^ accessor ';'
	|	IDENT '='^ accessorStatement ';'
	| 	accessor '='^ expression ';'
	;



accessor
	: a = IDENT ('[' b += INTEGER ']')+ -> ^(ACCESS $a $b+)
	;

ACCESS
	: 'ACCESS'
	;
	
accessorStatement
	:	a=IDENT ('.' b+=funCallStatement)+  -> ^(FIELD_ACCESS $a $b+)
	;
	
FIELD_ACCESS
	: 'F_ACCESS'
	;

array
	: '['a = expression
	(',' b += expression)*']' -> ^(ARRAY $a $b*)
	| '['']'
	;

ARRAY
	: 'ARRAY'
	;

term 
	:	IDENT
	|	'(' expression ')' -> expression
	|	INTEGER
	| 	STRING
	|	BOOL
	|	FLOAT
	|	array
	|	accessor
	|	funCallStatement
	;
	
negation
	:	'not'^* term 
	;
	
unary
	:	('+'^ | '-'^)* negation
	;

mult
	:	unary (('*'^ | '/'^ | 'mod'^) unary)*
	;
	
add
	:	mult (('+'^ | '-'^) mult)*
	;

relation
	:	add (('=='^ | '!='^ | '<'^ | '<='^ | '>='^ | '>'^) add)*
	;
	
expression
	:	relation (('&&'^ | '||'^) relation)*
	;


BOOL
	: 'true'
	| 'false'
	;

FLOAT
    :   ('0'..'9')+ '.' ('0'..'9')* EXPONENT?
    |   '.' ('0'..'9')+ EXPONENT?
    |   ('0'..'9')+ EXPONENT
    ;

MULTILINE_COMMENT : '/*' .* '*/' {$channel = HIDDEN;} ;

fragment
EXPONENT : ('e'|'E') ('+'|'-')? ('0'..'9')+ ;

fragment
HEX_DIGIT : ('0'..'9'|'a'..'f'|'A'..'F') ;

fragment
ESC_SEQ
    :   '\\' ('b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\')
    |   UNICODE_ESC
    |   OCTAL_ESC
    ;

fragment
OCTAL_ESC
    :   '\\' ('0'..'3') ('0'..'7') ('0'..'7')
    |   '\\' ('0'..'7') ('0'..'7')
    |   '\\' ('0'..'7')
    ;

fragment
UNICODE_ESC
    :   '\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
    ;
fragment LETTER : ('a'..'z' | 'A'..'Z') ;
fragment DIGIT : '0'..'9';
INTEGER : DIGIT+ ;
STRING
    :  '"' ( ESC_SEQ | ~('\\'|'"') )* '"'
    ;
IDENT : LETTER (LETTER | DIGIT)*;
WS : (' ' | '\t' | '\n' | '\r' | '\f')+ {$channel = HIDDEN;};
COMMENT : '//' .* ('\n'|'\r') {$channel = HIDDEN;};