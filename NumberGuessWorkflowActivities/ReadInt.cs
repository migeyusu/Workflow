using System;
using System.Activities;

namespace NumberGuessWorkflowActivities
{
    public sealed class ReadInt : NativeActivity<int>
    {
        public InArgument<int> Test { get; set; }
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var i = Test.Get(context);
//            var nameVariable = new Variable<string>();
//            var inArgument = new InArgument<string>((e)=>nameVariable.Get(e));
             var name = BookmarkName.Get(context);
            if (name == string.Empty) {
                throw new ArgumentException("ddd");
            }
            context.CreateBookmark(name, Target);

            
        }

        private void Target(NativeActivityContext context, Bookmark bookmark, object value)
        {
            this.Result.Set(context, Convert.ToInt32(value)); 
        }

        protected override bool CanInduceIdle {
            get { return true; }
        }
    }
}