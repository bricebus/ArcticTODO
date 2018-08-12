using System;
using System.Collections.Generic;
using System.Linq;
using StormshrikeTODO.Model;
using StormshrikeTODO.Persistence;


namespace StormshrikeTODO.CmdLine
{
    public class CmdLineMain
    {
        private Session _session = null;

        private const string OPEN_PROJECT_CMD = "open-project:";
        private const string OPEN_TASK_CMD = "open-task:";
        private const string NEW_PROJECT_CMD = "new-project:";
        private const string NEW_TASK_CMD = "new-task:";
        private const string CHANGE_PRJ_DUE_DATE = "change-project-due-date:";
        private const string CHANGE_PRJ_NAME = "change-project-name:";
        private const string CHANGE_TASK_STATUS = "change-task-status:";
        private const string CHANGE_TASK_DETAILS = "change-task-details:";

        private Project _openProject = null;
        private Task _openTask = null;

        public CmdLineMain(Session session)
        {
            _session = session;
        }

        public int Start(string[] args)
        {
            while (true)
            {
                try
                {
                    System.Console.Out.Write("StormshrikeTODO> ");
                    String inputCmd = System.Console.In.ReadLine();
    
                    if (inputCmd.StartsWith(NEW_PROJECT_CMD))
                    {
                        if (!_session.Initialized)
                        {
                            System.Console.Out.WriteLine("Data not initialized!");
                            continue;
                        }
    
                        String newPrjInput = inputCmd.Substring(NEW_PROJECT_CMD.Length);
                        string[] cmdArrary = newPrjInput.Split(',');
                        String prjName = cmdArrary[0].Trim();
    
                        DateTime? prjDueDate = null;
                        if (cmdArrary.Length > 1)
                        {
                            prjDueDate = DateTime.Parse(cmdArrary[1].Trim());
                        }
    
                        Project prj = new Project(prjName, prjDueDate);
                        _session.AddProject(prj);
                        _openProject = prj;
                        _openTask = null;
                    }
                    else if (inputCmd.StartsWith(OPEN_PROJECT_CMD))
                    {
                        _openTask = null;
                        _openProject = null;
                        String prjID = inputCmd.Substring(OPEN_PROJECT_CMD.Length);
                        _openProject = _session.FindProjectByID(prjID);
                       if (_openProject == null)
                        {
                            System.Console.Out.WriteLine("Project not found!");
                        }
                    }
                    else if (inputCmd.StartsWith(CHANGE_PRJ_NAME))
                    {
                        if (_openProject == null)
                        {
                            System.Console.Out.WriteLine("No open project");
                            continue;
                        }
    
                        String newName = inputCmd.Substring(CHANGE_PRJ_NAME.Length).Trim();
                        if (String.IsNullOrEmpty(newName))
                        {
                            System.Console.Out.WriteLine("New Name is blank!");
                        }
                        else
                        {
                            _openProject.ProjectName = newName;
                        }
                    }
                    else if (inputCmd.StartsWith(CHANGE_TASK_DETAILS))
                    {
                        if (_openProject == null)
                        {
                            System.Console.Out.WriteLine("No open project");
                            continue;
                        }
                        else if (_openTask == null)
                        {
                            System.Console.Out.WriteLine("No open task");
                            continue;
                        }
    
                        String newDetails = inputCmd.Substring(CHANGE_TASK_DETAILS.Length).Trim();
                        if (String.IsNullOrEmpty(newDetails))
                        {
                            System.Console.Out.WriteLine("New Deatsils are blank!");
                        }
                        else
                        {
                            _openTask.Details = newDetails;
                        }
                    }
                    else if (inputCmd.StartsWith(CHANGE_TASK_STATUS))
                    {
                        if (_openProject == null)
                        {
                            System.Console.Out.WriteLine("No open project");
                            continue;
                        }
                        else if (_openTask == null)
                        {
                            System.Console.Out.WriteLine("No open task");
                            continue;
                        }
    
                        String statusStr = inputCmd.Substring(CHANGE_TASK_STATUS.Length);
                        try
                        {
                            _openTask.Status = (Task.StatusEnum) Enum.Parse(typeof(Task.StatusEnum), statusStr);
                            System.Console.WriteLine(_openTask.ToString());
                        }
                        catch
                        {
                            System.Console.Out.WriteLine("Invalid Status: '" + statusStr + "'");
                        }
                    }
                    else if (inputCmd.StartsWith(CHANGE_PRJ_DUE_DATE))
                    {
                        if (_openProject != null)
                        {
                            String newDueDateStr = inputCmd.Substring(CHANGE_PRJ_DUE_DATE.Length);
                            try
                            {
                                DateTime? newDueDate = DateTime.Parse(newDueDateStr.Trim());
                                _openProject.DueDate = newDueDate;
                            }
                            catch (Exception ex)
                            {
                                if (ex is System.FormatException)
                                {
                                    System.Console.Out.WriteLine("Invalid Date: " + newDueDateStr + " " +
                                        ex.Message);
                                }
                                else
                                {
                                    System.Console.Out.WriteLine("Error changing Due Date: " +
                                        newDueDateStr + " " + ex.Message);
                                }
                            }
                        }
                        else
                        {
                            System.Console.Out.WriteLine("No open project");
                        }
    
                    }
                    else if (inputCmd == "show-open-project")
                    {
                        if (_openProject != null)
                        {
                            System.Console.Out.WriteLine(_openProject.ToString());
                        }
                        else
                        {
                            System.Console.Out.WriteLine("No open project");
                        }
                    }
                    else if (inputCmd == "show-open-task")
                    {
                        if (_openProject == null)
                        {
                            System.Console.Out.WriteLine("No open project");
                        }
                        else if (_openTask == null)
                        {
                            System.Console.Out.WriteLine("No open task");
                        }
                        else
                        {
                            System.Console.Out.WriteLine(_openTask.ToString());
                        }
                    }
                    else if (inputCmd == "list-tasks")
                    {
                        if (_openProject != null)
                        {
                            _openProject.GetTaskList().ToList().ForEach(t => System.Console.Out.WriteLine(t.ToString()));
                        }
                        else
                        {
                            System.Console.Out.WriteLine("No open project");
                        }
                    }
                    else if (inputCmd.StartsWith(OPEN_TASK_CMD))
                    {
                        if (_openProject != null)
                        {
                            String taskInput = inputCmd.Substring(OPEN_TASK_CMD.Length);
                            string[] cmdArrary = taskInput.Split(',');
                            String taskIdStr = cmdArrary[0].Trim();
    
                            if ((_openTask = _openProject.GetTask(taskIdStr)) == null)
                            {
                                System.Console.Out.WriteLine("Could not find Task ID: '" + taskIdStr + "'");
                            }
                        }
                        else
                        {
                            System.Console.Out.WriteLine("No open project");
                        }
                    }
                    else if (inputCmd == "list-contexts")
                    {
                        ListContexts();
                    }
                    else if (inputCmd == "list-projects")
                    {
                        ListProjects();
                    }
                    else if (inputCmd == "list-all")
                    {
                        ListProjects();
                        ListContexts();
                    }
                    else if (inputCmd == "load")
                    {
                        Load();
                    }
                    else if (inputCmd == "save")
                    {
                        _session.Save();
                        System.Console.Out.WriteLine(_session.ProjectCount.ToString() + " projects saved");
                        System.Console.Out.WriteLine(_session.Contexts.Count.ToString() + " contexts saved");
                    }
                    else if (inputCmd.StartsWith(NEW_TASK_CMD))
                    {
                        if (_openProject == null)
                        {
                            System.Console.Out.WriteLine("No open project!");
                        }
                        else
                        {
                            try
                            {
                                _openTask = null;
    
                                String newTaskInput = inputCmd.Substring(NEW_TASK_CMD.Length);
                                string[] cmdArrary = newTaskInput.Split(',');
                                String taskName = cmdArrary[0].Trim();
                                DateTime? prjDueDate = null;
    
                                if (cmdArrary.Length >= 2)
                                {
                                    prjDueDate = DateTime.Parse(cmdArrary[1].Trim());
                                }
    
                                Task task = new Task(taskName, prjDueDate);
                                _openProject.AddTask(task);
    
                                if (cmdArrary.Length >= 3)
                                {
                                    task.Details = cmdArrary[2].Trim();
                                }
    
                                _openTask = task;
                            }
                            catch (Exception e)
                            {
                                System.Console.Error.WriteLine("Error creating new task! '" + inputCmd + "': " + e.Message);
                            }
    
                        }
                        
                    }
                    else if (inputCmd == "delete-open-project")
                    {
                        if (_openProject == null)
                        {
                            System.Console.Out.WriteLine("No open project!");
                        }
                        else
                        {
                            _session.RemoveProject(_openProject.UniqueID.ToString());
                            _openProject = null;
                            _openTask = null;
                        }
                    }
                    else if (inputCmd == "clear-open-project")
                    {
                        _openProject = null;
                        _openTask = null;
                    }
                    else if (inputCmd == "clear-open-task")
                    {
                        _openTask = null;
                    }
                    else if (inputCmd == "list-next-tasks")
                    {
                        foreach (var prj in _session.ProjectEnumerable())
                        {
                           ListNextTask(prj);
                        }
                    }
                    else if (inputCmd == "list-no-tasks")
                    {
                        var prjResults = _session.ListProjectsWithNoTasks();
                        if (prjResults != null)
                        {
                            foreach (var prj in prjResults)
                            {
                                PrintProject(prj);
                            }
                        }
                    }
                    else if (inputCmd == "load-default-contexts")
                    {
                        _session.LoadDefaultContexts();
                    }
                    else if (inputCmd == "?")
                    {
                        ListCommands();
                    }
                    else if (inputCmd == "exit")
                    {
                        return 0;
                    }
                    else
                    {
                        System.Console.Out.WriteLine("Unknown Command! (Enter '?' to see valid commands)");
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error:" + e.Message);
                    System.Console.WriteLine("     :" + e.InnerException.Message);

                }
            }
        }

        private static void ListCommands()
        {
            System.Console.Out.WriteLine("load");
            System.Console.Out.WriteLine("save");
            System.Console.Out.WriteLine("exit");
            System.Console.Out.WriteLine("load-default-contexts");
            System.Console.Out.WriteLine("list-all");
            System.Console.Out.WriteLine("list-projects");
            System.Console.Out.WriteLine("list-contexts");
            System.Console.Out.WriteLine(OPEN_PROJECT_CMD + "<ID>");
            System.Console.Out.WriteLine(OPEN_TASK_CMD + "<ID>");
            System.Console.Out.WriteLine("show-open-task");
            System.Console.Out.WriteLine("show-open-project");
            System.Console.Out.WriteLine("list-next-tasks");
            System.Console.Out.WriteLine("list-no-tasks");
            System.Console.Out.WriteLine("list-tasks");
            System.Console.Out.WriteLine("clear-open-project");
            System.Console.Out.WriteLine("clear-open-task");
            System.Console.Out.WriteLine(NEW_PROJECT_CMD + "<name>[,<due date>]");
            System.Console.Out.WriteLine(NEW_TASK_CMD + "<name>[,<due date>[,detail]]");
            System.Console.Out.WriteLine("delete-open-project");
            System.Console.Out.WriteLine(CHANGE_PRJ_DUE_DATE + "<due date>");
            System.Console.Out.WriteLine(CHANGE_PRJ_NAME + "<name>");
            System.Console.Out.WriteLine(CHANGE_TASK_STATUS + "<status>");
            System.Console.Out.WriteLine(CHANGE_TASK_DETAILS + "<details>");
        }

        private void LoadProjects()
        {
            _session.LoadProjects();
            System.Console.Out.WriteLine(_session.ProjectCount.ToString() + " projects loaded");
        }

        private void LoadContexts()
        {
            _session.LoadContexts();
            System.Console.Out.WriteLine(_session.Contexts.Count.ToString() + " contexts loaded");
        }

        private void Load()
        {
            LoadProjects();
            LoadContexts();
        }

        private void ListProjects()
        {
            int prjCount = _session.ProjectCount;
            System.Console.Out.WriteLine("Project List...(" + prjCount + ")");
            foreach (var prj in _session.ProjectEnumerable())
            {
                PrintProject(prj);
            }
        }

        private void ListContexts()
        {
            int ctxCount = _session.Contexts != null ? _session.Contexts.Count : 0;
            System.Console.Out.WriteLine("Contexts List...(" + ctxCount + ")");
            if (ctxCount > 0)
            {
                _session.Contexts.GetList().ForEach(ctx => PrintContext(ctx));
            }
        }

        private void PrintContext(Context ctx)
        {
            System.Console.Out.WriteLine(ctx.ToString() + " ID=" + ctx.ID);
        }

        private void ListNextTask(Project prj)
        {
            System.Console.Out.WriteLine(prj.ProjectName + ": " + prj.GetNextTask()?.Name);
        }

        private void PrintProject(Project prj)
        {
            System.Console.Out.WriteLine(prj.ToString());
            prj.GetTaskList().ToList().ForEach(t => System.Console.Out.WriteLine("   " + t.ToString()));
        }
    }
}
