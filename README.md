a simple to-do list console application that allows users to create a list of tasks. 
all tasks have a title and work-on date, deadline is optional. every task should belong to a project (however, that, too, is optional). 
the user will use a text-based UI via the command line. they are able to edit and remove tasks. 
the current task and project lists are automatically saved to a json file, and retrieved from it on restart.

the task list is displayed by date; if not completed, all overdue tasks are automatically moved to 'today,' and their deadline shows in red.
tasks can also be displayed sorted by the projects they belong to, and then the user gets a fuller description of each project.

the application includes a self-care feature, too: if the number of tasks per day excedes two, a self-care activity from a predefined list
is automatically inserted after each two tasks.
