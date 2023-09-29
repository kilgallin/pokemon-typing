using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonTyping
{
    // Compute and store competitive analysis for a given league or distribution of Pokemon
    public class PokemonRanking
    {
        // Store the number of times a given TypeCombo appears in the input distribution
        public static Dictionary<TypeCombo, double> _frequency = null;
        public static Dictionary<TypeCombo, double> frequency
        {
            get { return _frequency ?? loadFrequencies(); }
        }

        // Store the cumulative records of each TypeCombo against the given distribution.
        public static Dictionary<TypeCombo, double> _scores = null;
        public static Dictionary<TypeCombo, double> scores
        {
            get { return _scores ?? calculateScores(); }
        }

        // Track the best pair of moves for each type combination
        public static Dictionary<TypeCombo, TypeCombo> _bestMovesets = null;
        public static Dictionary<TypeCombo, TypeCombo> bestMovesets
        {
            get { return _bestMovesets ?? getBestMovesets(); }
        }

        // Find the number of times the given TypeCombo appears in the data set
        public static double getFrequency(TypeCombo type)
        {
            // Disable weighting and assess against all types
            if (String.IsNullOrEmpty(Program.leagueFile))  
            {
                return 1;
            }
            return frequency.FirstOrDefault(x => x.Key == type).Value;
        }

        // Read pvpoke.com CSV file and parse type combinations from top-tier Pokemon.
        private static Dictionary<TypeCombo,double> loadFrequencies()
        {
            string[] allPokemon = File.ReadAllLines(Program.leagueFile).Take(Program.leagueRelevanceLimit).ToArray();
            IEnumerable<TypeCombo> allTypes = allPokemon.Select(x => x.Split(',')).Select(x => new TypeCombo(x[3], x[4]));
            _frequency = allTypes.GroupBy(t => t).ToDictionary(group => group.Key, group => (double)group.Count());
            return _frequency;
        }

        // Total the score a given attacker would net across all possible defenders
        private static double calculateScore(TypeCombo attacker)
        {
            PokemonTyping.forAllTypes(defender =>
            {
                double result = PokemonTyping.netDamage(attacker, defender, _bestMovesets[attacker], _bestMovesets[defender]);
                double weight = getFrequency(defender);
                _scores[attacker] = _scores.FirstOrDefault(x => x.Key == attacker).Value + result * weight;
                Program.log($"{attacker} scores {_scores[attacker]} against {defender}, weight {weight}");
            });
            return _scores[attacker];
        }

        // Calculate scores for all possible attackers
        private static Dictionary<TypeCombo,double> calculateScores()
        {
            _scores = new Dictionary<TypeCombo, double>();
            PokemonTyping.forAllTypes(attacker =>
            {
                _scores[attacker] = Math.Round(calculateScore(attacker) * Math.Pow(10,Program.decimalPlaces)) / Math.Pow(10, Program.decimalPlaces);
            });
            return _scores;
        }

        private static Dictionary<TypeCombo, TypeCombo> getBestMovesets()
        {
            _bestMovesets = new Dictionary<TypeCombo, TypeCombo>();
            PokemonTyping.forAllTypes(attacker =>
            {
                _bestMovesets[attacker] = attacker;
            });
            if (Program.unlockAttackTypes)
            {
                iterateBestMovesets(Program.bestMovesetIterations);
            }
            return _bestMovesets;
        }

        // Assess score for every possible pair of attacks for each type. Repeat recursively to fixed point or iteration count.
        private static void iterateBestMovesets(int iterations)
        {
            Console.WriteLine($"Iterations remaining: {iterations}");
            
            // Base case
            if(iterations == 0)
            {
                return;
            }

            // Recursive case. Note that Enumerable.Sum over all types, within nested PokemonTyping.forAllTypes
            // actions ranging for attacker types and attacker moveset types, is O(n^3) in number of TypeCombos. 
            // This is O(n^6) in number of TYPES (not combos), which means iterating recursively can be expensive.
            // JDK 2023-04-18: Observe that the O(n^6) is reached with only two levels of indentation within the
            // method, and no for loops.
            bool dirtyIteration = false;
            PokemonTyping.forAllTypes(attacker =>
            {
                // Initialize score from current best-known moveset for the attacker against all other types' best movesets
                TypeCombo initialBestMoveset = _bestMovesets[attacker];
                double bestScore = Enumerable.Sum(PokemonTyping.allTypes.Select(defender =>
                {
                    return PokemonTyping.netDamage(attacker, defender, initialBestMoveset, _bestMovesets[defender]);
                }));

                // Analyze each possible move type combination for the attacker type
                PokemonTyping.forAllTypes(attackerMoves =>
                {
                    // Compute the given moveset's score against all other types with their best-known moveset.
                    double movesetQuality = Enumerable.Sum(PokemonTyping.allTypes.Select(defender => {
                        return PokemonTyping.netDamage(attacker, defender, attackerMoves, _bestMovesets[defender]) * getFrequency(defender); 
                    }));
                 
                    if (movesetQuality > bestScore)
                    {
                        _bestMovesets[attacker] = attackerMoves;
                        bestScore = movesetQuality;
                    }

                    // Edge case hasn't arisen yet (jdk 2023-04-18). Doesn't require special handling anyway.
                    if (movesetQuality == bestScore && attackerMoves != _bestMovesets[attacker])
                    {
                        Console.WriteLine($"{attacker}: Tie between {attackerMoves} and {_bestMovesets[attacker]}");
                    }
                });
                if (initialBestMoveset != _bestMovesets[attacker])
                {
                    dirtyIteration = true;
                    Console.WriteLine($"{attacker}: Replacing {initialBestMoveset} with {_bestMovesets[attacker]}");
                }
            });

            // Fixed point found
            if (!dirtyIteration)
            {
                Console.WriteLine("Fixed point found");
                return;
            }

            // Recurse
            iterateBestMovesets(iterations - 1);
        }

        public static List<List<Type>> orderCoverage()
        {
            Dictionary<String, int> coverages = new Dictionary<string, int>();
            PokemonTyping.forAllTypes(firstTwo =>
            {
                PokemonTyping.forAllTypes(secondTwo =>
                {
                    if (secondTwo.type1 <= firstTwo.type2)
                    {
                        return;
                    }
                    List<Type> allFourTypes = new List<Type>() { firstTwo.type1, firstTwo.type2, secondTwo.type1, secondTwo.type2 };
                    coverages.Add(String.Join(",", allFourTypes.ToArray()), PokemonTyping.superEffectiveCoverage(allFourTypes));
                });
            });
            List<String> ordered = coverages.Keys.ToList()
                .OrderBy(s => coverages[s])
                .Reverse()
                //.Where(s => !s.Contains("None") && !s.Contains("Normal") && !s.Contains("Dragon") && !s.Contains("Ghost"))
                .Select(s => s + ": " + coverages[s])
                .ToList();
            return null;
        }
    }
}
