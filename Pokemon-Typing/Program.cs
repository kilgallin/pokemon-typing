using static PokemonTyping.PokemonTyping;

namespace PokemonTyping {
    public class Program
    {
        static void Main(String[] args)
        {
            var sortedScores = PokemonRanking.scores.OrderByDescending(x => x.Value).ToList();
            for (int i = 0; i < sortedScores.Count; i++)
            {
                if (PokemonRanking.getFrequency(sortedScores[i].Key) == 0)
                {
                    continue;
                }
                Console.WriteLine($"{sortedScores[i].Key}:{sortedScores[i].Value}");
            }
        }
    }
}