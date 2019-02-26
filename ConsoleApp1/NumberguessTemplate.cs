using System;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace ConsoleApp1
{
    public class NumberguessTemplate : StateMachineTemplate
    {
        public NumberguessTemplate() : base()
        {
            this.Version = Guid.Parse("D5AE474A-5919-4A9C-A90E-F14BD8D92E3A");
            this.Name = "NumberGuess";
        }

        public override void Generation()
        {
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
            var readIntActivity = new CodeActivity(context => {
                    context.WaitOn("");
                    return true;
                }, (context, s, arg3) => context.Set("Guess",arg3.ToString()))
                { Bookmark = "EnterGuess", Version = Guid.Parse("B06E8F2F-37FD-4B18-8744-55285FB4EA1B")};

            var activity1 = new CodeActivity((context => {
                var s = int.Parse(context.Get("Guess"));
                var i = int.Parse(context.Get("Target"));
                Console.WriteLine(s < i ? "Your guess is too low." : "Your guess is too high.");
                return true;
            }), null) {Version = Guid.Parse("E85BB78B-5C36-488E-8C93-D857B4ED9625")};
            CustomActivities.AddRange(new[] {codeActivity, activity, codeActivity1, readIntActivity, activity1});

            var initializeState = new State() {
                DisplayName = "Initialize Target",
                Entry = codeActivity,
            };
            var enterState = new State() {
                DisplayName = "Enter Guess",
                Entry = activity,
                Exit = codeActivity1
            };
            var finalState = new State() {
                DisplayName = "FinalState"
            };
            States.AddRange(new[] {initializeState, enterState, finalState});

            //空transition和path理论上不需要加进去
            var transition = new Transition();
            transition.TransitionPaths.Add(new TransitionPath {To = enterState});
            initializeState.Transitions.Add(transition);
            var transition1 = new Transition() {
                Trigger = readIntActivity,
                Version = Guid.Parse("2EF26ADC-3D0E-4DEA-9DFD-6E46D5979A87")
            };
            Transitions.AddRange(new[] {transition1});

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
                    return guess != target;
                },
                Aciton = activity1,
                Version = Guid.Parse("FC7A3F25-0577-42CD-B706-B79A96108415")
            };
            transition1.TransitionPaths.Add(path);
            enterState.Transitions.Add(transition1);

            TransitionPaths.AddRange(new[] {transitionPath, path});

            StartState = initializeState;
        }

    }
}