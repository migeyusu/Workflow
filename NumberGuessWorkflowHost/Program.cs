using System;
using System.Linq;
using System.Activities;
using NumberGuessWorkflowHost;
using System.Activities.Statements;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel.Activities;
using System.Threading;
using System.Windows.Forms;
using NumberGuessWorkflowActivities;

namespace NumberGuessWorkflowHost
{
    class Program
    {
        static void Main(string[] args)
        {
//            Application.EnableVisualStyles();
//            Application.Run(new WorkflowHostForm());
            var inputs = new Dictionary<string, object>() {{"MaxNumber", 100}};
            var autoResetEvent = new AutoResetEvent(false);
            var idleEvent = new AutoResetEvent(false);
            var workflowApplication = new WorkflowApplication(new StateMachineNumberGuessWorkflow(), inputs) {
                Completed = (e) => {
                    var i = Convert.ToInt32(e.Outputs["Turns"]);
                    Console.WriteLine("guess succeed :" + i);
                    autoResetEvent.Set();
                },
                Aborted = (e) => {
                    Console.WriteLine(e.Reason);
                    autoResetEvent.Set();
                },
                Idle = (e) => {
                    idleEvent.Set();
                },
                OnUnhandledException = (e) => {
                    Console.WriteLine(e.UnhandledException.ToString());
                    return UnhandledExceptionAction.Terminate;
                }
            };
            workflowApplication.Run();
            var handles = new WaitHandle[] {autoResetEvent, idleEvent};
            while (WaitHandle.WaitAny(handles) != 0) {
                // Gather the user input and resume the bookmark.`
                var validEntry = false;
                while (!validEntry) {
                    if (!Int32.TryParse(Console.ReadLine(), out var guess)) {
                        Console.WriteLine("Please enter an integer.");
                    }
                    else {
                        validEntry = true;
                        workflowApplication.ResumeBookmark("EnterGuess", guess);
                    }
                }
            }

            Console.ReadKey();
        }
    }
}