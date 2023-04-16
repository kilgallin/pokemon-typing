using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonTyping
{
    // Store and render a type combination
    public class TypeCombo
    {
        public Type type1 = Type.None;
        public Type type2 = Type.None;
        private static string[] names
        {
            get
            {
                return Enumerable.Range(0, 19).Select(i => ( (Type)i ).ToString().ToLower()).ToArray();
            }
        }
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
            this(Array.IndexOf(names, first.ToLower()), Array.IndexOf(names, second.ToLower()))
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

        public override int GetHashCode()
        {
            return (int)type1 * 19 + (int)type2;
        }
    }

}
