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

        private const string LOAD_CMD = "load";
        private const string SAVE_CMD = "save";
        private const string EXIT_CMD = "exit";
        private const string OPEN_PROJECT_CMD = "open-project:";
        private const string OPEN_TASK_CMD = "open-task:";
        private const string NEW_PROJECT_CMD = "new-project:";
        private const string NEW_TASK_CMD = "new-task:";
        private const string LOAD_DEFAULT_CONTEXTS_CMD = "load-default-contexts";
        private const string LIST_ALL_CMD = "list-all";
        private const string LIST_PROJECTS_CMD = "list-projects";
        private const string LIST_CONTEXTS_CMD = "list-contexts";
        private const string CHANGE_PRJ_DUE_DATE_CMD = "change-project-due-date:";
        private const string CHANGE_PRJ_NAME_CMD = "change-project-name:";
        private const string CHANGE_TASK_STATUS_CMD = "change-task-status:";
        private const string CHANGE_TASK_DETAILS_CMD = "change-task-details:";
        private const string CHANGE_TASK_CONTEXT_CMD = "change-task-context:";
        private const string REMOVE_TASK_CONTEXT_CMD = "remove-task-context";
        private const string NEW_CONTEXT_CMD = "new-context:";
        private const string REMOVE_CONTEXT_CMD = "remove-context:";
        private const string CHANGE_CONTEXT_CMD = "change-context:";
        private const string SHOW_OPEN_TASK_CMD = "show-open-task";
        private const string SHOW_OPEN_PROJECT_CMD = "show-open-project";
        private const string LIST_NEXT_TASKS_CMD = "list-next-tasks";
        private const string LIST_NO_TASKS_CMD = "list-no-tasks";
        private const string LIST_TASKS_CMD = "list-tasks";
        private const string CLEAR_OPEN_PROJECT_CMD = "clear-open-project";
        private const string CLEAR_OPEN_TASK_CMD = "clear-open-task";
        private const string DELETE_OPEN_PROJECT_CMD = "delete-open-project";
        private const string MOVE_TASK_FIRST_CMD = "move-task-first";
        private const string MOVE_TASK_LAST_CMD = "move-task-last";
        private const string MOVE_TASK_UP_CMD = "move-task-up";
        private const string MOVE_TASK_DOWN_CMD = "move-task-down";

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
                    else if (inputCmd.StartsWith(CHANGE_PRJ_NAME_CMD))
                    {
                        if (_openProject == null)
                        {
                            System.Console.Out.WriteLine("No open project");
                            continue;
                        }
    
                        String newName = inputCmd.Substring(CHANGE_PRJ_NAME_CMD.Length).Trim();
                        if (String.IsNullOrEmpty(newName))
                        {
                            System.Console.Out.WriteLine("New Name is blank!");
                        }
                        else
                        {
                            _openProject.ProjectName = newName;
                        }
                    }
                    else if (inputCmd.StartsWith(CHANGE_TASK_DETAILS_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            System.Console.Out.WriteLine(errmsg);
                            continue;
                        }

                        String newDetails = inputCmd.Substring(CHANGE_TASK_DETAILS_CMD.Length).Trim();
                        if (String.IsNullOrEmpty(newDetails))
                        {
                            System.Console.Out.WriteLine("New Details are blank!");
                        }
                        else
                        {
                            _openTask.Details = newDetails;
                        }
                    }
                    else if (inputCmd.StartsWith(CHANGE_TASK_CONTEXT_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            System.Console.Out.WriteLine(errmsg);
                            continue;
                        }

                        String newContextID = inputCmd.Substring(CHANGE_TASK_CONTEXT_CMD.Length).Trim();
                        if (String.IsNullOrEmpty(newContextID))
                        {
                            System.Console.Out.WriteLine("New ContextID is blank!");
                        }

                        if (!IsValidContext(newContextID))
                        {
                            System.Console.Out.WriteLine("Cannot find Context with ID: '" + newContextID + "'");
                        }
                        else
                        {
                            _openTask.ContextID = newContextID;
                        }
                    }
                    else if (inputCmd.StartsWith(REMOVE_TASK_CONTEXT_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            System.Console.Out.WriteLine(errmsg);
                            continue;
                        }

                        _openTask.ContextID = "";

                    }
                    else if (inputCmd.StartsWith(CHANGE_TASK_STATUS_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            System.Console.Out.WriteLine(errmsg);
                            continue;
                        }

                        String statusStr = inputCmd.Substring(CHANGE_TASK_STATUS_CMD.Length);
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
                    else if (inputCmd.StartsWith(MOVE_TASK_FIRST_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            System.Console.Out.WriteLine(errmsg);
                            continue;
                        }

                        _openProject.MoveTaskFirst(_openTask.UniqueID.ToString());
                    }
                    else if (inputCmd.StartsWith(MOVE_TASK_LAST_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            System.Console.Out.WriteLine(errmsg);
                            continue;
                        }

                        _openProject.MoveTaskLast(_openTask.UniqueID.ToString());
                    }
                    else if (inputCmd.StartsWith(MOVE_TASK_DOWN_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            System.Console.Out.WriteLine(errmsg);
                            continue;
                        }

                        _openProject.MoveTaskDown(_openTask.UniqueID.ToString());
                    }
                    else if (inputCmd.StartsWith(MOVE_TASK_UP_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            System.Console.Out.WriteLine(errmsg);
                            continue;
                        }

                        _openProject.MoveTaskUp(_openTask.UniqueID.ToString());
                    }
                    else if (inputCmd.StartsWith(CHANGE_PRJ_DUE_DATE_CMD))
                    {
                        if (_openProject != null)
                        {
                            String newDueDateStr = inputCmd.Substring(CHANGE_PRJ_DUE_DATE_CMD.Length);
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
                    else if (inputCmd.StartsWith(NEW_CONTEXT_CMD))
                    {
                        String newContextDescr = inputCmd.Substring(NEW_CONTEXT_CMD.Length).Trim();
                        try
                        {
                            if (String.IsNullOrEmpty(newContextDescr))
                            {

                                System.Console.Out.WriteLine("Context is blank");
                            }
                            else if (_session.Contexts.FindIdByDescr(newContextDescr) != null)
                            {
                                System.Console.Out.WriteLine("Context already exists: '" + newContextDescr + "'");
                            }
                            else
                            {
                                Context c = new Context(Guid.NewGuid().ToString(), newContextDescr);
                                _session.Contexts.Add(c);
                            }
                        }
                        catch
                        {
                            System.Console.Out.WriteLine("Error adding Context: '" + newContextDescr + "'");
                        }
                    }
                    else if (inputCmd.StartsWith(REMOVE_CONTEXT_CMD))
                    {
                        String contextID = inputCmd.Substring(REMOVE_CONTEXT_CMD.Length).Trim();
                        try
                        {
                            var context = _session.Contexts.FindIdByID(contextID);
                            if (String.IsNullOrEmpty(contextID))
                            {

                                System.Console.Out.WriteLine("Context ID is blank");
                            }
                            else if (context == null)
                            {
                                System.Console.Out.WriteLine("Cannot find Context with ID: '" + contextID + "'");
                            }
                            else
                            {
                                String d = context.Description;
                                _session.Contexts.Remove(contextID);
                                System.Console.Out.WriteLine("Removed Context with ID: '" + contextID + "' Description:'" + d + "'");
                            }
                        }
                        catch
                        {
                            System.Console.Out.WriteLine("Error removing Context: '" + contextID + "'");
                        }
                    }
                    else if (inputCmd.StartsWith(CHANGE_CONTEXT_CMD))
                    {
                        String cmd = inputCmd.Substring(CHANGE_CONTEXT_CMD.Length).Trim();

                        try
                        {
                            var split = cmd.Split(',');

                            if (split.Length != 2)
                            {
                                System.Console.Out.WriteLine("Invalid command: '" + cmd + "'");
                                continue;
                            }

                            var id = split[0];
                            var newDescription = split[1];
                            var context = _session.Contexts.FindIdByID(id);

                            if (String.IsNullOrEmpty(id))
                            {
                                System.Console.Out.WriteLine("Context ID is blank");
                                continue;
                            }
                            else if (String.IsNullOrEmpty(newDescription))
                            {
                                System.Console.Out.WriteLine("New description is blank");
                                continue;
                            }
                            else if (context == null)
                            {
                                System.Console.Out.WriteLine("Cannot find Context with ID: '" + id + "'");
                                continue;
                            }

                            context.Description = newDescription;
                        }
                        catch
                        {
                            System.Console.Out.WriteLine("Error changing Context: '" + cmd + "'");
                        }
                    }
                    else if (inputCmd == SHOW_OPEN_PROJECT_CMD)
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
                    else if (inputCmd == SHOW_OPEN_TASK_CMD)
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
                    else if (inputCmd == LIST_TASKS_CMD)
                    {
                        if (_openProject != null)
                        {
                            _session.GetTaskList(_openProject.UniqueID).ForEach(t => System.Console.Out.WriteLine(t.ToString()));
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
                    else if (inputCmd == LIST_CONTEXTS_CMD)
                    {
                        ListContexts();
                    }
                    else if (inputCmd == LIST_PROJECTS_CMD)
                    {
                        ListProjects();
                    }
                    else if (inputCmd == LIST_ALL_CMD)
                    {
                        ListProjects();
                        ListContexts();
                    }
                    else if (inputCmd == LOAD_CMD)
                    {
                        Load();
                    }
                    else if (inputCmd == SAVE_CMD)
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
                    else if (inputCmd == DELETE_OPEN_PROJECT_CMD)
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
                    else if (inputCmd == CLEAR_OPEN_PROJECT_CMD)
                    {
                        _openProject = null;
                        _openTask = null;
                    }
                    else if (inputCmd == CLEAR_OPEN_TASK_CMD)
                    {
                        _openTask = null;
                    }
                    else if (inputCmd == LIST_NEXT_TASKS_CMD)
                    {
                        foreach (var prj in _session.ProjectEnumerable())
                        {
                           ListNextTask(prj);
                        }
                    }
                    else if (inputCmd == LIST_NO_TASKS_CMD)
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
                    else if (inputCmd == LOAD_DEFAULT_CONTEXTS_CMD)
                    {
                        var result = _session.LoadDefaultContexts();
                        if (!result)
                        {
                            System.Console.Out.WriteLine("Could not load Default Contexts - Contexts already exist");
                        }
                    }
                    else if (inputCmd == "?" || inputCmd == "help")
                    {
                        ListCommands();
                    }
                    else if (inputCmd == EXIT_CMD)
                    {
                        return 0;
                    }
                    else
                    {
                        System.Console.Out.WriteLine("Unknown Command! (Enter '?' or 'help' to see valid commands)");
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
            System.Console.Out.WriteLine(LOAD_CMD);
            System.Console.Out.WriteLine(SAVE_CMD);
            System.Console.Out.WriteLine(EXIT_CMD);
            System.Console.Out.WriteLine(LOAD_DEFAULT_CONTEXTS_CMD);
            System.Console.Out.WriteLine(LIST_ALL_CMD);
            System.Console.Out.WriteLine(LIST_PROJECTS_CMD);
            System.Console.Out.WriteLine(LIST_CONTEXTS_CMD);
            System.Console.Out.WriteLine(OPEN_PROJECT_CMD + "<ID>");
            System.Console.Out.WriteLine(OPEN_TASK_CMD + "<ID>");
            System.Console.Out.WriteLine(SHOW_OPEN_TASK_CMD);
            System.Console.Out.WriteLine(SHOW_OPEN_PROJECT_CMD);
            System.Console.Out.WriteLine(LIST_NEXT_TASKS_CMD);
            System.Console.Out.WriteLine(LIST_NO_TASKS_CMD);
            System.Console.Out.WriteLine(LIST_TASKS_CMD);
            System.Console.Out.WriteLine(CLEAR_OPEN_PROJECT_CMD);
            System.Console.Out.WriteLine(CLEAR_OPEN_TASK_CMD);
            System.Console.Out.WriteLine(NEW_PROJECT_CMD + "<name>[,<due date>]");
            System.Console.Out.WriteLine(NEW_TASK_CMD + "<name>[,<due date>[,detail]]");
            System.Console.Out.WriteLine(DELETE_OPEN_PROJECT_CMD);
            System.Console.Out.WriteLine(CHANGE_PRJ_DUE_DATE_CMD + "<due date>");
            System.Console.Out.WriteLine(CHANGE_PRJ_NAME_CMD + "<name>");
            System.Console.Out.WriteLine(CHANGE_TASK_STATUS_CMD + "<status>");
            System.Console.Out.WriteLine(CHANGE_TASK_DETAILS_CMD + "<details>");
            System.Console.Out.WriteLine(CHANGE_TASK_CONTEXT_CMD + " <ContextID>");
            System.Console.Out.WriteLine(REMOVE_TASK_CONTEXT_CMD);
            System.Console.Out.WriteLine(NEW_CONTEXT_CMD + "<description>");
            System.Console.Out.WriteLine(REMOVE_CONTEXT_CMD + "<ContextID>");
            System.Console.Out.WriteLine(CHANGE_CONTEXT_CMD + "<ContextID>,<new description>");
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
            prj.GetTaskList().OrderBy(t=>t.Order).ToList().ForEach(t => System.Console.Out.WriteLine("   " + t.ToString()));
        }

        private bool IsValidContext(string contextID)
        {
            return _session.Contexts.ContainsID(contextID);
        }

        private bool AreProjectAndTaskOpen(out string errmsg)
        {
            errmsg = "";
            if (_openProject == null)
            {
                errmsg = "No open project";
                return false;
            }
            else if (_openTask == null)
            {
                errmsg = "No open task";
                return false;
            }
            return true;
        }
    }
}
