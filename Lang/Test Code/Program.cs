using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMult
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Size of matrix: ");
                int size = Int32.Parse(Console.ReadLine());
                DateTime begin = DateTime.Now;

                Matrix first = new Matrix(size);
                Matrix second = new Matrix(size);
                var res = first.Multiply(second);

                Console.WriteLine("Runtime on size " + size + " : " + (DateTime.Now - begin).TotalSeconds.ToString());
            } while (Console.ReadLine() != "q");
        }
    }

    class Matrix
    {
        public List<List<int>> matrix = new List<List<int>>();
        private int size;
        Random rand = new Random();


        public Matrix(int size)
        {
            this.size = size;
            for (int i = 0; i < size; i++)
            {
                matrix.Add(new List<int>());
                for (int j = 0; j < size; j++)
                {
                    matrix[i].Add(rand.Next());
                }
            }
        }

        public Matrix Multiply(Matrix other)
        {
            Matrix res = new Matrix(size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        res.matrix[i][j] = matrix[i][k]*other.matrix[k][j];
                    }
                }
            }
            return res;
        }

    }
}
