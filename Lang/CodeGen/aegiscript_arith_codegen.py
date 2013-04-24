if __name__ == '__main__':
	main()

def main():
	pass

"""
	Rels:
		In source: > : GT : ResNode
		Represented as: Rel["GT"] = [">", "ResNode"] 
"""
def gen_arith_for_types(type1, type2, res, rels):
	Method = 'public static TermNode Op(' + type1 + 'Node left, ' + type2 + 'Node right, ArithmticNode.Operator op) \n { \n \t\tswitch(op) \n \t\t\t {'
	for Key in rels.keys():
		Method += '\t\t\t case ArithmeticNode.Operator' + Key + ': \n\t\t\t\t return new ' + res + '(left.Value ' + rels[Key] + ' right.Value); \n'
	Method += 'default: \n throw new Exception("Invalid operation"); \t\t\t\n } \t\t\n }' 

	return Method


"""
def gen_arith_class():
	C
"""