using System;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /*
     * 允许非继承新建使用delegation
     */
     
    /// <summary>
    /// empty transitionpath doesn't need to add into workflow transition collection
    /// which has empty conditionfunc
    /// </summary>
    public class TransitionPath:IActivity
    {
        public State To { get; set; }

        public ICustomActivity Aciton { get; set; }

        public Func<PipelineContext,bool> ConditionFunc;

        /*public static TransitionPath Create(ICustomActivity actionActivity,State to,
            Func<PipelineContext,bool> func)
        {
            return new TransitionPath() {
                To = to,
                Aciton = actionActivity,
                ConditionFunc = func

            };
        }*/

        /// <summary>
        /// template生成时使用固定guid
        /// </summary>
        public Guid Version { get; set; }

        public string DisplayName { get; set; }
    }

    
}