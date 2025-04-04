

namespace TaskList.Models
{
    class ToDo
    {
        public string Name { get; set; }
        public string? Project { get; set; }
        public DateTime Workon { get; set; }
        public DateTime? Deadline { get; set; }
        public bool Done { get; set; }
        public bool LoveIt { get; set; }

        // constructor
        public ToDo(string name, bool loveIt, string? project = null, DateTime? deadline = null, DateTime? workon = null)
        {
            Name = name;
            Project = project;
            Workon = workon ?? DateTime.Today; // default to today if null
            Deadline = deadline;
            Done = false;
            LoveIt = loveIt;
        }

    }
}