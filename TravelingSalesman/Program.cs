/******************************************************************************
 * Author:      Jason Tierney
 * Description: A silly TSP solver.
 *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TravelingSalesman
{
    public class Program
    {
        static int ALL = -1;
        static bool[] visited;
        static int[] minCircuit;
        static int[] hamiltonianCircuit;
        static int minCircuitLength;
        static int size = 1;
        static int[,] matrix = new int[size, size];
        static int max = 100;
        static int newCircuitLength;

        /// <summary>
        /// Main entry point of the program.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Write("Please enter the filename: ");
            string filename = Console.ReadLine();
            string[] lines = File.ReadAllLines(filename);
            newCircuitLength = max;

            /**
             * The following code Loops through and parses the input file. 
             * 
             * The file is in the following format:
             * 
             * 5
             * 0,1,6
             * 0,2,7
             * 0,3,8
             * 0,4,9
             * 1,2,1
             * 1,3,1
             * 1,4,1
             * 2,3,4
             * 2,4,10
             * 3,4,12
             * E
             * 
             * The first line represents the size of the matrix.
             * 
             * Each line thereafter represents two connected vertices and 
             * the weight between them.
             * NOTE: The graph is undirected, so 0,1,6 as in above is equivalent
             * to 1,0,6.
             * 
             * The "E" represents the end of a matrix's input. There can be multiple
             * matrices within the file, the "E" is just a separator between them.
             */
            for (int i = 0; i < lines.Length; i++)
            {
                // Start handling the input.
                if (!lines[i].Contains(','))
                {
                    int temp;
                    if (int.TryParse(lines[i], out temp))
                    {
                        size = temp;
                        matrix = new int[size, size];                    
                    }
                    else if (lines[i].Equals("E")) // Start analyzing the graph.
                    {
                        // Print out the matrix.
                        PrintMatrix(matrix, size);

                        // Reset our variables.
                        visited = new bool[size];
                        minCircuit = new int[size];
                        hamiltonianCircuit = new int[size + 2];
                        max = 0;

                        for(int k = 0; k < size; k++)
                        {
                            for(int l = 0; l < size; l++)
                            {
                                max += matrix[k, l];
                                if (matrix[k, l] == 0)
                                {
                                    matrix[k, l] = 100;
                                }
                            }
                        }

                        max = max / 2;
                        minCircuitLength = max;
                        CheckMatrix();                        
                    }
                    else
                    {
                        Console.WriteLine("Error reading file. Please check your input.");
                        return;
                    }
                }
                else
                {
                    string[] parts = lines[i].Split(',');
                    if(parts.Length < 3 || parts.Length > 3)
                    {
                        Console.WriteLine("Error reading file. Please check your input.");
                        return;
                    }

                    int row = 0, col = 0, weight = 0;
                    if (int.TryParse(parts[0], out row) && int.TryParse(parts[1], out col) && int.TryParse(parts[2], out weight))
                    {
                        if (row < size && col < size)
                        {
                            matrix[row, col] = weight;
                            matrix[col, row] = weight;
                        }
                        else
                        {
                            Console.WriteLine("The row or col is larger than the size of the matrix.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not a number...");
                    }
                }
            }

            // Keep the window open for the user.
            Console.ReadKey();
        }

        /// <summary>
        /// Checks the matrix for the shortest path.
        /// </summary>
        private static void CheckMatrix()
        {
            for (int svid = 0; svid < size; svid++)
            {
                for (int j = 0; j < size; j++)
                {
                    SetVisited(ALL, false);
                    SetVisited(0, true);
                    ResetMinCircuit(0);
                    newCircuitLength = GetValidCircuit(0, j);

                    if (newCircuitLength <= minCircuitLength)
                    {
                        minCircuitLength = newCircuitLength;
                        SetHamiltonianCircuit(minCircuitLength);
                    }
                }
            }

            if (minCircuitLength < max)
            {
                Console.WriteLine("Minimum circuit length is: {0}", minCircuitLength);
                Console.Write("The circuit is: [");
                for (int j = 1; j < size + 2; j++)
                {
                    if (j == size + 1)
                    {
                        Console.Write("{0}]", hamiltonianCircuit[j]);
                    }
                    else
                    {
                        Console.Write("{0}, ", hamiltonianCircuit[j]);
                    }
                }

                Console.WriteLine("\n\n");
            }
            else
            {
                Console.WriteLine("No circuit...");
            }
        }


        /// <summary>
        /// Prints the matrix to the console.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="size"></param>
        public static void PrintMatrix(int[,] matrix, int size)
        {
            Console.WriteLine("The matrix representation of the graph is:");
            Console.WriteLine();
            Console.Write("[");
            for (int i = 0; i < size; i++)
            {
                Console.Write("[");
                for (int j = 0; j < size; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }

                Console.WriteLine("]");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Resets the currenct minimum circuit.
        /// </summary>
        /// <param name="start"></param>
        private static void ResetMinCircuit(int start)
        {
            minCircuit[0] = start;
            for (int i = 1; i < size; i++)
            {
                minCircuit[i] = -1;
            }
        }

        /// <summary>
        /// Sets whether a node has been visited.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        private static void SetVisited(int value, bool flag)
        {
            if (value == ALL)
            {
                for (int i = 0; i < size; i++)
                {
                    visited[i] = flag;
                }
            }
            else
            {
                visited[value] = flag;
            }
        }

        /// <summary>
        /// Sets the Hamiltonian Circuit.
        /// </summary>
        /// <param name="pl"></param>
        private static void SetHamiltonianCircuit(int pl)
        {
            hamiltonianCircuit[0] = pl;
            for (int i = 0; i < size; i++)
            {
                hamiltonianCircuit[i + 1] = minCircuit[i];
            }

            hamiltonianCircuit[size + 1] = minCircuit[0];
        }

        /// <summary>
        /// Returns a valid circuit within the graph.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private static int GetValidCircuit(int start, int next)
        {
            int nextv = 0, min, vcount = 1;
            int pathLength = 0;
            minCircuit[0] = start;
            minCircuit[1] = next;
            SetVisited(next, true);
            pathLength += matrix[start, next];

            for (int i = next; vcount < size - 1; vcount++)
            {
                min = max;
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j] < max && !visited[j] && matrix[i, j] <= min && matrix[i, j] != 0)
                    {
                        nextv = j;
                        min = matrix[i, j];
                    }
                }

                SetVisited(nextv, true);
                i = minCircuit[vcount + 1] = nextv;
                pathLength += min;
            }

            pathLength += matrix[minCircuit[size - 1], start];
            return pathLength;
        }
    }
}