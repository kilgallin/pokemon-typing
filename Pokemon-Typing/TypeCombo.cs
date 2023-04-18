using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonTyping
{
    // Store and render a type combination, representing either a Pokemon, or a pair of attacks a Pokemon could learn
    public class TypeCombo
    {
        // Individual types for a combo
        public Type type1 = Type.None;
        public Type type2 = Type.None;

        // Normalize the types used in the constructor. Refer to PokemonTyping class.
        public TypeCombo(int first, int second)
        {
            if (first > second)
            {
                (first, second) = (second, first);
            }
            type1 = (Type)first;
            type2 = second != first ? (Type)second : Type.None;
        }

        // Make a type combo by names of the types (as found in PvPoke CSV file)
        public TypeCombo(string first, string second) :
            this(Array.IndexOf(PokemonTyping.names, first.ToLower()), Array.IndexOf(PokemonTyping.names, second.ToLower()))
        { }

        // Single-type Pokemon are named by that type. Dual-types are separated by a '/'.
        public override string ToString()
        {
            if (type2 == Type.None)
            {
                return type1.ToString();
            }
            return $"{type1.ToString()}/{type2.ToString()}";
        }

        // Override equality testing and hashing
        public static bool operator ==(TypeCombo self, TypeCombo other) => self.type1 == other.type1 && self.type2 == other.type2;
        public static bool operator !=(TypeCombo self, TypeCombo other) => self.type1 != other.type1 || self.type2 != other.type2;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            return this == (TypeCombo)obj;
        }

        // Give each of the 171 valid combinations a unique number. Normal/Fire = 0*19+1=1,..., Fairy/None=17*19+18=341.
        public override int GetHashCode()
        {
            return (int)type1 * 19 + (int)type2;
        }
    }

}
