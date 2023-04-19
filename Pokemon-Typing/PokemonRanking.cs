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
                _scores[attacker] = calculateScore(attacker);//Math.Round(calculateScore(attacker) * 10000) / 10000;
            });
            return _scores;
        }

        // Calculate score for every possible pair of attacks for each type
        private static Dictionary<TypeCombo,TypeCombo> getBestMovesets(int iterations=10)
        {
            _bestMovesets = new Dictionary<TypeCombo,TypeCombo>();
            PokemonTyping.forAllTypes(attacker =>
            {
                _bestMovesets[attacker] = attacker;
            });

            for (int i = 0; i < iterations; i++)
            {
                Console.WriteLine($"Iteration {i}");
                bool dirtyIteration = false;
                PokemonTyping.forAllTypes(attacker =>
                {
                    double bestScore = Enumerable.Sum(PokemonTyping.allTypes.Select(defender => PokemonTyping.netDamage(attacker, defender, _bestMovesets[attacker], _bestMovesets[defender])));
                    TypeCombo initialBestMovest = _bestMovesets[attacker];
                    PokemonTyping.forAllTypes(attackerMoves =>
                    {
                        double movesetQuality = Enumerable.Sum(PokemonTyping.allTypes.Select(defender => PokemonTyping.netDamage(attacker, defender, attackerMoves, _bestMovesets[defender]) * getFrequency(defender)));
                        if (movesetQuality > bestScore)
                        {
                            _bestMovesets[attacker] = attackerMoves;
                            bestScore = movesetQuality;
                        }
                        if(movesetQuality == bestScore && attackerMoves != _bestMovesets[attacker])
                        {
                            Console.WriteLine($"{attacker}: Tie between {attackerMoves} and {_bestMovesets[attacker]}");
                        }
                    });
                    if (initialBestMovest != _bestMovesets[attacker])
                    {
                        dirtyIteration = true;
                        Console.WriteLine($"{attacker}: Replacing {initialBestMovest} with {_bestMovesets[attacker]}");
                    }
                });

                // Fixed point found
                if (!dirtyIteration)
                {
                    Console.WriteLine("Fixed point found");
                    return _bestMovesets;
                }
            }
            return _bestMovesets;
        }

    }
}
