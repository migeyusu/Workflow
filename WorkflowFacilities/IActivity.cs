using System;

namespace WorkflowFacilities
{
    /*理论上工作流的持久化可以分为两类：
     1.支持activity内execute持久化，优点是不需要保留以前的代码，缺点是需要创建执行环境 
     2.只保留activity的version，每次从代码里load，优点实现简单，缺点需要注意历史代码*/

    public interface IActivity
    {
        /// <summary>
        /// 唯一标记，版本号
        /// </summary>
        Guid Version { get; set; }
    }
}