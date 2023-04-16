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
        public Type type1;
        public Type type2;

        public override string ToString()
        {
            if (type2 == Type.None)
            {
                return type1.ToString();
            }
            return $"{type1.ToString()}/{type2.ToString()}";
        }

        public static bool operator ==(TypeCombo self, TypeCombo other) => self.type1 == other.type1 && self.type2 == other.type2;
        public static bool operator !=(TypeCombo self, TypeCombo other) => self.type1 != other.type1 || self.type2 != other.type2;

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

}
