using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Statements;

namespace NumberGuessWorkflowActivities
{
    public sealed class ReadInt : NativeActivity<int>
    {   
        
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            
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