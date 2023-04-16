using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonTyping
{
    public class PokemonRanking
    {
        public static Dictionary<TypeCombo, int> _frequency = null;
        public static Dictionary<TypeCombo, int> frequency
        {
            get { return _frequency ?? loadFrequencies(); }
        }

        public static Dictionary<TypeCombo, double> _scores;
        public static Dictionary<TypeCombo, double> scores
        {
            get { return _scores ?? calculateScores(); }
        }
        public static int getFrequency(TypeCombo type)
        {
            var x = frequency.FirstOrDefault(x => x.Key == type);
            return x.Value;
        }

        // Read pvpoke.com CSV file and parse type combinations from top-tier Pokemon.
        private static Dictionary<TypeCombo,int> loadFrequencies()
        {
            string[] allPokemon = File.ReadAllLines(@"C:\Users\jdk\Downloads\all Rankings.csv");
            Dictionary<TypeCombo, int> comboCounts = new Dictionary<TypeCombo, int>();
            String[] typeNames = Enumerable.Range(0, 19).Select(i => ( (Type)i ).ToString().ToLower()).ToArray();
            for (int i = 0; i < 100; i++)
            {
                string[] pokemon = allPokemon[i].Split(',');
                TypeCombo type = new TypeCombo(Array.IndexOf(typeNames, pokemon[3]), Array.IndexOf(typeNames, pokemon[4]));
                if (Config.debug)
                {
                    Console.WriteLine(pokemon[0]);
                }
                var existing = comboCounts.Where(x => x.Key == type).ToList();
                if (existing.Count != 0)
                {
                    comboCounts[existing[0].Key]++;
                }
                else
                {
                    comboCounts[type] = 1;
                }
            }
            _frequency = comboCounts;
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
                    double result = PokemonTyping.ratio(attacker, defender);
                    double weight = getFrequency(defender);
                    score += result * weight;
                    if (Config.debug && weight > 0 && weight != 1)
                    {
                        Console.WriteLine($"{attacker} scores {result} against {defender}, weight {weight}");
                    }
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
