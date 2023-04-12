using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon_Typing
{
    public class PokemonTyping
    {
        // The 18 base types, and a "None" option to represent single-type Pokemon and computational purposes
        public enum Type { Normal, Fire, Water, Grass, Electric, Flying, Ground, Rock, Ice, 
            Fighting, Psychic, Bug, Poison, Ghost, Dragon, Dark, Steel, Fairy, None };
        
        // Tallying matchups
        public enum Outcome { win=1, lose=-1, tie=0}

        // Multipliers for no-modifier attacks, super-effective attacks, not-very-effective attacks, and
        // no-effect attacks, respectively. Symbols are chosen for visual representation in matrix below.
        public static double _ = 1;
        public static double O = 2;
        public static double L = .5;
        public static double E = 0;

        // Matrix of type effectiveness matchups. Rows are attackers and columns are defenders.
        // Ordering matches the Type enum above to allow indexing. effects[attacker][defender]
        // gives the multiplier for that single-type matchup, with the params as indexed by enum.
        public static double[][] effects =
        {
            //Defender->Nm Fir Wat Gra Ele Fly Gnd Roc Ice Fgt Psy Bug Poi Gho Dgn Drk Ste Fai NONE  Attacker \/
            new double[]{_, _,  _,  _,  _,  _,  _,  L,  _,  _,  _,  _,  _,  E,  _,  _,  L,  _,  _},  // Normal
            new double[]{_, L,  L,  O,  _,  _,  _,  L,  O,  _,  _,  O,  _,  _,  L,  _,  O,  _,  _},  // Fire
            new double[]{_, O,  L,  L,  _,  _,  O,  O,  _,  _,  _,  _,  _,  _,  L,  _,  _,  _,  _},  // Water
            new double[]{_, L,  O,  L,  _,  L,  O,  O,  _,  _,  _,  L,  L,  _,  L,  _,  L,  _,  _},  // Grass
            new double[]{_, _,  O,  L,  L,  O,  E,  _,  _,  _,  _,  _,  _,  _,  L,  _,  _,  _,  _},  // Electric
            new double[]{_, _,  _,  O,  L,  _,  _,  L,  _,  O,  _,  O,  _,  _,  _,  _,  L,  _,  _},  // Flying
            new double[]{_, O,  _,  L,  O,  E,  _,  O,  _,  _,  _,  L,  O,  _,  _,  _,  O,  _,  _},  // Ground
            new double[]{_, O,  _,  _,  _,  O,  L,  _,  O,  L,  _,  O,  _,  _,  _,  _,  L,  _,  _},  // Rock
            new double[]{_, L,  L,  O,  _,  O,  O,  _,  L,  _,  _,  _,  _,  _,  O,  _,  L,  _,  _},  // Ice
            new double[]{O, _,  _,  _,  _,  L,  _,  O,  O,  _,  L,  L,  L,  E,  _,  O,  O,  L,  _},  // Fighting
            new double[]{_, _,  _,  _,  _,  _,  _,  _,  _,  O,  L,  _,  O,  _,  _,  E,  L,  _,  _},  // Psychic
            new double[]{_, L,  _,  O,  _,  L,  _,  _,  _,  L,  O,  _,  L,  L,  _,  O,  L,  L,  _},  // Bug
            new double[]{_, _,  _,  O,  _,  _,  L,  L,  _,  _,  _,  _,  L,  L,  _,  _,  E,  O,  _},  // Poison
            new double[]{E, _,  _,  _,  _,  _,  _,  _,  _,  _,  O,  _,  _,  O,  _,  L,  _,  _,  _},  // Ghost
            new double[]{_, _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  O,  _,  L,  E,  _},  // Dragon
            new double[]{_, _,  _,  _,  _,  _,  _,  _,  _,  L,  O,  _,  _,  O,  _,  L,  _,  L,  _},  // Dark
            new double[]{_, L,  L,  _,  L,  _,  _,  O,  O,  _,  _,  _,  _,  _,  _,  _,  L,  O,  _},  // Steel
            new double[]{_, L,  _,  _,  _,  _,  _,  _,  _,  O,  _,  _,  L,  _,  O,  O,  L,  _,  _},  // Fairy
            new double[]{_, _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _}   // NONE
        };

        // Store and render a type combination
        public class TypeCombo
        {
            public Type type1;
            public Type type2;

            public override string ToString()
            {
                if (type2 == Type.None)
                {
                    return type1.ToString().PadRight(18);
                }
                return $"{type1.ToString()}/{type2.ToString()}".PadRight(18);
            }

            public TypeCombo(int first, int second)
            {
                if (first > second)
                {
                    (first, second) = (second, first);
                }
                type1 = (Type)first;
                type2 = second != first ? (Type)second : Type.None;
            }
        }

        // Calculate the damage that an attack would do to a Pokemon with a given type combo
        public static double damage(Type attack, TypeCombo defend)
        {
            return effects[(int)attack][(int)defend.type1] * effects[(int)attack][(int)defend.type2];
        }

        // Calculate the relative type advantage between two Pokemon.
        public static double ratio(TypeCombo attacker, TypeCombo defender)
        {
            // Damage each Pokemon would do to each other if they used whichever STAB move type is better
            double firstOnSecond = Math.Max(damage(attacker.type1, defender), damage(attacker.type2, defender));
            double secondOnFirst = Math.Max(damage(defender.type1, attacker), damage(defender.type2, attacker));

            // Avoid dividing by zero. TODO: Redesign scoring
            firstOnSecond = firstOnSecond == 0 ? .25 : firstOnSecond;
            secondOnFirst = secondOnFirst == 0 ? .25 : secondOnFirst;

            // If attacker has the advantage, return the relative modifier.
            // If defender has the advantage, subtract the score they would get.
            // If both have the same advantage, return 0.
            return
                firstOnSecond > secondOnFirst ? firstOnSecond / secondOnFirst :
                secondOnFirst > firstOnSecond ? -secondOnFirst / firstOnSecond :
                0;
        }

        // Text rendering for the outcome of a matchup
        public static Outcome compare(TypeCombo first, TypeCombo second)
        {
            double outcome = ratio(first, second);
            return
                outcome > 0 ? Outcome.win :
                outcome < 0 ? Outcome.lose :
                Outcome.tie;
        }

        // Total the score a given attacker would net across all possible defenders
        public static double calculateScore(TypeCombo attacker)
        {
            double score = 0;
            for (int defType1 = 0; defType1 < 18; defType1++)
            {
                for (int defType2 = defType1 + 1; defType2 < 19; defType2++)
                {
                    TypeCombo defender = new TypeCombo(defType1, defType2);
                    double result = ratio(attacker, defender);
                    score += result;// * (double)frequency.GetValueOrDefault(defender); TODO: frequency broken?
                    if (debug)
                    {
                        //Console.WriteLine($"{attacker} scores {result} against {defender})");
                    }
                }
            }
            return score;
        }

        // Calculate scores for all possible attackers
        public static Dictionary<TypeCombo,double> calculateScores()
        {
            Dictionary<TypeCombo, double> scores = new Dictionary<TypeCombo, double>();
            for (int atkType1 = 0; atkType1 < 18; atkType1++)
            {
                for (int atkType2 = atkType1 + 1; atkType2 < 19; atkType2++)
                {
                    TypeCombo attacker = new TypeCombo(atkType1, atkType2);
                    scores[attacker] = Math.Round(calculateScore(attacker)*1000)/1000;
                }
            }
            return scores;
        }

        // Read pvpoke.com CSV file and parse type combinations from top-tier Pokemon.
        public static Dictionary<TypeCombo, int> getFrequency()
        {
            string[] allPokemon = File.ReadAllLines(@"C:\Users\jdk\Downloads\all Rankings.csv");
            Dictionary<TypeCombo, int> comboCounts = new Dictionary<TypeCombo, int>();
            String[] typeNames = Enumerable.Range(0, 19).Select(i => ((Type)i).ToString().ToLower()).ToArray();
            for(int i = 0; i < 100; i++)
            {
                string[] pokemon = allPokemon[i].Split(',');
                TypeCombo type = new TypeCombo(Array.IndexOf(typeNames, pokemon[3]), Array.IndexOf(typeNames, pokemon[4]));
                if (debug)
                {
                    Console.WriteLine(pokemon[0]);
                }
                if (comboCounts.ContainsKey(type))
                {
                    comboCounts[type]++;
                } else
                {
                    comboCounts[type] = 1;
                }
            }
            return comboCounts;
        }

        public static Dictionary<TypeCombo, int> frequency;
        public static bool pogo = false;
        public static bool debug = false;
        public static void main()
        {
            frequency = getFrequency();
            Dictionary<TypeCombo, double> scores = calculateScores();

            var sortedScores = scores.OrderByDescending(x => x.Value).ToList();
            for(int i = 0; i < 18; i++)
            {
                Console.WriteLine($"{sortedScores[i].Key}:{sortedScores[i].Value}");
            }
        }
    }
}
