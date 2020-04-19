using System;
using System.Collections.Generic;
using System.Linq;

#if !NETCOREAPP
using System.Collections;
#endif

namespace Expogo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            int testCases = int.Parse(Console.ReadLine());

            for (int i = 0; i < testCases; ++i)
            {
                HandleTestCase(i + 1);
            }
        }

        public static void HandleTestCase(int caseNumber)
        {
            string solution = string.Empty;

            var strings = Console.ReadLine()
                .Trim(' ', '\r', '\n')
                .Split(' ');

            if (strings.Length != 2) { throw new Exception("Error parsing input"); }

            var destinationPoint = new Point(
                int.Parse(strings[0]),
                int.Parse(strings[1]));

            bool isXEven = destinationPoint.X % 2 == 0;
            bool isYEven = destinationPoint.Y % 2 == 0;

            if ((isXEven && isYEven) || (!isXEven && !isYEven))
            {
                Console.WriteLine($"Case #{caseNumber}: IMPOSSIBLE");
                return;
            }

            ICollection<Point> candidatesPoints = isXEven
                ? new List<Point>
                    {
                        new Point(0, 1),
                        new Point(0, -1)
                    }
                : new List<Point>
                    {
                        new Point(1, 0),
                        new Point(-1, 0)
                    };

            var path = new List<Point>();
            var currentPoint = new Point(0, 0);
            path.Add(currentPoint);

            // Positive and negative, to calculate combination arrays and .Sum() them.
            var potenciasDeDos = new List<int>();
            // Positive to discard in GenerateCombinations() those ones that 'skip' numbers
            var potenciasDeDosPositivas = new List<int>();

            const int nMax = 8;
            for (int n = 1; n < nMax; ++n)
            {
                int potenciaDeDos = (int)Math.Pow(2, n);
                potenciasDeDosPositivas.Add(potenciaDeDos);
                potenciasDeDos.Add(potenciaDeDos);
                potenciasDeDos.Add(-potenciaDeDos);
            }

            for (int i = 1; i < nMax; ++i)
            {
                if (candidatesPoints.Contains(destinationPoint))
                {
                    break;
                }

                // Skips the first n numbers
                potenciasDeDosPositivas = potenciasDeDosPositivas.Skip(i - 1).ToList();
                potenciasDeDos = potenciasDeDos.Skip(2 * (i - 1)).ToList();

                IEnumerable<IList<int>> combinations = GenerateCombinations(potenciasDeDos, potenciasDeDosPositivas);

                var combinationSums = new HashSet<int>(combinations.Select(c => c.Sum())
                    .Where(n => n > 0))
                    .OrderBy(n => n).ToList();

                var candidatesQueSumanBien = candidatesPoints.Where(c => combinationSums.Contains(c.ManhattanDistance(destinationPoint)));

                if (candidatesQueSumanBien.Count() == 1)
                {
                    currentPoint = candidatesQueSumanBien.Single();
                    path.Add(currentPoint);
                }
                else if (candidatesQueSumanBien.Any())
                {
                    var candidatesWithDifferentXAndY = candidatesQueSumanBien.Where(c => Math.Abs(destinationPoint.X - c.X) != Math.Abs(destinationPoint.Y - c.Y));
                    if (candidatesWithDifferentXAndY.Count() == 1)
                    {
                        currentPoint = candidatesWithDifferentXAndY.Single();
                        path.Add(currentPoint);
                    }
                    else
                    {
                        throw new Exception("El algoritmo es una puta mierda");
                    }
                }

                candidatesPoints = ExtractCandidates(currentPoint, i);
            }

            path.Add(destinationPoint);

            solution = GenerateSolutionFromPath(solution, path);

            Console.WriteLine($"Case #{caseNumber}: {solution}");
        }

        private static ICollection<Point> ExtractCandidates(Point currentPoint, int i)
        {
            var gap = (int)Math.Pow(2, i);
            return new List<Point>
            {
                new Point(currentPoint.X + gap, currentPoint.Y),
                new Point(currentPoint.X - gap, currentPoint.Y),
                new Point(currentPoint.X, currentPoint.Y + gap),
                new Point(currentPoint.X, currentPoint.Y - gap)
            };
        }

        private static IEnumerable<IList<int>> GenerateCombinations(List<int> potenciasDeDos, List<int> potenciasDeDosPositivas)
        {
#if NETCOREAPP
            var totalCombinations = MoreLinq.MoreEnumerable.Subsets(potenciasDeDos).Where(c => c.Any());
#else
                var totalCombinations = Subsets(potenciasDeDos).Where(c => c.Any());
#endif

            return totalCombinations
                .Where(combination =>
                {
                    var potenciasPositivasUsadas = combination.Select(Math.Abs);
                    bool combinationIncludesXAndMinusX = potenciasPositivasUsadas.Count() != new HashSet<int>(potenciasPositivasUsadas)?.Count;

                    if (combinationIncludesXAndMinusX)
                    {
                        return false;
                    }

                    // Detect if any number is 'skipped'
                    int max = combination.Max(Math.Abs);
                    foreach (var potenciaDeDos in potenciasDeDosPositivas)
                    {
                        if (potenciaDeDos == max)
                        {
                            break;
                        }

                        if (!potenciasPositivasUsadas.Contains(potenciaDeDos))
                        {
                            return false;
                        }
                    }

                    return true;
                });
        }

        private static string GenerateSolutionFromPath(string solution, List<Point> path)
        {
            for (int i = 1; i < path.Count; ++i)
            {
                var current = path[i];
                var past = path[i - 1];

                if (current.X > past.X)
                {
                    solution += "E";
                }
                else if (current.X < past.X)
                {
                    solution += "W";
                }
                else if (current.Y > past.Y)
                {
                    solution += "N";
                }
                else if (current.Y < past.Y)
                {
                    solution += "S";
                }
            }

            return solution;
        }

#if NETCOREAPP
    }
#else
    private static IEnumerable<IList<T>> Subsets<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));

            var sequenceAsList = sequence.ToList();
            var sequenceLength = sequenceAsList.Count;

            // the first subset is the empty set
            yield return new List<T>();

            // all other subsets are computed using the subset generator
            // this check also resolves the case of permuting empty sets
            if (sequenceLength > 0)
            {
                for (var i = 1; i < sequenceLength; i++)
                {
                    // each intermediate subset is a lexographically ordered K-subset
                    var subsetGenerator = new SubsetGenerator<T>(sequenceAsList, i);
                    foreach (var subset in subsetGenerator)
                        yield return subset;
                }

                yield return sequenceAsList; // the last subet is the original set itself
            }
        }
    }

    internal sealed class SubsetGenerator<T> : IEnumerable<IList<T>>
    {
        /// <summary>
        /// SubsetEnumerator uses a snapshot of the original sequence, and an
        /// iterative, reductive swap algorithm to produce all subsets of a
        /// predetermined size less than or equal to the original set size.
        /// </summary>

        class SubsetEnumerator : IEnumerator<IList<T>>
        {
            readonly IList<T> _set;   // the original set of elements
            readonly T[] _subset;     // the current subset to return
            readonly int[] _indices;  // indices into the original set

            // TODO: It would be desirable to give these index members clearer names
            bool _continue;  // termination indicator, set when all subsets have been produced

            int _m;            // previous swap index (upper index)
            int _m2;           // current swap index (lower index)
            int _k;            // size of the subset being produced
            int _n;            // size of the original set (sequence)
            int _z;            // count of items excluded from the subet

            public SubsetEnumerator(IList<T> set, int subsetSize)
            {
                // precondition: subsetSize <= set.Count
                if (subsetSize > set.Count)
                    throw new ArgumentOutOfRangeException(nameof(subsetSize), "Subset size must be <= sequence.Count()");

                // initialize set arrays...
                _set = set;
                _subset = new T[subsetSize];
                _indices = new int[subsetSize];
                // initialize index counters...
                Reset();
            }

            public void Reset()
            {
                _m = _subset.Length;
                _m2 = -1;
                _k = _subset.Length;
                _n = _set.Count;
                _z = _n - _k + 1;
                _continue = _subset.Length > 0;
            }

            public IList<T> Current => (IList<T>)_subset.Clone();

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (!_continue)
                    return false;

                if (_m2 == -1)
                {
                    _m2 = 0;
                    _m = _k;
                }
                else
                {
                    if (_m2 < _n - _m)
                    {
                        _m = 0;
                    }
                    _m++;
                    _m2 = _indices[_k - _m];
                }

                for (var j = 1; j <= _m; j++)
                    _indices[_k + j - _m - 1] = _m2 + j;

                ExtractSubset();

                _continue = (_indices[0] != _z);
                return true;
            }

            void IDisposable.Dispose() { }

            void ExtractSubset()
            {
                for (var i = 0; i < _k; i++)
                    _subset[i] = _set[_indices[i] - 1];
            }
        }

        readonly IEnumerable<T> _sequence;
        readonly int _subsetSize;

        public SubsetGenerator(IEnumerable<T> sequence, int subsetSize)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
            if (subsetSize < 0)
                throw new ArgumentOutOfRangeException(nameof(subsetSize), "{subsetSize} must be between 0 and set.Count()");
            _subsetSize = subsetSize;
            _sequence = sequence;
        }

        /// <summary>
        /// Returns an enumerator that produces all of the k-sized
        /// subsets of the initial value set. The enumerator returns
        /// and <see cref="IList{T}"/> for each subset.
        /// </summary>
        /// <returns>an <see cref="IEnumerator"/> that enumerates all k-sized subsets</returns>

        public IEnumerator<IList<T>> GetEnumerator()
        {
            return new SubsetEnumerator(_sequence.ToList(), _subsetSize);
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
#endif

    internal sealed class Point : IEquatable<Point>
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int ManhattanDistance(Point point)
        {
            return Math.Abs(point.X - X) + Math.Abs(point.Y - Y);
        }

        #region Equals override

        public override int GetHashCode()
        {
            var hashCode = 1166230731;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Point))
            {
                return false;
            }

            return Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            if (other == null)
            {
                return false;
            }

            return X == other.X && Y == other.Y;
        }
        #endregion
    }
}
