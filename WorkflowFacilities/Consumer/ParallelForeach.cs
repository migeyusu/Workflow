using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /*
     * foreach don't need persistance 
     */

    public class ParallelForeach : BaseCodeActivity
    {
        public BaseCodeActivity Body { get; set; }

        private readonly Func<IEnumerable<string>> _getSourceFunc;

        private readonly string _itemName;

        private List<string> _sourceList;

        //loop count in memory
        private int _currentLoop;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getSource">loop source</param>
        /// <param name="itemName">loop itemname</param>
        public ParallelForeach(Func<IEnumerable<string>> getSource, string itemName)
        {
            _getSourceFunc = getSource ?? throw new ArgumentNullException("getSource mustn't be null");
            _itemName = itemName;
        }

        public override bool Execute(PipelineContext context)
        {
            if (_sourceList == null) {
                _sourceList = _getSourceFunc.Invoke().ToList();
                //this version same as parent 
                context.Set(this.Version.ToString(), _sourceList.Count.ToString());
                _currentLoop = 0;
            }

            if (_currentLoop == _sourceList.Count) {
                //stop loop
                return false;
            }

            context.Set(_itemName, _sourceList[_currentLoop]);
            _currentLoop++;
            return true;
        }

        internal override IExecuteActivity InternalTranslate(IExecuteActivity executeActivity,
            IDictionary<Guid, IExecuteActivity> stateMapping)
        {
            //形成循环--
            var loopStart = new ParallelForeachEntry(this);
            executeActivity.NextActivities.Add(loopStart);
            var loopEnd = new ParallelForeachLoopEnd();
            loopStart.NextActivities.Add(loopEnd);
            var internalTranslate = Body.InternalTranslate(loopEnd,stateMapping);
            var parallelEndActivity = new ParallelEndActivity() {Version = this.Version};
            internalTranslate.NextActivities.Add(parallelEndActivity);
            loopEnd.NextActivities.Add(loopStart);
            return parallelEndActivity;
        }
    }
}