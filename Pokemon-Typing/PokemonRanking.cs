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
            double score = 0;
            for (int defType1 = 0; defType1 < 18; defType1++)
            {
                for (int defType2 = defType1 + 1; defType2 < 19; defType2++)
                {
                    TypeCombo defender = new TypeCombo(defType1, defType2);
                    double result = PokemonTyping.netDamage(attacker, defender);
                    double weight = getFrequency(defender);
                    score += result * weight;
                    Program.log($"{attacker} scores {result} against {defender}, weight {weight}");
                }
            }
            return score;
        }

        // Calculate scores for all possible attackers
        private static Dictionary<TypeCombo,double> calculateScores()
        {
            _scores = new Dictionary<TypeCombo, double>();
            for (int atkType1 = 0; atkType1 < 18; atkType1++)
            {
                for (int atkType2 = atkType1 + 1; atkType2 < 19; atkType2++)
                {
                    TypeCombo attacker = new TypeCombo(atkType1, atkType2);
                    _scores[attacker] = Math.Round(calculateScore(attacker) * 1000) / 1000;
                }
            }
            return _scores;
        }

    }
}
