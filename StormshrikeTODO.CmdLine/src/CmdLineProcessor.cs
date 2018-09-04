using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using StormshrikeTODO.Model;
using StormshrikeTODO.Data;


namespace StormshrikeTODO.CmdLine
{
    public class CmdLineProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public CmdLineProcessor(Session session)
        {
            _session = session;
        }

        public int Start(string[] args)
        {
            log.Info("Starting main loop");
            while (true)
            {
                try
                {
                    log.Debug("Waiting for user input...");
                    Write("StormshrikeTODO> ");
                    String inputCmd = System.Console.In.ReadLine();
    
                    log.Debug("Input='" + inputCmd + "'");
                    if (inputCmd.StartsWith(NEW_PROJECT_CMD))
                    {
                        if (!_session.Initialized)
                        {
                            WriteError("Data not initialized!");
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
                            WriteError("Project not found!");
                        }
                    }
                    else if (inputCmd.StartsWith(CHANGE_PRJ_NAME_CMD))
                    {
                        if (_openProject == null)
                        {
                            WriteError("No open project");
                            continue;
                        }
    
                        String newName = inputCmd.Substring(CHANGE_PRJ_NAME_CMD.Length).Trim();
                        if (String.IsNullOrEmpty(newName))
                        {
                            WriteError("New Name is blank!");
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
                            WriteError(errmsg);
                            continue;
                        }

                        String newDetails = inputCmd.Substring(CHANGE_TASK_DETAILS_CMD.Length).Trim();
                        if (String.IsNullOrEmpty(newDetails))
                        {
                            WriteError("New Details are blank!");
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
                            WriteError(errmsg);
                            continue;
                        }

                        String newContextID = inputCmd.Substring(CHANGE_TASK_CONTEXT_CMD.Length).Trim();
                        if (String.IsNullOrEmpty(newContextID))
                        {
                            WriteError("New ContextID is blank!");
                        }

                        if (!IsValidContext(newContextID))
                        {
                            WriteError("Cannot find Context with ID: '" + newContextID + "'");
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
                            WriteError(errmsg);
                            continue;
                        }

                        _openTask.ContextID = "";

                    }
                    else if (inputCmd.StartsWith(CHANGE_TASK_STATUS_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            WriteError(errmsg);
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
                            WriteError("Invalid Status: '" + statusStr + "'");
                        }
                    }
                    else if (inputCmd.StartsWith(MOVE_TASK_FIRST_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            WriteError(errmsg);
                            continue;
                        }

                        _openProject.MoveTaskFirst(_openTask.UniqueID.ToString());
                    }
                    else if (inputCmd.StartsWith(MOVE_TASK_LAST_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            WriteError(errmsg);
                            continue;
                        }

                        _openProject.MoveTaskLast(_openTask.UniqueID.ToString());
                    }
                    else if (inputCmd.StartsWith(MOVE_TASK_DOWN_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            WriteError(errmsg);
                            continue;
                        }

                        _openProject.MoveTaskDown(_openTask.UniqueID.ToString());
                    }
                    else if (inputCmd.StartsWith(MOVE_TASK_UP_CMD))
                    {
                        if (!AreProjectAndTaskOpen(out string errmsg))
                        {
                            WriteError(errmsg);
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
                                    WriteError("Invalid Date: " + newDueDateStr, ex);
                                }
                                else
                                {
                                    WriteError("Error changing Due Date: " + newDueDateStr, ex);
                                }
                            }
                        }
                        else
                        {
                            WriteError("No open project");
                        }
    
                    }
                    else if (inputCmd.StartsWith(NEW_CONTEXT_CMD))
                    {
                        String newContextDescr = inputCmd.Substring(NEW_CONTEXT_CMD.Length).Trim();
                        try
                        {
                            if (String.IsNullOrEmpty(newContextDescr))
                            {

                                WriteError("Context is blank");
                            }
                            else if (_session.Contexts.FindIdByDescr(newContextDescr) != null)
                            {
                                WriteError("Context already exists: '" + newContextDescr + "'");
                            }
                            else
                            {
                                Context c = new Context(Guid.NewGuid().ToString(), newContextDescr);
                                _session.Contexts.Add(c);
                            }
                        }
                        catch
                        {
                            WriteError("Error adding Context: '" + newContextDescr + "'");
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

                                WriteError("Context ID is blank");
                            }
                            else if (context == null)
                            {
                                WriteError("Cannot find Context with ID: '" + contextID + "'");
                            }
                            else
                            {
                                String d = context.Description;
                                _session.RemoveContext(context);
                                WriteInfo("Removed Context with ID: '" + contextID + "' Description:'" + d + "'");
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteError("Error removing Context: '" + contextID, ex);
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
                                WriteError("Invalid command: '" + cmd + "'");
                                continue;
                            }

                            var id = split[0];
                            var newDescription = split[1];
                            var context = _session.Contexts.FindIdByID(id);

                            if (String.IsNullOrEmpty(id))
                            {
                                WriteError("Context ID is blank");
                                continue;
                            }
                            else if (String.IsNullOrEmpty(newDescription))
                            {
                                WriteError("New description is blank");
                                continue;
                            }
                            else if (context == null)
                            {
                                WriteError("Cannot find Context with ID: '" + id + "'");
                                continue;
                            }

                            context.Description = newDescription;
                        }
                        catch
                        {
                            WriteError("Error changing Context: '" + cmd + "'");
                        }
                    }
                    else if (inputCmd == SHOW_OPEN_PROJECT_CMD)
                    {
                        if (_openProject != null)
                        {
                            WriteLine(_openProject.ToString());
                        }
                        else
                        {
                            WriteError("No open project");
                        }
                    }
                    else if (inputCmd == SHOW_OPEN_TASK_CMD)
                    {
                        if (_openProject == null)
                        {
                            WriteError("No open project");
                        }
                        else if (_openTask == null)
                        {
                            WriteError("No open task");
                        }
                        else
                        {
                            WriteLine(_openTask.ToString());
                        }
                    }
                    else if (inputCmd == LIST_TASKS_CMD)
                    {
                        if (_openProject != null)
                        {
                            _session.GetTaskList(_openProject.UniqueID).ForEach(t => WriteLine(t.ToString()));
                        }
                        else
                        {
                            WriteError("No open project");
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
                                WriteError("Could not find Task ID: '" + taskIdStr + "'");
                            }
                        }
                        else
                        {
                            WriteError("No open project");
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
                        WriteInfo(_session.ProjectCount.ToString() + " projects saved");
                        WriteInfo(_session.Contexts.Count.ToString() + " contexts saved");
                    }
                    else if (inputCmd.StartsWith(NEW_TASK_CMD))
                    {
                        if (_openProject == null)
                        {
                            WriteError("No open project!");
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
                            catch (Exception ex)
                            {
                                WriteError("Error creating new task! '" + inputCmd, ex);
                            }
    
                        }
                        
                    }
                    else if (inputCmd == DELETE_OPEN_PROJECT_CMD)
                    {
                        if (_openProject == null)
                        {
                            WriteError("No open project!");
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
                            WriteError("Could not load Default Contexts - Contexts already exist");
                        }
                    }
                    else if (inputCmd == "?" || inputCmd == "help")
                    {
                        ListCommands();
                    }
                    else if (inputCmd == EXIT_CMD)
                    {
                        log.Info("Exiting StormshrikeTODO. Bye!");
                        break;
                    }
                    else
                    {
                        WriteError("Unknown Command! (Enter '?' or 'help' to see valid commands)");
                    }
                }
                catch (Exception e)
                {
                    WriteError("Error:" + e.Message, e);
                    if (e.InnerException != null)
                    {
                        WriteError("InnerException" + e.InnerException.Message, e.InnerException);
                    }

                }

            }
            log.Info("Stopping main loop");

            return 0;
        }

        private void WriteInfo(String infomsg)
        {
            log.Info(infomsg);
            WriteLine(infomsg);
        }

        private void WriteError(String errmsg)
        {
            log.Error(errmsg);
            WriteLine(errmsg);

        }

        private void WriteError(String errmsg, Exception ex)
        {
            log.Error(errmsg);
            WriteLine(errmsg);
            log.Debug(errmsg, ex);
        }

        private void WriteLine(string msg)
        {
            System.Console.Out.WriteLine(msg);
        }

        private void Write(string msg)
        {
            System.Console.Out.Write(msg);
        }

        private void ListCommands()
        {
            WriteLine(LOAD_CMD);
            WriteLine(SAVE_CMD);
            WriteLine(EXIT_CMD);
            WriteLine(LOAD_DEFAULT_CONTEXTS_CMD);
            WriteLine(LIST_ALL_CMD);
            WriteLine(LIST_PROJECTS_CMD);
            WriteLine(LIST_CONTEXTS_CMD);
            WriteLine(OPEN_PROJECT_CMD + "<ID>");
            WriteLine(OPEN_TASK_CMD + "<ID>");
            WriteLine(SHOW_OPEN_TASK_CMD);
            WriteLine(SHOW_OPEN_PROJECT_CMD);
            WriteLine(LIST_NEXT_TASKS_CMD);
            WriteLine(LIST_NO_TASKS_CMD);
            WriteLine(LIST_TASKS_CMD);
            WriteLine(CLEAR_OPEN_PROJECT_CMD);
            WriteLine(CLEAR_OPEN_TASK_CMD);
            WriteLine(NEW_PROJECT_CMD + "<name>[,<due date>]");
            WriteLine(NEW_TASK_CMD + "<name>[,<due date>[,detail]]");
            WriteLine(DELETE_OPEN_PROJECT_CMD);
            WriteLine(CHANGE_PRJ_DUE_DATE_CMD + "<due date>");
            WriteLine(CHANGE_PRJ_NAME_CMD + "<name>");
            WriteLine(CHANGE_TASK_STATUS_CMD + "<status>");
            WriteLine(CHANGE_TASK_DETAILS_CMD + "<details>");
            WriteLine(CHANGE_TASK_CONTEXT_CMD + " <ContextID>");
            WriteLine(REMOVE_TASK_CONTEXT_CMD);
            WriteLine(NEW_CONTEXT_CMD + "<description>");
            WriteLine(REMOVE_CONTEXT_CMD + "<ContextID>");
            WriteLine(CHANGE_CONTEXT_CMD + "<ContextID>,<new description>");
        }

        private void LoadProjects()
        {
            _session.LoadProjects();
            WriteInfo(_session.ProjectCount.ToString() + " projects loaded");
        }

        private void LoadContexts()
        {
            _session.LoadContexts();
            WriteInfo(_session.Contexts.Count.ToString() + " contexts loaded");
        }

        private void Load()
        {
            LoadProjects();
            LoadContexts();
        }

        private void ListProjects()
        {
            int prjCount = _session.ProjectCount;
            WriteInfo("Project List...(" + prjCount + ")");
            foreach (var prj in _session.ProjectEnumerable())
            {
                PrintProject(prj);
            }
        }

        private void ListContexts()
        {
            int ctxCount = _session.Contexts != null ? _session.Contexts.Count : 0;
            WriteInfo("Contexts List...(" + ctxCount + ")");
            if (ctxCount > 0)
            {
                _session.Contexts.GetList().ForEach(ctx => PrintContext(ctx));
            }
        }

        private void PrintContext(Context ctx)
        {
            WriteLine(ctx.ToString() + " ID=" + ctx.ID);
        }

        private void ListNextTask(Project prj)
        {
            WriteLine(prj.ProjectName + ": " + prj.GetNextTask()?.Name);
        }

        private void PrintProject(Project prj)
        {
            WriteLine(prj.ToString());
            prj.GetTaskList().OrderBy(t=>t.Order).ToList().ForEach(t => WriteLine("   " + t.ToString()));
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
