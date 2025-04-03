using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaskList.Models;

namespace TaskList.Managers
{
    class ProjectManager
    {
        private List<Project> projects = new List<Project>();
        private string projectFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects.json");

        // Singleton pattern for accessing the ProjectManager instance
        private static ProjectManager instance;
        public static ProjectManager Instance => instance ??= new ProjectManager();

        private ProjectManager()
        {
            LoadProjects();  // Load projects from file if available
        }

        // Method to save projects to a JSON file
        private void SaveProjects()
        {
            string json = JsonConvert.SerializeObject(projects, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(projectFilePath, json);

        }

        // Method to load projects from a JSON file
        private void LoadProjects()
        {
            if (File.Exists(projectFilePath))
            {
                string json = File.ReadAllText(projectFilePath);
                projects = JsonConvert.DeserializeObject<List<Project>>(json) ?? new List<Project>();
                Console.WriteLine($"loaded {projects.Count} projects"); // Debug line
            }
        }

        public List<Project> GetProjects() => projects;

        public void AddProject()
        {
            Console.Write("\nenter the name of the project: ");
            string name = Console.ReadLine();
            if (name.ToLower() == "q") return;

            Console.Write("\nenter a description of the project: ");
            string description = Console.ReadLine();
            Console.Write("\nis it a creative project? (y/n): ");
            bool creative = Console.ReadLine().ToLower() == "y";

            Project project = new Project(name, description, creative);
            projects.Add(project);
            SaveProjects();  // Save projects to file after adding a new one
            Console.WriteLine("\nproject added successfully!");
        }

        public void DeleteProject()
        {
            Console.WriteLine("\nenter the name of the project you want to delete: ");
            string projectName = Console.ReadLine();

            var projectToDelete = projects.FirstOrDefault(p => p.Name.ToLower() == projectName.ToLower());
            if (projectToDelete != null)
            {
                projects.Remove(projectToDelete);
                SaveProjects();  // Save projects to file after deletion
                Console.WriteLine("\nproject deleted successfully!");
            }
            else
            {
                Console.WriteLine("\nproject not found!");
            }
        }

       
        public void DisplayProjects()
        {
            Console.Clear();

            if (projects.Count == 0)
            {
                Console.WriteLine("\nyou have no projects going on");
                return;
            }

            TaskManager taskManager = new TaskManager();

            foreach (var project in projects)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"***** {project.Name} *****");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"you can think about it like this:\n'{project.Description}'");
                Console.ResetColor();
                Console.WriteLine(project.Creative ? "it\'s a creative project" : "it\'s not a particularily creative project");
                taskManager.TasksByProject(project.Name);
            }
        }
    }
}
