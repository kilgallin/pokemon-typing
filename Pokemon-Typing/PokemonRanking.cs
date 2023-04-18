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
        public static Dictionary<TypeCombo, double> _scores;
        public static Dictionary<TypeCombo, double> scores
        {
            get { return _scores ?? calculateScores(); }
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
                double result = PokemonTyping.netDamage(attacker, defender);
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
                _scores[attacker] = Math.Round(calculateScore(attacker) * 1000) / 1000;
            });
            return _scores;
        }

    }
}
