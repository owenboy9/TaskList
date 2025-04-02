using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaskList.Models;

namespace TaskList.Managers
{
    class TaskManager
    {
        private List<ToDo> tasks = new List<ToDo>();
        private string taskFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");

        public TaskManager()
        {
            LoadTasks();  // Load tasks from file if available
        }

        // Method to save tasks to a JSON file
        private void SaveTasks()
        {
            string json = JsonConvert.SerializeObject(tasks, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(taskFilePath, json);  // creates file if it doesn't exist
        }

        // Method to load tasks from a JSON file
        private void LoadTasks()
        {
            if (File.Exists(taskFilePath))
            {
                string json = File.ReadAllText(taskFilePath);
                tasks = JsonConvert.DeserializeObject<List<ToDo>>(json) ?? new List<ToDo>();
            }
        }

        public void AddTask()
        {
            Console.Write("\nEnter the task name: \nOr press 'q' to return to the main menu: ");
            string name = Console.ReadLine();
            if (name.ToLower() == "q") return; // exit to main menu if user presses 'q'

            Console.Write("\nWhich project does this task belong to? ");
            string projectName = Console.ReadLine();

            // Check if project exists (replace this with the actual project list or load it from ProjectManager)
            var projectExists = ProjectManager.Instance.GetProjects().Any(p => p.Name.ToLower() == projectName.ToLower());

            if (!projectExists)
            {
                Console.WriteLine("\nThis project does not exist. Would you like to add it now? (y/n)");
                if (Console.ReadLine().ToLower() == "y")
                {
                    ProjectManager.Instance.AddProject(); // Add the project if user chooses 'y'
                }
                else
                {
                    return;  // Return if user doesn't want to add the project
                }
            }

            Console.Write("\nWhen do you need to be done with it? ");
            DateTime deadline = DateTime.Parse(Console.ReadLine());
            Console.Write("\nWhen are you planning to work on it? ");
            DateTime workon = DateTime.Parse(Console.ReadLine());
            Console.Write("\nDo you love this task? (y/n) ");
            bool loveIt = Console.ReadLine().ToLower() == "y";

            ToDo task = new ToDo(name, projectName, workon, deadline, loveIt);
            tasks.Add(task);
            SaveTasks();  // Save tasks to file after adding a new one
            Console.WriteLine("\nTask added successfully!");
        }

        public void DeleteTask()
        {
            Console.WriteLine("\nEnter the name of the task you want to delete: ");
            string taskName = Console.ReadLine();

            var taskToDelete = tasks.FirstOrDefault(t => t.Name.ToLower() == taskName.ToLower());
            if (taskToDelete != null)
            {
                tasks.Remove(taskToDelete);
                SaveTasks();  // Save tasks to file after deletion
                Console.WriteLine("\nTask deleted successfully!");
            }
            else
            {
                Console.WriteLine("\nTask not found!");
            }
        }

        public void DisplayTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("\nThere are no tasks in your list.");
                return;
            }

            var groupedTasks = tasks.GroupBy(t => t.Workon.Date);

            Console.WriteLine("\nHere is your to-do list:");

            foreach (var dayGroup in groupedTasks)
            {
                Console.WriteLine($"\n======> * {dayGroup.Key:yyyy-MM-dd} * <======\n");
                int taskCount = 0;

                foreach (var task in dayGroup)
                {
                    Console.WriteLine($"** {task.Name} ==> Project: {task.Project}, Deadline: {task.Deadline:yyyy-MM-dd}, Done: {task.Done}, Loved: {task.LoveIt}");
                    taskCount++;

                    if (taskCount % 2 == 0 && taskCount < dayGroup.Count())
                    {
                        string relaxationActivity = Relaxation.GetRandomRelaxationActivity();
                        Console.WriteLine($"\n*** {relaxationActivity} ***\n");
                    }
                }
            }
        }
    }
}
