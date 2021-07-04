using System;
using System.Collections.Generic;
using System.Linq;

namespace _8QueenGA
{
    public class Program
    {

        private static readonly Random rnd = new();


        private static readonly float crossrate = 0.4f;
        private static readonly float mutrate = 0.001f;
        private static readonly int popSize = 1500;

        private static void Main(string[] args)
        {
            new Program().Run();
        }



        private void Run()
        {
            var population = new List<SolutionPopulation>();

            for (int i = 0; i < popSize; i++)
            {
                population.Add(new SolutionPopulation());
            }


            foreach (var solution in population)
            {
                for (int counter = 0; counter < 64; counter++)
                {
                    var i = rnd.Next(0, 8);
                    var j = rnd.Next(0, 8);
                    solution.AddQueen(i, j);
                }



            }
            var generation = 0;
            var max = 0.0f;
            while (true)
            {

                foreach (var solution in population)
                {
                    if (solution.NoOfQueens() != 8)
                    {
                        solution.MakeSure(8);
                    }
                    solution.CalculateFitness();
                }

                var _max = population.Max(z => z.Fitness);
                if (true)
                {
                    max = _max;
                    var sol = population.FirstOrDefault(ft => ft.Fitness == max);
                    Console.Clear();
                    Console.WriteLine($"{max}");
                    Console.WriteLine();
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (sol.GetQueen(i, j))
                            {
                                Console.Write("Q ");
                            }
                            else
                            {
                                Console.Write(". ");
                            }
                        }
                        Console.WriteLine();
                    }
                }
                population = GenerateNewPopulation(population);
                generation++;
            }
        }

        private List<SolutionPopulation> GenerateNewPopulation(List<SolutionPopulation> currentPopulation)
        {
            List<SolutionPopulation> newPopulation = new();

            if (currentPopulation.Count % 2 != 0)
            {
                throw new Exception("Population count must be even number");
            }
            var totalThisGenerationFitness = 0;
            foreach (var solution in currentPopulation)
            {
                totalThisGenerationFitness += solution.Fitness;
            }

            while (newPopulation.Count < popSize)
            {
                var p1 = SelectCandidate(currentPopulation, totalThisGenerationFitness);
                var p2 = SelectCandidate(currentPopulation, totalThisGenerationFitness);

                CreateChild(p1, p2, out SolutionPopulation c1, out SolutionPopulation c2);

                c1.Mutate();
                c2.Mutate();


                newPopulation.Add(c1);
                newPopulation.Add(c2);


            }
            return newPopulation;
        }

        private static SolutionPopulation SelectCandidate(List<SolutionPopulation> currentPopulation, int totalThisGenerationFitness)
        {
            var rndValForSel = rnd.Next(0, totalThisGenerationFitness);

            for (int i = 0; i < popSize; i++)
            {
                rndValForSel -= currentPopulation[i].Fitness;

                if (rndValForSel <= 0)
                {
                    return currentPopulation[i];
                }

            }

            return currentPopulation[popSize - 1];
        }

        private static void CreateChild(SolutionPopulation parent1, SolutionPopulation parent2, out SolutionPopulation child1, out SolutionPopulation child2)
        {
            child1 = new SolutionPopulation();
            child2 = new SolutionPopulation();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (parent1.GetQueen(i, j))
                    {
                        child1.AddQueen(i, j);
                    }

                    if (parent2.GetQueen(i, j))
                    {
                        child2.AddQueen(i, j);
                    }
                }
            }

            if (rnd.NextDouble() < crossrate)
            {
                var crossI = rnd.Next(0, 8);
                var crossJ = rnd.Next(0, 8);

                for (int i = 0; i < 8; i++)
                {
                    bool brk = false;
                    for (int j = 0; j < 8; j++)
                    {
                        if (i == crossI & j == crossJ)
                        {
                            brk = true;
                            break;
                        }
                        if (parent1.GetQueen(i, j))
                        {
                            child1.AddQueen(i, j);
                        }
                        else
                        {
                            child1.RemoveQueen(i, j);
                        }

                        if (parent2.GetQueen(i, j))
                        {
                            child2.AddQueen(i, j);
                        }
                        else
                        {
                            child2.RemoveQueen(i, j);
                        }

                    }
                    if (brk)
                    {
                        break;
                    }
                }

                for (int i = crossI; i < 8; i++)
                {
                    for (int j = crossJ; j < 8; j++)
                    {
                        if (parent1.GetQueen(i, j))
                        {
                            child2.AddQueen(i, j);
                        }
                        else
                        {
                            child2.RemoveQueen(i, j);
                        }

                        if (parent2.GetQueen(i, j))
                        {
                            child1.AddQueen(i, j);
                        }
                        else
                        {
                            child1.RemoveQueen(i, j);
                        }
                    }
                    crossJ = 0;
                }
            }
        }

        private class SolutionPopulation
        {
            private readonly bool[][] queenpos = new bool[8][];
            public int Fitness { get; private set; }

            public bool GetQueen(int i, int j)
            {
                return queenpos[i][j];
            }

            public SolutionPopulation()
            {
                queenpos[0] = new bool[8];
                queenpos[1] = new bool[8];
                queenpos[2] = new bool[8];
                queenpos[3] = new bool[8];
                queenpos[4] = new bool[8];
                queenpos[5] = new bool[8];
                queenpos[6] = new bool[8];
                queenpos[7] = new bool[8];
            }

            public void Mutate()
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (rnd.NextDouble() < mutrate)
                        {

                            queenpos[i][j] = !queenpos[i][j];
                        }
                    }
                }
            }

            public int NoOfQueens()
            {
                int queens = 0;
                foreach (var i in queenpos)
                {
                    foreach (var j in i)
                    {
                        if (j)
                        {
                            queens++;
                        }
                    }
                }
                return queens;

            }


            public void MakeSure(int noOfQueens)
            {
                while (NoOfQueens() != noOfQueens)
                {
                    var i = rnd.Next(0, 8);
                    var j = rnd.Next(0, 8);

                    if (NoOfQueens() < noOfQueens)
                    {
                        queenpos[i][j] = true;
                    }
                    else
                    {
                        queenpos[i][j] = false;
                    }
                }
            }

            public void AddQueen(int i, int j)
            {
                queenpos[i][j] = true;
            }

            public void RemoveQueen(int i, int j)
            {
                queenpos[i][j] = false;
            }

            public void CalculateFitness()
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    for (int jj = 0; jj < 8; jj++)
                    {
                        if (queenpos[ii][jj])
                        {
                            var ft = FitmessPerQueen(ii, jj);
                            Fitness += ft;
                        }
                    }
                }

            }

            private int FitmessPerQueen(int i, int j)
            {
                var left = false;
                var right = false;
                var top = false;
                var bottom = false;

                var fitness = 0;

                if (!queenpos[i][j])
                {
                    throw new Exception("No queen in this position");
                }

                if (i == 0)
                {
                    left = true;
                }
                else if (i == 7)
                {
                    right = true;
                }

                if (j == 0)
                {
                    bottom = true;
                }
                else if (j == 7)
                {
                    top = true;
                }


                if (!right)
                {
                    fitness += 1;
                    for (int pos = i + 1, dis = 1; pos < 8; pos++, dis++)
                    {
                        if (queenpos[pos][j])
                        {
                            return 0;
                        }
                    }
                }

                if (!left)
                {
                    fitness += 1;
                    for (int pos = i - 1, dis = 1; pos >= 0; pos--, dis++)
                    {
                        if (queenpos[pos][j])
                        {
                            return 0;
                        }
                    }
                }

                if (!top)
                {
                    fitness += 1;
                    for (int pos = j + 1, dis = 1; pos < 8; pos++, dis++)
                    {
                        if (queenpos[i][pos])
                        {
                            return 0;
                        }
                    }
                }

                if (!bottom)
                {
                    fitness += 1;
                    for (int pos = j - 1, dis = 1; pos >= 0; pos--, dis++)
                    {
                        if (queenpos[i][pos])
                        {
                            return 0;
                        }
                    }
                }

                if (!top && !left)
                {
                    fitness += 1;
                    for (int x = i - 1, y = j + 1, dis = 1; x >= 0 && y < 8; x--, y++, dis++)
                    {
                        if (queenpos[x][y])
                        {
                            return 0;
                        }
                    }
                }

                if (!top && !right)
                {
                    fitness += 1;
                    for (int x = i + 1, y = j + 1, dis = 1; x < 8 && y < 8; x++, y++, dis++)
                    {
                        if (queenpos[x][y])
                        {
                            return 0;
                        }
                    }
                }

                if (!bottom && !right)
                {
                    fitness += 1;
                    for (int x = i + 1, y = j - 1, dis = 1; x < 8 && y >= 0; x++, y--, dis++)
                    {
                        if (queenpos[x][y])
                        {
                            return 0;
                        }
                    }
                }

                if (!bottom && !left)
                {
                    fitness += 1;
                    for (int x = i - 1, y = j - 1, dis = 1; x >= 0 && y >= 0; x--, y--, dis++)
                    {
                        if (queenpos[x][y])
                        {
                            return 0;
                        }
                    }
                }

                return fitness;
            }
        }
    }
}
