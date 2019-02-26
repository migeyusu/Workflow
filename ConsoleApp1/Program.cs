using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkflowFacilities;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Persistent;
using WorkflowFacilities.Running;

namespace ConsoleApp1
{
    public class Program
    {
        static void Main(string[] args)
        {
            new WorkflowFact().Register<NumberguessTemplate>();
            
           StateMachine stateMachine;
            using (var openField = WorkflowFact.OpenField("WorkflowDb")) {
                openField.CheckUpdates();
                stateMachine = openField.NewStateMachine<NumberguessTemplate>();
                openField.SaveChanges();
            }
           // var stateMachineScheduler = new StateMachineScheduler(stateMachine);
           // var completed = false;
           // stateMachineScheduler.OnCompleted += () => { completed = true; };
           // stateMachineScheduler.Run();
           // if (!completed) {
           //     var validEntry = false;
           //     while (!validEntry) {
           //         if (!Int32.TryParse(Console.ReadLine(), out var guess)) {
           //             Console.WriteLine("Please enter an integer.");
           //         }
           //         else {
           //             validEntry = true;
           //             stateMachineScheduler.ResumeBookmark("EnterGuess", guess.ToString());
           //         }
           //     }
           // }
           // using (var openField = WorkflowFact.OpenField("WorkflowDb")) {
           //     openField.Save(stateMachine);
           //     Console.WriteLine(stateMachine.Id);
           //     openField.SaveChanges();
           // }
            /*using (var workflowDbContext = new WorkflowDbContext()) {
                var stateMachineModel = workflowDbContext.StateMachineModels.First();
                var binaryFormatter = new BinaryFormatter();
                
                using (var memoryStream = new MemoryStream(s)) {
                    var concurrentDictionary = binaryFormatter.Deserialize(memoryStream) as ConcurrentDictionary<string,string>;
                    Console.WriteLine(concurrentDictionary.Keys.Count);
                }
            }*/

            /*using (var openField = WorkflowFact.OpenField("WorkflowDb"))
            {
                var findStateMachine = openField.Get(Guid.Parse("894FE322-332F-43C7-B2C2-904523B07796"));
                var machineScheduler = new StateMachineScheduler(findStateMachine);
                machineScheduler.ResumeBookmark("EnterGuess", 9.ToString());
                openField.Update(findStateMachine);
                openField.SaveChanges();
            }*/
  
            Console.ReadKey();
        }
    }

    public class New:BaseCodeActivity
    {
        
    }
}