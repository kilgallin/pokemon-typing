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
            // Get all scores from best to worst
            var sortedScores = PokemonRanking.scores.OrderByDescending(x => x.Value).ToList();
            // Write each type and score from that type in sorted order
            Console.WriteLine(String.Join('\n',Enumerable.Range(0, sortedScores.Count()).Select(i => $"{sortedScores[i].Key}:{sortedScores[i].Value}")));
        }
    }
}