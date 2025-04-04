using Newtonsoft.Json;
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
            DateTime deadline; // declare variable
            try
            {
                deadline = DateTime.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("\ninvalid date format. please try again.");
                return;
            }

            Console.Write("\nwhen are you planning to work on it? ");
            string workonInput = Console.ReadLine();
            DateTime workon;
            // default to today's date
            if (string.IsNullOrWhiteSpace(workonInput))
            { 
                workon = DateTime.Today; // default to today if no input
            }
            else
            
            {
             // catch incorrect user input
                try
                {
                    workon = DateTime.Parse(workonInput);
                }
                catch (FormatException)
                {
                    Console.WriteLine("\ninvalid date format. please try again.");
                    return;
                }   
               
            }

            Console.Write("\ndo you love this task? (y/n) ");
            bool loveIt = Console.ReadLine().ToLower() == "y" ? true : false;

            ToDo task = new ToDo(name, loveIt, projectName, workon, deadline);
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
            DisplayTasks();
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

            DisplayTasks();
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
            DisplayTasks();
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
            DisplayTasks();
        }

        public void TasksByProject(string projName)
        {
            var projectTasks = tasks
                .Where(task => task.Project.Equals(projName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (projectTasks.Count == 0)
            {
                Console.WriteLine("* no tasks assigned to this project");
                return;
            }

            Console.WriteLine("\n* this project includes:");
            foreach (var task in projectTasks)
            {
                Console.Write($"==>  {task.Name}        ");
                Console.WriteLine(task.LoveIt ? "and you love it!" : "it\'s just something to get over with");
            }
        }

        public void DisplayTasks()
        {
            Console.Clear();

            if (tasks.Count == 0)
            {
                Console.WriteLine("\n======> * there are no tasks in your list * <======\n");
                return;
            }

            // check for overdue undone tasks and adjust Workon
            foreach (var task in tasks)
            {
                if (!task.Done && task.Deadline.HasValue && DateTime.Today > task.Deadline.Value)
                {
                    task.Workon = DateTime.Today;
                }
            }

            var groupedTasks = tasks.GroupBy(t => t.Workon.Date).OrderBy(g => g.Key);  // ensure dates are in chronological order

            Console.WriteLine("\n======> * HERE\'S YOUR TO-DO LIST * <======");
            var options = new List<string>();

            foreach (var dayGroup in groupedTasks)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\n======> * {dayGroup.Key:yyyy-MM-dd} * <======\n");
                Console.ResetColor();

                int taskCount = 0;
                

                foreach (var task in dayGroup)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"*** {task.Name} ***");
                    Console.ResetColor();
                    if (!task.Done && task.Deadline.HasValue && task.Workon > task.Deadline.Value)
                    {
                        Console.Write($"project: {task.Project}");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"    deadline: {task.Deadline:MM-dd}");
                        Console.ResetColor();
                    }
                    else
                        Console.WriteLine($"project: {task.Project}     deadline: {task.Deadline:MM-dd}");
                    Console.Write(task.Done ? "done!.........." : "working on it..........");
                    Console.WriteLine(task.LoveIt ? "such a great thing to do!\n" : "just one of those things you sorta gotta get outta your way\n");
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
            }
            Console.WriteLine("\n\n\n");
            Menu(options.ToArray());

        }

        public void Menu(string[] options)
        {
            int selectedIndex = 0;
            ConsoleKey key;

            Console.CursorVisible = false; // hide cursor for a clean ui

            // calculate fixed line positions
            int footerLine = Console.WindowHeight - 2;
            int menuLine = footerLine - 1;
            int headerLine = menuLine - 1;

            // clear a specific console line
            void ClearLine(int line)
            {
                Console.SetCursorPosition(0, line);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            // print header
            ClearLine(headerLine);
            Console.SetCursorPosition(0, headerLine);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("==> choose a task to modify or press esc to skip <==");
            Console.ResetColor();

            // print footer
            ClearLine(footerLine);
            Console.SetCursorPosition(0, footerLine);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("=========================>*<========================".PadRight(Console.WindowWidth));
            Console.ResetColor();

            do
            {
                // move cursor to the menu line
                ClearLine(menuLine);
                Console.SetCursorPosition(0, menuLine);

                // print highlighted option
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"                    => {options[selectedIndex]} <=");
                Console.ResetColor();

                // read user input
                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                else if (key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
                else if (key == ConsoleKey.Escape)
                    return; // exit menu if user presses esc

            } while (key != ConsoleKey.Enter);

            HandleSelection(options[selectedIndex]);
        }





        public void HandleSelection(string selection)
        {
            string taskName = selection;
            string[] options = { "edit task", "delete task", "mark as done", "mark as not done", "change nothing" };
            int selectedIndex = 0;
            ConsoleKey key;
            do
            { 
                Console.Clear();
                Console.WriteLine($"\n===> * {taskName} * <===");
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($"  *{options[i]}*  ");
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

            IndividualTaskEdit(taskName, options[selectedIndex]);
        }

        public void IndividualTaskEdit(string taskName, string selection)
        {
            switch (selection)
            { 
                case "edit task":
                    EditTask(taskName);
                    break;
                case "delete task":
                    DeleteTask(taskName);
                    break;
                case "mark as done":
                    MarkAsDone(taskName);
                    break;
                case "mark as not done":
                    MarkAsNotDone(taskName);
                    break;
                case "change nothing":
                    Console.WriteLine("\nno changes made");
                    return;
            }
            Console.WriteLine("\npress any key to return to the main menu");
            Console.ReadKey();
            DisplayTasks();
        }
    }
}
