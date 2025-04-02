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
            }
        }

        public List<Project> GetProjects() => projects;

        public void AddProject()
        {
            Console.Write("\nEnter the name of the project: ");
            string name = Console.ReadLine();
            if (name.ToLower() == "q") return;

            Console.Write("\nEnter a description of the project: ");
            string description = Console.ReadLine();
            Console.Write("\nEnter the start date of the project: ");
            DateTime startDate = DateTime.Parse(Console.ReadLine());
            Console.Write("\nEnter the end date of the project: ");
            DateTime endDate = DateTime.Parse(Console.ReadLine());
            Console.Write("\nIs it a creative project? (y/n): ");
            bool creative = Console.ReadLine().ToLower() == "y";

            Project project = new Project(name, description, startDate, endDate, creative);
            projects.Add(project);
            SaveProjects();  // Save projects to file after adding a new one
            Console.WriteLine("\nProject added successfully!");
        }

        public void DeleteProject()
        {
            Console.WriteLine("\nEnter the name of the project you want to delete: ");
            string projectName = Console.ReadLine();

            var projectToDelete = projects.FirstOrDefault(p => p.Name.ToLower() == projectName.ToLower());
            if (projectToDelete != null)
            {
                projects.Remove(projectToDelete);
                SaveProjects();  // Save projects to file after deletion
                Console.WriteLine("\nProject deleted successfully!");
            }
            else
            {
                Console.WriteLine("\nProject not found!");
            }
        }

        public void DisplayProjects()
        {
            if (projects.Count == 0)
            {
                Console.WriteLine("\nThere are no projects.");
                return;
            }

            foreach (var project in projects)
            {
                Console.WriteLine($"\nProject Name: {project.Name}");
                Console.WriteLine($"Description: {project.Description}");
                Console.WriteLine($"Start Date: {project.StartDate:yyyy-MM-dd}");
                Console.WriteLine($"End Date: {project.EndDate:yyyy-MM-dd}");
                Console.WriteLine(project.Creative ? "Creative Project" : "Non-Creative Project");
            }
        }
    }
}
