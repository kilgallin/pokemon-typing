namespace PokemonTyping {
    public class Program
    {
        // Whether to use Pokemon Go rules or rules for all other games
        public static bool pogo = true;
        // Boolean for whether to write debug logging to console
        public static bool debug = false;
        // Number of digits to include in output. TODO: Don't round internal representation.
        public static int decimalPlaces = 0;
        // Compute scores with best attack type combo instead of STAB type combo
        public static bool unlockAttackTypes = true;
        // Number of times to update the best moveset for each type combo
        public const int bestMovesetIterations = 8;
        // Source file from PvPoke CSV extract
        public static string leagueFile = @"C:\Users\jdk\Downloads\all Rankings.csv";
        // Number of top Pokemon in the league file to consider
        public static int leagueRelevanceLimit = 100;

        // Write log message if debug logging enabled
        public static void log(String message)
        {
            if (debug)
            {
                Console.WriteLine(message);
            }
        }

        // Main entry point
        static void Main(String[] args)
        {
            PokemonRanking.orderCoverage();
            //var coverage = PokemonTyping.superEffectiveCoverage(new List<Type>() { Type.Fairy});

            var x = PokemonRanking.bestMovesets;
            var y = x.GroupBy(p => p.Value).ToDictionary(group => group.Key, group => group.Count()).OrderByDescending(x => x.Value).ToList();
            Console.WriteLine(String.Join('\n', Enumerable.Range(0, y.Count()).Select(i => $"{y[i].Key} with {y[i].Value}")));
            var z = y.Where(g => g.Value == 1).Where(g => x[g.Key] != g.Key);
            var zz = y.Where(g => g.Value == 1).Where(g => x[g.Key] == g.Key);
            var zzz = y.Where(g => x[g.Key] == g.Key);
            var zzzz = x.Where(p => p.Value == new TypeCombo("water", "flying"));
            // Get all scores from best to worst
            List<KeyValuePair<TypeCombo,double>> sortedScores = PokemonRanking.scores.OrderByDescending(x => x.Value).ToList();
            // Write each type and score from that type in sorted order
            Console.WriteLine(String.Join('\n',Enumerable.Range(0, sortedScores.Count()).Select(i => $"{i+1}: {sortedScores[i].Key} with {PokemonRanking.bestMovesets[sortedScores[i].Key]}:{sortedScores[i].Value}")));
            //Console.WriteLine(Enumerable.Sum(sortedScores.Select(e => e.Value)));
        }
    }
}