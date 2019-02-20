using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var stateMachineTemplate = new NumberguessTemplate();
            /*var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            var concurrentDictionary = new ConcurrentDictionary<string, string>();
            concurrentDictionary.TryAdd("df", "dsf");
            binaryFormatter.Serialize(memoryStream, concurrentDictionary);
            binaryFormatter.Deserialize()
            memoryStream.Dispose();*/
            ICustomActivity activity = null;
            Class.set(out activity);
            Console.WriteLine(activity.Name);

        }
    }

    public class NumberguessTemplate : StateMachineTemplate
    {
        public override void OnGeneration()
        {
            this.Version = Guid.Parse("D5AE474A-5919-4A9C-A90E-F14BD8D92E3A");
            var codeActivity = new CodeActivity((context => {
                var maxNumber = int.Parse(context.Get("MaxNumber"));
                var target = new System.Random().Next(1, maxNumber + 1);
                context.Set("Target", target.ToString());
                return true;
            }), null) {Version = Guid.Parse("4FEA05BB-C8AB-41F3-93A4-89CC961F4264")};
            var activity = new CodeActivity((context => {
                var maxNumber = int.Parse(context.Get("MaxNumber"));
                Console.WriteLine("Please enter a number between 1 and " + maxNumber);
                return true;
            }), null) {Version = Guid.Parse("F3504036-814D-4101-B2BB-00371120E312")};
            var codeActivity1 = new CodeActivity((context => {
                var turns = int.Parse(context.Get("Turns"));
                turns++;
                context.Set("Turns", turns.ToString());
                return true;
            }), null) {Version = Guid.Parse("5AD03706-F349-4559-A8DD-FB881D0E9466")};
            var readIntActivity = new CodeActivity((context => {
                    context.WaitOn();
                    return true;
                }), (context => { context.Set("Guess", context.ResumingBookmark.Value.ToString()); }))
                {Bookmark = "EnterGuess", Version = Guid.Parse("B06E8F2F-37FD-4B18-8744-55285FB4EA1B")};

            var activity1 = new CodeActivity((context => {
                var s = int.Parse(context.Get("Guess"));
                var i = int.Parse(context.Get("Target"));
                Console.WriteLine(s < i ? "Your guess is too low." : "Your guess is too high.");
                return true;
            }),null){Version = Guid.Parse("E85BB78B-5C36-488E-8C93-D857B4ED9625")};
            CustomActivities.AddRange(new[] {codeActivity, activity, codeActivity1, readIntActivity, activity1});
            
            var initializeState = new State() {
                Name = "Initialize Target",
                Entry = codeActivity,
            };
            var enterState = new State() {
                Name = "Enter Guess",
                Entry = activity,
                Exit = codeActivity1
            };
            var finalState = new State() {
                Name = "FinalState"
            };
            States.AddRange(new []{initializeState,enterState,finalState});

            //空transition和path理论上不需要加进去
            var transition = new Transition();
            transition.TransitionPaths.Add(new TransitionPath {To = enterState});
            initializeState.Transitions.Add(transition);
            var transition1 = new Transition() {
                Trigger = readIntActivity,
                Version = Guid.Parse("2EF26ADC-3D0E-4DEA-9DFD-6E46D5979A87")
            };
            Transitions.AddRange(new []{transition1});
            
            var transitionPath = new TransitionPath() {
                To = finalState, 
                ConditionFunc = context => {
                    var guess = context.Get("Guess");
                    var target = context.Get("Target");
                    return guess == target;
                },
                Version = Guid.Parse("F8E6F16C-9FDF-47B4-845C-FFBD04DDD684")
            };
            transition1.TransitionPaths.Add(transitionPath);
            var path = new TransitionPath() {
                To = enterState,
                ConditionFunc = context => {
                    var guess = context.Get("Guess");
                    var target = context.Get("Target");
                    return guess == target;
                },
                Aciton = activity1,
                Version = Guid.Parse("FC7A3F25-0577-42CD-B706-B79A96108415")
            };
            transition1.TransitionPaths.Add(path);
            enterState.Transitions.Add(transition1);

            TransitionPaths.AddRange(new []{transitionPath,path});

            StartState = initializeState;
        }

        public override void OnInitialize(PipelineContext pipelineContext)
        {
            pipelineContext.Set("Guess", null);
            pipelineContext.Set("Target", null);
            pipelineContext.Set("MaxNumber", 10.ToString());
            pipelineContext.Set("Turns", 0.ToString());
        }
    }

    public class CodeActivity : ICustomActivity
    {
        public Guid Version { get; set; }

        public string Name { get; set; }

        public string Bookmark { get; set; }

        private readonly Func<PipelineContext, bool> _executeFunc;

        private readonly Action<PipelineContext> _callbackAction;

        public CodeActivity(Func<PipelineContext, bool> executeFunc, Action<PipelineContext> callbackAction)
        {
            this._callbackAction = callbackAction;
            _executeFunc = executeFunc;
        }

        public bool Execute(PipelineContext context)
        {
            return _executeFunc == null || _executeFunc(context);
        }

        public void BookmarkCallback(PipelineContext context)
        {
            _callbackAction?.Invoke(context);
        }
    }

    


    public class Class
    {
        public static void set(out ICustomActivity activity)
        {
            activity=new BaseExecuteActivity() {
                Name = "sdf"
            };
            
        }
    } 
}