using System.Runtime.ExceptionServices;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace WorkflowFacilities
{
    public class StateMachineScheduler
    {
        /*
         * 翻译成可执行的activity
         */
        public void Run(StateMachine stateMachine)
        { }

        private void Translate(State state, IExecuteActivity activity)
        {
            var stateEmptyExecuteActivity = new StateEmptyExecuteActivity {ParentActivity = state};
            activity.NextActivities.Add(stateEmptyExecuteActivity);
            activity = stateEmptyExecuteActivity;
            var entry = state.Entry;
            if (entry != null) {
                var baseExecuteActivity = new CustomExecuteActivity(entry.Execute, entry.BookmarkCallback) {
                    Version = entry.Version,
                    Bookmark = entry.Bookmark,
                    Name = 
                };
            }
        }

        private void InternalRun(IExecuteActivity activity, PipelineContext context)
        {
            var execute = activity.Execute(context);
            activity.Executed = true;
            if (!execute) {
                return;
            }

            if (activity.IsHangUped) {
                return;
            }

            if (activity.NextActivities.Count == 0) {
                return;
            }

            foreach (var nextActivity in activity.NextActivities) {
                InternalRun(nextActivity, context);
            }
        }
    }
}