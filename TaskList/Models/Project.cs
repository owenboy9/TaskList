using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TaskList.Models
{
    class Project
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Creative { get; set; }

        // constructor
        public Project(string name, string? description, DateTime? startDate, DateTime? endDate, bool creative)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Creative = creative;
        }

        public void MarkAsCreative() => Creative = true;
        public void MarkAsNotCreative() => Creative = false;

    }
}