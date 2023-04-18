using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonTyping
{
    // The 18 base types, and a "None" option to represent single-type Pokemon and computational purposes
    public enum Type
    {
        Normal, Fire, Water, Grass, Electric, Flying, Ground, Rock, Ice,
        Fighting, Psychic, Bug, Poison, Ghost, Dragon, Dark, Steel, Fairy, None
    };

    public class PokemonTyping
    {
        // Multipliers for no-modifier attacks, super-effective attacks, not-very-effective attacks, and
        // no-effect attacks, respectively. Symbols are chosen for visual representation in matrix below.
        // Modifiers account for Pokemon Go type effectiveness multipliers vs those in all other games.
        public static double _
        {
            get { return 1; }  // Normal damage
        }
        public static double W {
            get { return Program.pogo ? 1.6 : 2; }  // Super-effective
        }
        public static double L
        {
            get { return Program.pogo ? .625 : .5; }  // Not very effective
        }
        public static double O
        {
            get { return Program.pogo ? .39025 : 0; }  // "No" effect
        }

        // Matrix of type effectiveness matchups. Rows are attackers and columns are defenders.
        // Ordering matches the Type enum above to allow indexing. effects[attacker][defender]
        // gives the multiplier for that single-type matchup, with the params as indexed by enum.
        public static double[][] effects =
        {
            //Defender->Nm Fir Wat Gra Ele Fly Gnd Roc Ice Fgt Psy Bug Poi Gho Dgn Drk Ste Fai NONE  Attacker \/
            new double[]{_, _,  _,  _,  _,  _,  _,  L,  _,  _,  _,  _,  _,  O,  _,  _,  L,  _,  _},  // Normal
            new double[]{_, L,  L,  W,  _,  _,  _,  L,  W,  _,  _,  W,  _,  _,  L,  _,  W,  _,  _},  // Fire
            new double[]{_, W,  L,  L,  _,  _,  W,  W,  _,  _,  _,  _,  _,  _,  L,  _,  _,  _,  _},  // Water
            new double[]{_, L,  W,  L,  _,  L,  W,  W,  _,  _,  _,  L,  L,  _,  L,  _,  L,  _,  _},  // Grass
            new double[]{_, _,  W,  L,  L,  W,  O,  _,  _,  _,  _,  _,  _,  _,  L,  _,  _,  _,  _},  // Electric
            new double[]{_, _,  _,  W,  L,  _,  _,  L,  _,  W,  _,  W,  _,  _,  _,  _,  L,  _,  _},  // Flying
            new double[]{_, W,  _,  L,  W,  O,  _,  W,  _,  _,  _,  L,  W,  _,  _,  _,  W,  _,  _},  // Ground
            new double[]{_, W,  _,  _,  _,  W,  L,  _,  W,  L,  _,  W,  _,  _,  _,  _,  L,  _,  _},  // Rock
            new double[]{_, L,  L,  W,  _,  W,  W,  _,  L,  _,  _,  _,  _,  _,  W,  _,  L,  _,  _},  // Ice
            new double[]{W, _,  _,  _,  _,  L,  _,  W,  W,  _,  L,  L,  L,  O,  _,  W,  W,  L,  _},  // Fighting
            new double[]{_, _,  _,  _,  _,  _,  _,  _,  _,  W,  L,  _,  W,  _,  _,  O,  L,  _,  _},  // Psychic
            new double[]{_, L,  _,  W,  _,  L,  _,  _,  _,  L,  W,  _,  L,  L,  _,  W,  L,  L,  _},  // Bug
            new double[]{_, _,  _,  W,  _,  _,  L,  L,  _,  _,  _,  _,  L,  L,  _,  _,  O,  W,  _},  // Poison
            new double[]{O, _,  _,  _,  _,  _,  _,  _,  _,  _,  W,  _,  _,  W,  _,  L,  _,  _,  _},  // Ghost
            new double[]{_, _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  W,  _,  L,  O,  _},  // Dragon
            new double[]{_, _,  _,  _,  _,  _,  _,  _,  _,  L,  W,  _,  _,  W,  _,  L,  _,  L,  _},  // Dark
            new double[]{_, L,  L,  _,  L,  _,  _,  W,  W,  _,  _,  _,  _,  _,  _,  _,  L,  W,  _},  // Steel
            new double[]{_, L,  _,  _,  _,  _,  _,  _,  _,  W,  _,  _,  L,  _,  W,  W,  L,  _,  _},  // Fairy
            new double[]{_, _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _,  _}   // NONE
        };

        public static List<TypeCombo> allTypes
        {
            get
            {
                List<TypeCombo> types = new List<TypeCombo>();
                forAllTypes(
                    type => types.Add(type)
                );
                return types;
            }
        }

        // Calculate the damage that an attack would do to a Pokemon with a given type combo
        public static double effectiveness(Type attack, TypeCombo defend)
        {
            return effects[(int)attack][(int)defend.type1] * effects[(int)attack][(int)defend.type2];
        }

        // Calculate the relative type advantage between two Pokemon.
        public static double netDamage(TypeCombo attacker, TypeCombo defender, TypeCombo attackerMoves, TypeCombo defenderMoves)
        {
            // Damage each Pokemon would do to each other if they used whichever STAB move type is better
            double firstOnSecond = Math.Max(effectiveness(attackerMoves.type1, defender), effectiveness(attackerMoves.type2, defender));
            double secondOnFirst = Math.Max(effectiveness(defenderMoves.type1, attacker), effectiveness(defenderMoves.type2, attacker));

            // If attacker has the advantage, return the expected remaining HP.
            // If defender has the advantage, return the net HP difference.
            // If both have the same advantage, return 0.
            return
                firstOnSecond > secondOnFirst ? 1 - (secondOnFirst / firstOnSecond) :
                secondOnFirst > firstOnSecond ? -1 + (firstOnSecond / secondOnFirst) :
                0;
        }

        public static double netDamage(TypeCombo attacker, TypeCombo defender)
        {
            return netDamage(attacker, defender, attacker, defender);
        }

        public static void forAllTypes(Action<TypeCombo> action)
        {
            for (int type1 = 0; type1 < 18; type1++)
            {
                for (int type2 = type1 + 1; type2 < 19; type2++)
                {
                    action(new TypeCombo(type1, type2));
                }
            }
        }
    }
}
