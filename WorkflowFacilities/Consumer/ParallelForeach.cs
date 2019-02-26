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

        public Func<IEnumerable<string>> GetSource {
            get { return _getSource; }
            set { _getSource = value; }
        }

        private Func<IEnumerable<string>> _getSource;

        private string _itemName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getSource">loop source</param>
        /// <param name="itemName">loop itemname</param>
        public ParallelForeach(Func<IEnumerable<string>> getSource, string itemName)
        {
            _getSource = getSource ?? throw new ArgumentNullException("getSource mustn't be null");
            _itemName = itemName;
        }

        internal override IExecuteActivity InternalTranslate(IExecuteActivity executeActivity)
        {
            //形成三角循环--
            var loopStart = new LoopStart();
            var loopEnd = new LoopEnd();
            executeActivity.NextActivities.Add(loopStart);
            var internalTranslate = Body.InternalTranslate(loopStart);
            internalTranslate.NextActivities.Add(loopEnd);
            loopEnd.NextActivities.Add(loopStart);
            return loopEnd;
        }
    }


    public class ParallelForeachEntry : BaseExecuteActivity
    {
        private readonly Func<IEnumerable<string>> _getSourceFunc;

        private string _itemName;

        private List<string> _sourceList;

        private int _currentLoop;

        public ParallelForeachEntry(Func<IEnumerable<string>> getSourceFunc, string itemName)
        {
            this._getSourceFunc = getSourceFunc;
            this._itemName = itemName;
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
            else {
                
                _currentLoop++;
            }

            return true;
        }
    }

    public class ParallelForeachLoopEnd : BaseExecuteActivity
    {
        public ParallelForeachLoopEnd()
        { }
    }


    public class ParallelForeachExit : BaseExecuteActivity
    {
        public override bool Execute(PipelineContext context)
        {
            return base.Execute(context);
        }
    }
}