namespace PokemonTyping {
    public class Program
    {
        // Whether to use Pokemon Go rules or rules for all other games
        public static bool pogo = true;
        // Boolean for whether to write debug logging to console
        public static bool debug = false;
        // Source file from PvPoke CSV extract
        public static string leagueFile = "";//@"C:\Users\jdk\Downloads\all Rankings.csv";
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
            var x = PokemonRanking.bestMovesets;
            var y = x.GroupBy(p => p.Value).ToDictionary(group => group.Key, group => group.Count()).OrderByDescending(x => x.Value);
            // Get all scores from best to worst
            List<KeyValuePair<TypeCombo,double>> sortedScores = PokemonRanking.scores.OrderByDescending(x => x.Value).ToList();
            // Write each type and score from that type in sorted order
            Console.WriteLine(String.Join('\n',Enumerable.Range(0, sortedScores.Count()).Select(i => $"{sortedScores[i].Key} with {PokemonRanking.bestMovesets[sortedScores[i].Key]}:{sortedScores[i].Value}")));
            //Console.WriteLine(Enumerable.Sum(sortedScores.Select(e => e.Value)));
        }
    }
}