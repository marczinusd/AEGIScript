AEGIScript
==========

AEGIScript is a dynamically typed script language created using ANTLR v3.5's C# target. It'a main purpose is to provide an easy to use language for the developers of the AEGIS project to write geoinformatics batch-scripts in. 

The application comes with a user interface designed in Windows Presentation Foundation, with AvalonEdit as the editor, and MahApps for window and controller styling.

Usage
==========
There are two ways you can start using AEGIScript

Downloading the binaries
----------
Download the binaries folder from the master branch, and run AEGIScript.exe. All DLL files are included, but .NET 4.0 is required to run the editor.

Compiling from source
----------
To compile from source, clone the whole master branch, and load the solution up in Visual Studio. You might get some error messages regarding missing DLL files. In this case, simply add new references to the DLLs, that can be found in the binaries folder.

Syntax
==========
AEGIScript uses a very simple syntax, very similar to Python. Below is the semi-formal definition of the language:

'''
PROGRAM   		::= begin STATEMENT+ end;
STATEMENT 			::= ASSIGN_STATEMENT | IF_STATEMENT 
					| WHILE_STATEMENT | FUNCALL_STATEMENT
ASSIGN_STATEMENT 	::= ( identifier | ARRAY_ACCESSOR ) = EXPRESSION ;
IF_STATEMENT 	 	::= if EXPRESSION : STATEMENT+ 
						(elsif EXPRESSION: STATEMENT+ end;)* 
						(else: STATEMENT+)? endif;
WHILE_STATEMENT 	::= while EXPRESSION: STATEMENT+ end while ;
FUNCALL_STATEMENT 	::= identifier ( argument? (, argument)* ) ;
EXPRESSION 			::= identifier | arithmetic_expression 
						| FUNCALL_EXPRESSION | ARRAY_ACCESSOR | FIELD_ACCESSOR
FUNCALL_EXPRESSION  ::= identifier ( argument? (, argument)* )
ARRAY_ACCESSOR      ::= identifier ([EXPRESSION])*
FIELD_ACCESSOR      ::= identifier.FUNCALL_EXPRESSION+
Operators	        ::= { +, -, *, /, mod, not, ||, && }
Keywords	        ::= { begin, end, while, if, endif, true, false }
'''
