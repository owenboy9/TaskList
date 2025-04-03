using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TaskList.Models;
using TaskList.Managers;

namespace TaskList
{
    class Program
    {
        // instantiate all the appropriate classes
        static ToDo task = new ToDo("task name", "project name", DateTime.Now, DateTime.Now.AddDays(1), false);
        static Project project = new Project("project name", "project description", true);
        static TaskManager taskManager = new TaskManager();
        static ProjectManager projectManager = ProjectManager.Instance;

        static void Main()
        {
            ShowMenu();
        }

        static void ShowMenu()
        {
            string[] options = { "display tasks", "display projects", "add a new task", "add a new project", "exit" };
            int selectedIndex = 0;
            ConsoleKey key;

            do
            {
                Console.Clear();
                Console.WriteLine("===> select an option <===");

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

            // Call the appropriate method based on the selected index
            HandleSelection(options[selectedIndex]);
        }

        static void HandleSelection(string selection)
        {
            Console.Clear();
            switch (selection)
            {
                case "display tasks":
                    taskManager.DisplayTasks(); // Call instance method
                    break;
                case "display projects":

                    projectManager.DisplayProjects(); // Call instance method
                    break;
                case "add a new task":

                    taskManager.AddTask(); // Call instance method
                    break;
                case "add a new project":

                    projectManager.AddProject(); // Call instance method
                    break;
                case "exit":

                    Console.WriteLine("goodbye!");
                    return;
            }

            Console.WriteLine("\nPress any key to return to the main menu");
            Console.ReadKey();
            ShowMenu();
        }
    }
}