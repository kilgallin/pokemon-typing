using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Formats.Asn1.AsnWriter;

namespace Pokemon_Typing
{
    public class PokemonTyping
    {
        public enum Type { Normal, Fire, Water, Grass, Electric, Flying, Ground, Rock, Ice, 
            Fighting, Psychic, Bug, Poison, Ghost, Dragon, Dark, Steel, Fairy, None };
        public enum Outcome { win=1, lose=-1, tie=0}

        public static double[][] effects =
        {
            //         Nml Fir Wat Gra Ele Fly Gro Roc Ice Fit Psy Bug Psn Gho Dgn Drk Ste Fai NONE
            new double[]{1, 1,  1,  1,  1,  1,  1, .5,  1,  1,  1,  1,  1,  0,  1,  1, .5,  1,  1},  // Normal
            new double[]{1,.5, .5,  2,  1,  1,  1, .5,  2,  1,  1,  2,  1,  1, .5,  1,  2,  1,  1},  // Fire
            new double[]{1, 2, .5, .5,  1,  1,  2,  2,  1,  1,  1,  1,  1,  1, .5,  1,  1,  1,  1},  // Water
            new double[]{1,.5,  2, .5,  1, .5,  2,  2,  1,  1,  1, .5, .5,  1, .5,  1, .5,  1,  1},  // Grass
            new double[]{1, 1,  2, .5, .5,  2,  0,  1,  1,  1,  1,  1,  1,  1, .5,  1,  1,  1,  1},  // Electric
            new double[]{1, 1,  1,  2, .5,  1,  1, .5,  1,  2,  1,  2,  1,  1,  1,  1, .5,  1,  1},  // Flying
            new double[]{1, 2,  1, .5,  2,  0,  1,  2,  1,  1,  1, .5,  2,  1,  1,  1,  2,  1,  1},  // Ground
            new double[]{1, 2,  1,  1,  1,  2, .5,  1,  2, .5,  1,  2,  1,  1,  1,  1, .5,  1,  1},  // Rock
            new double[]{1,.5, .5,  2,  1,  2,  2,  1, .5,  1,  1,  1,  1,  1,  2,  1, .5,  1,  1},  // Ice
            new double[]{2, 1,  1,  1,  1, .5,  1,  2,  2,  1, .5, .5, .5,  0,  1,  2,  2, .5,  1},  // Fighting
            new double[]{1, 1,  1,  1,  1,  1,  1,  1,  1,  2, .5,  1,  2,  1,  1,  0, .5,  1,  1},  // Psychic
            new double[]{1,.5,  1,  2,  1, .5,  1,  1,  1, .5,  2,  1, .5, .5,  1,  2, .5, .5,  1},  // Bug
            new double[]{1, 1,  1,  2,  1,  1, .5, .5,  1,  1,  1,  1, .5, .5,  1,  1,  0,  2,  1},  // Poison
            new double[]{0, 1,  1,  1,  1,  1,  1,  1,  1,  1,  2,  1,  1,  2,  1, .5,  1,  1,  1},  // Ghost
            new double[]{1, 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  2,  1, .5,  0,  1},  // Dragon
            new double[]{1, 1,  1,  1,  1,  1,  1,  1,  1, .5,  2,  1,  1,  2,  1, .5,  1, .5,  1},  // Dark
            new double[]{1,.5, .5,  1, .5,  1,  1,  2,  2,  1,  1,  1,  1,  1,  1,  1, .5,  2,  1},  // Steel
            new double[]{1,.5,  1,  1,  1,  1,  1,  1,  1,  2,  1,  1, .5,  1,  2,  2, .5,  1,  1},  // Fairy
            new double[]{1, 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1}   // NONE
        };

        public struct TypeCombo
        {
            public Type type1;
            public Type type2;

            public override string ToString()
            {
                return $"{type1.ToString()}/{type2.ToString()}";
            }
        }

        public struct Pokemon
        {
            public TypeCombo type;
            public List<TypeCombo> wins;
            public List<TypeCombo> losses;
        }

        public static TypeCombo combination(int first, int second)
        {
            return new TypeCombo
            {
                type1 = (Type)first,
                type2 = second != first ? (Type)second : Type.None
            };
        }

        public static double damage(Type attack, TypeCombo defend, bool pogo = true)
        {
            double effect1 = effects[(int)attack][(int)defend.type1];
            double effect2 = effects[(int)attack][(int)defend.type2];
            if (pogo)
            {
                effect1 =
                    effect1 == .5 ? .625 :
                    effect1 == 2 ? 1.6 :
                    effect1 == 0 ? .390625 : 
                    1;

                effect2 =
                    effect2 == .5 ? .625 :
                    effect2 == 2 ? 1.6 :
                    effect2 == 0 ? .390625 : 
                    1;
            }
            return effect1 * effect2;
        }

        public static Outcome compare(TypeCombo first, TypeCombo second)
        {
            double firstOnSecond = Math.Max(damage(first.type1, second), damage(first.type2, second));
            double secondOnFirst = Math.Max(damage(second.type1, first), damage(second.type2, first));
            return
                firstOnSecond > secondOnFirst ? Outcome.win :
                secondOnFirst > firstOnSecond ? Outcome.lose :
                Outcome.tie;
        }

        public static double ratio(TypeCombo first, TypeCombo second)
        {
            double firstOnSecond = Math.Max(damage(first.type1, second), damage(first.type2, second));
            double secondOnFirst = Math.Max(damage(second.type1, first), damage(second.type2, first));
            firstOnSecond = firstOnSecond == 0 ? .1 : firstOnSecond;
            secondOnFirst = secondOnFirst == 0 ? .1 : secondOnFirst;
            return
                firstOnSecond > secondOnFirst ? firstOnSecond / secondOnFirst :
                secondOnFirst > firstOnSecond ? -secondOnFirst / firstOnSecond :
                0;
        }

        public static double calculateScore(TypeCombo attacker,bool debug=false)
        {
            double score = 0;
            for (int defType1 = 0; defType1 < 18; defType1++)
            {
                for (int defType2 = defType1 + 1; defType2 < 19; defType2++)
                {
                    TypeCombo defender = combination(defType1, defType2);
                    //Outcome result = compare(attacker, defender);
                    double result = ratio(attacker, defender);
                    score += result;
                    if (debug)
                    {
                        Console.WriteLine($"{attacker} will {result} against {defender})");
                    }
                }
            }
            return score;
        }

        public static Dictionary<TypeCombo,double> calculateScores()
        {
            Dictionary<TypeCombo, double> scores = new Dictionary<TypeCombo, double>();
            for (int atkType1 = 0; atkType1 < 18; atkType1++)
            {
                for (int atkType2 = atkType1 + 1; atkType2 < 19; atkType2++)
                {
                    TypeCombo attacker = combination(atkType1, atkType2);
                    scores[attacker] = Math.Round(calculateScore(attacker)*1000)/1000;
                }
            }
            return scores;
        }

        public static void main()
        {
            Dictionary<TypeCombo, double> scores = calculateScores();
            //calculateScore(combination((int)Type.Fire, (int)Type.Ground),true);

            var sortedScores = scores.OrderByDescending(x => x.Value).ToList();
            for(int i = 0; i < 50; i++)
            {
                Console.WriteLine($"{sortedScores[i].Key}:{sortedScores[i].Value}");
            }
            //Console.WriteLine(String.Join(',',effects.Select(x => x.Count(e => e == 0).ToString())));
            //Console.WriteLine(Enumerable.Range(0,19).Where(x => effects[0][x] < 1).Select(x => ((Type)x).ToString()).Aggregate((x, y) => x + y));
        }

    }
}
