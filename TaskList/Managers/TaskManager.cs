using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
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
            Console.Write("\nenter the task name or press 'q' to return to the main menu:   ");
            string name = Console.ReadLine();
            if (name.ToLower() == "q") return; // exit to main menu if user presses 'q'

            Console.Write("\nwhich project does this task belong to? ");
            string projectName = Console.ReadLine();

            // Check if project exists (replace this with the actual project list or load it from ProjectManager)
            var projectExists = ProjectManager.Instance.GetProjects().Any(p => p.Name.ToLower() == projectName.ToLower());

            if (!projectExists)
            {
                Console.WriteLine("\nthis project does not exist. would you like to add it now? (y/n)");
                if (Console.ReadLine().ToLower() == "y")
                {
                    ProjectManager.Instance.AddProject(); // Add the project if user chooses 'y'
                }
                else
                {
                    return;  // Return if user doesn't want to add the project
                }
            }

            Console.Write("\nwhen do you need to be done with it? ");
            DateTime deadline = DateTime.Parse(Console.ReadLine());
            Console.Write("\nwhen are you planning to work on it? ");
            DateTime workon = DateTime.Parse(Console.ReadLine());
            Console.Write("\ndo you love this task? (y/n) ");
            bool loveIt = Console.ReadLine().ToLower() == "y" ? true : false;

            ToDo task = new ToDo(name, projectName, workon, deadline, loveIt);
            tasks.Add(task);
            SaveTasks();  // Save tasks to file after adding a new one
            Console.WriteLine("\ntask added successfully!");
        }

        public void DeleteTask(string taskName)
        {

            var taskToDelete = tasks.FirstOrDefault(t => t.Name.ToLower() == taskName.ToLower());
            if (taskToDelete != null)
            {
                tasks.Remove(taskToDelete);
                SaveTasks();  // save tasks to file after deletion
                Console.WriteLine("\ntask deleted successfully!");
            }
            else
            {
                Console.WriteLine("\ntask not found!");
            }
        }

        public void EditTask(string taskName)
        {
            var taskToEdit = tasks.FirstOrDefault(t => t.Name.ToLower() == taskName.ToLower());

            if (taskToEdit == null)
            {
                Console.WriteLine("\ntask not found!");
                return;
            }

            Console.WriteLine("\nyou can always press 'q' to return to the main menu");

            // Edit task name
            Console.Write("\nchange task name (press enter to keep current): ");
            string name = Console.ReadLine();
            if (name.ToLower() == "q") return;
            if (!string.IsNullOrWhiteSpace(name)) taskToEdit.Name = name;

            // Edit project
            Console.Write("\nmove to another project (press enter to keep current): ");
            string projectName = Console.ReadLine();
            if (projectName.ToLower() == "q") return;
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                var projectExists = ProjectManager.Instance.GetProjects()
                    .Any(p => p.Name.ToLower() == projectName.ToLower());

                if (!projectExists)
                {
                    Console.WriteLine("\nthis project does not exist. would you like to add it now? (y/n)");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        ProjectManager.Instance.AddProject();
                    }
                    else
                    {
                        return;
                    }
                }
                taskToEdit.Project = projectName;
            }

            // Edit deadline
            Console.Write("\nchange deadline (press enter to keep current): ");
            string deadlineInput = Console.ReadLine();
            if (deadlineInput.ToLower() == "q") return;
            if (!string.IsNullOrWhiteSpace(deadlineInput) && DateTime.TryParse(deadlineInput, out DateTime newDeadline))
            {
                taskToEdit.Deadline = newDeadline;
            }

            // Edit workon date
            Console.Write("\nany new plans for working on it? (press enter to keep current): ");
            string workonInput = Console.ReadLine();
            if (workonInput.ToLower() == "q") return;
            if (!string.IsNullOrWhiteSpace(workonInput) && DateTime.TryParse(workonInput, out DateTime newWorkon))
            {
                taskToEdit.Workon = newWorkon;
            }

            // Edit "LoveIt" status
            Console.Write("\ndo you love this task? (y/n, press enter to keep current): ");
            string loveItInput = Console.ReadLine();
            if (loveItInput.ToLower() == "q") return;
            if (!string.IsNullOrWhiteSpace(loveItInput))
            {
                taskToEdit.LoveIt = loveItInput.ToLower() == "y";
            }

            // Save changes
            SaveTasks();
            Console.WriteLine("\ntask updated successfully!");
        }

        public void MarkAsDone(string taskName)
        {
            var taskToMark = tasks.FirstOrDefault(t => t.Name.ToLower() == taskName.ToLower());
            if (taskToMark != null)
            {
                taskToMark.Done = true;
                SaveTasks();  // save tasks to file after marking as done
                Console.WriteLine("\ntask marked as done!");
            }
            else
            {
                Console.WriteLine("\ntask not found!");
            }
        }

        public void MarkAsNotDone(string taskName)
        {
            var taskToMark = tasks.FirstOrDefault(t => t.Name.ToLower() == taskName.ToLower());
            if (taskToMark != null)
            {
                taskToMark.Done = false;
                SaveTasks();  // save tasks to file after marking as done
                Console.WriteLine("\ntask marked as done!");
            }
            else
            {
                Console.WriteLine("\ntask not found!");
            }
        }

        public void TasksByProject(string projName)
        {
            var projectTasks = tasks
                .Where(task => task.Project.Equals(projName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (projectTasks.Count == 0)
            {
                Console.WriteLine("no tasks assigned to this project");
                return;
            }

            Console.WriteLine("\n* this project includes:");
            foreach (var task in projectTasks)
            {
                Console.Write($"==>  {task.Name}        ");
                Console.WriteLine(task.LoveIt ? "and you love it!" : "it\'s just something to get over with");
            }
        }

        /*
        public void IndividualMenu(ToDo task)
        {
            string[] options = {"edit task", "delete task", "mark as done", "mark as not done"};
            int selectedIndex = 0;
            ConsoleKey key;

            do
            {
                Console.Clear();

                Console.WriteLine($"\n===> * {task.Name} * <===");

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($" * {options[i]} * ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"   {options[i]}   ");
                    }
                }
                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.LeftArrow)
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                else if (key == ConsoleKey.RightArrow)
                    selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
            } while (key != ConsoleKey.Enter);

            HandleSelection(options[selectedIndex], task);
        }
        

        public void HandleSelection(ToDo task)
        {
            string[] options = { "edit task", "delete task", "mark as done", "mark as not done" };
            int selectedIndex = 0;
            ConsoleKey key;

            do
            {
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($" * {options[i]} * ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"   {options[i]}   ");
                    }
                }

                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.LeftArrow)
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                else if (key == ConsoleKey.RightArrow)
                    selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;

                // apply changes directly to the task and refresh immediately
                if (key == ConsoleKey.Enter)
                {
                    switch (options[selectedIndex])
                    {
                        case "edit task":
                            EditTask();
                            break;
                        case "delete task":
                            tasks.Remove(task);
                            SaveTasks();
                            return; // exit menu since task was deleted
                        case "mark as done":
                            task.MarkAsDone();
                            SaveTasks();
                            break;
                        case "mark as not done":
                            task.MarkAsNotDone();
                            SaveTasks();
                            break;
                    }
                }

            } while (key != ConsoleKey.Escape);
        }

        private void DisplayTaskMenu(ToDo task, bool isSelected)
        {
            string[] options = { "edit task", "delete task", "mark as done", "mark as not done" };

            foreach (var option in options)
            {
                if (isSelected)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.Write($"  {option}  ");
                Console.ResetColor();
            }
            Console.WriteLine("\n");
        }
        */

        public void DisplayTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("\nthere are no tasks in your list");
                return;
            }

            Console.Clear();
            var groupedTasks = tasks.GroupBy(t => t.Workon.Date).OrderBy(g => g.Key);  // ensure dates are in chronological order

            Console.WriteLine("\nhere is your to-do list:");

            foreach (var dayGroup in groupedTasks)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\n======> * {dayGroup.Key:yyyy-MM-dd} * <======\n");
                Console.ResetColor();

                int taskCount = 0;
                var options = new List<string>();

                foreach (var task in dayGroup)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"*** {task.Name} ***");
                    Console.ResetColor();
                    Console.WriteLine($"project: {task.Project}, deadline: {task.Deadline:MM-dd}");
                    Console.Write(task.Done ? "done!" : "working on it..........");
                    Console.WriteLine(task.LoveIt ? "you love this!\n" : "just get it over with\n");
                    options.Add(task.Name.ToString());
                    taskCount++;

                    if (taskCount % 2 == 0 && taskCount < dayGroup.Count())
                    {
                        string relaxationActivity = Relaxation.GetRandomRelaxationActivity().ToUpper();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"*** {relaxationActivity} ***\n");
                        Console.ResetColor();
                    }
                }

                Menu(options.ToArray());
            }
        }

        public void Menu(string[] options)
        {
            int selectedIndex = 0;
            ConsoleKey key;
            do
            {
                Console.WriteLine("==> choose a task to modify <==");

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"==> {options[i]} <==");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"   {options[i]}");
                    }
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                else if (key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
            } while (key != ConsoleKey.Enter);

            // HandleSelection(options[selectedIndex]);
        }

        static void HandleSelection(string selection)
        {
            Console.WriteLine($"\nYou selected: {selection}");

        }
    }
}
