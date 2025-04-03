using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ToDo(string name, string? project, DateTime workon, DateTime? deadline, bool loveIt)
        {
            Name = name;
            Project = project;
            Workon = workon;
            Deadline = deadline;
            Done = false;
            LoveIt = loveIt;
        }

    }
}