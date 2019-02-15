using System;
using System.Collections.Generic;

namespace WorkflowFacilities
{
    /*理论上state之间是多对多的关系，transition是依附于state，为了方便持久化，transition独立出现
     为了节约时间，不实现condition的检查功能*/

    public class Transition:IActivity
    {
        public List<TransitionPath> TransitionPaths { get; set; }

        public Guid Version { get; set; }

        public ICustomActivity Trigger { get; set; }

        
    }
}