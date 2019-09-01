using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AsanaNet;

namespace AsanaAPI
{
    public class AsanaWorkSpace
    {

        readonly string key = "0/92735b07ef1c924c4d89aa7a2141ba6d";
        Asana _asanaRef;
        AsanaWorkspace _workspace;
        AsanaUser _user;

        public AsanaWorkSpace(string workspaceName)
        {
            _asanaRef = new Asana(key, AuthenticationType.Basic, errorCallback);

            _asanaRef.GetMe(o =>
            {
                _user = o as AsanaUser;
            }).Wait();

            _asanaRef.GetWorkspaces(o =>
            {
                foreach (AsanaWorkspace workspace in o)
                {
                    Debug.WriteLine("Found Workspace: " + workspace.Name);
                    if (workspace.Name.Equals(workspaceName, StringComparison.OrdinalIgnoreCase))
                    {
                        _workspace = workspace;
                    }
                }
                Debug.WriteLine("Active Workspace: " + _workspace.Name);

            }).Wait();
        }

        private AsanaProject GetProjectByName(string projectName)
        {
            AsanaProject activeProject = null;
            
            _asanaRef.GetProjectsInWorkspace(_workspace, o =>
            {

                foreach (AsanaProject project in o)
                {
                    Debug.WriteLine("Found Project: " + project.Name);
                    if (project.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
                    {
                        activeProject = project;
                        Debug.WriteLine("Active project: " + activeProject.Name);
                    }
                }
            }).Wait();
            return activeProject;
            
        }

        private AsanaTask GetTaskByName(string taskName)
        { 
            AsanaTask activeTask = null;
            _asanaRef.GetTasksInWorkspace(_workspace, o =>
            {

                foreach (AsanaTask task in o)
                {
                    if (task.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase))
                    {
                        activeTask = task;
                    }
                }
            }).Wait();
            return activeTask;

        }

        public void AddTask(string name, string notes, List<string> projectNames, DateTime dueDate)
        {

            AsanaTask task = new AsanaTask(_workspace)
            {
                Name = name,
                Notes = notes,
                DueOn = dueDate,
                //I modified Projects to have a public setter, but that didn't work since I dont think the API formats the parameter right to set the projects at task creation, but I am not sure
         //       Projects = new List<AsanaProject>() { GetProjectByName(projectNames[0]) }.ToArray()
            };
            task.Save(_asanaRef).Wait();
           // var serverTask = GetTaskByName(name);
            foreach (string proj in projectNames)
            {
                var project = GetProjectByName(proj);
                //this fails assumedly because the task ID is still 0.  The API should be updating it when the task is saved but isn't
                task.AddProject(project, _asanaRef).Wait();
            }

        }

        static void errorCallback(string s1, string s2, string s3)
        {
            throw new Exception(s1 + s2 + s3);
        }
    }

   /* public class AsanaProj
    {

        Asana _asanaRef;
        private readonly AsanaProject _proj;
        private readonly AsanaWorkspace _workspace;
        public AsanaProj(AsanaProject project, AsanaWorkspace workspace, Asana asanaRef)
        {
            _proj = project;
            _asanaRef = asanaRef;
            _workspace = workspace;
        }



    }*/


}
