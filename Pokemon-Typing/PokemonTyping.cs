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
        public static double O {
            get { return Program.pogo ? 1.6 : 2; }  // Super-effective
        }
        public static double L
        {
            get { return Program.pogo ? .625 : .5; }  // Not very effective
        }
        public static double E
        {
            get { return Program.pogo ? .39025 : 0; }  // "No" effect
        }

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

        // Calculate the damage that an attack would do to a Pokemon with a given type combo
        public static double effectiveness(Type attack, TypeCombo defend)
        {
            return effects[(int)attack][(int)defend.type1] * effects[(int)attack][(int)defend.type2];
        }

        // Calculate the relative type advantage between two Pokemon.
        public static double netDamage(TypeCombo attacker, TypeCombo defender)
        {
            // Damage each Pokemon would do to each other if they used whichever STAB move type is better
            double firstOnSecond = Math.Max(effectiveness(attacker.type1, defender), effectiveness(attacker.type2, defender));
            double secondOnFirst = Math.Max(effectiveness(defender.type1, attacker), effectiveness(defender.type2, attacker));

            // If attacker has the advantage, return the expected remaining HP.
            // If defender has the advantage, return the net HP difference.
            // If both have the same advantage, return 0.
            return
                firstOnSecond > secondOnFirst ? 1 - (secondOnFirst / firstOnSecond) :
                secondOnFirst > firstOnSecond ? -1 + (firstOnSecond / secondOnFirst) :
                0;
        }
    }
}
