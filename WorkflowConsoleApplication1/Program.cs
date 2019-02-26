using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Threading;
using NumberGuessWorkflowActivities;

namespace WorkflowConsoleApplication1
{

    class Program
    {
        static void Main(string[] args)
        {
              var inputs = new Dictionary<string, object>() {{"MaxNumber", 100}};
            var autoResetEvent = new AutoResetEvent(false);
            var idleEvent = new AutoResetEvent(false);
            var workflowApplication = new WorkflowApplication(new StateMachineNumberGuessWorkflow(), inputs) {
                Completed = (e) => {
                    var i = Convert.ToInt32(e.Outputs["Turns"]);
                    Console.WriteLine("guess succeed :" + i);
//                    autoResetEvent.Set();
                },
                Aborted = (e) => {
                    Console.WriteLine(e.Reason);
//                    autoResetEvent.Set();
                },
                Idle = (e) => {
//                    idleEvent.Set();
                },
                OnUnhandledException = (e) => {
                    Console.WriteLine(e.UnhandledException.ToString());
                    return UnhandledExceptionAction.Terminate;
                }
            };
            workflowApplication.Run();
            var readLine = int.Parse(Console.ReadLine());
            foreach (var bookmarkInfo in workflowApplication.GetBookmarks()) {
                Console.WriteLine(bookmarkInfo.BookmarkName);
            }
            workflowApplication.ResumeBookmark("Guess2", readLine);
            /*var handles = new WaitHandle[] {autoResetEvent, idleEvent};
            while (WaitHandle.WaitAny(handles) != 0) {
                // Gather the user input and resume the bookmark.`
                var validEntry = false;
                while (!validEntry) {
                    if (!Int32.TryParse(Console.ReadLine(), out var guess)) {
                        Console.WriteLine("Please enter an integer.");
                    }
                    else {
                        validEntry = true;
                        foreach (var bookmarkInfo in workflowApplication.GetBookmarks()) {
                            Console.WriteLine(bookmarkInfo.BookmarkName);
                        }
                        workflowApplication.ResumeBookmark("Guess2", guess);
                        break;
                    }
                }
            }*/
            var line = Console.ReadLine();
            Console.WriteLine(line);
            foreach (var bookmarkInfo in workflowApplication.GetBookmarks()) {
                Console.WriteLine(bookmarkInfo.BookmarkName);
            }
            Console.ReadKey();
        }
    }
}
