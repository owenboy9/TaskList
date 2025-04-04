

namespace TaskList.Models
{
    class Project
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public bool Creative { get; set; }

        // constructor
        public Project(string name, string? description, bool creative)
        {
            Name = name;
            Description = description;
            Creative = creative;
        }

        public void MarkAsCreative() => Creative = true;
        public void MarkAsNotCreative() => Creative = false;

    }
}