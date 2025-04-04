
namespace TaskList.Models
{
    // base class for relaxation tasks
    abstract class Relaxation
    {
        public string Name { get; set; }

        // constructor
        public Relaxation(string name)
        {
            Name = name;
        }


        // static Random instance for enhanced randomness
        private static readonly Random random = new Random();

        // method to get a random relaxation activity from the file
        public static string GetRandomRelaxationActivity()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "RelaxationActivities.txt");


            if (!File.Exists(filePath))
            {
                return "relaxation activity file not found";
            }

            // make sure there are no empty lines or trailing spaces in the file
            string[] relaxationActivities = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToArray();


            if (relaxationActivities.Length == 0)
            {
                return "no relaxation activities available";
            }

            int randomNumber = random.Next(relaxationActivities.Length);
            return relaxationActivities[randomNumber];

        }
    }
}