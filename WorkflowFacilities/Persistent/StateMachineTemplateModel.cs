using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowFacilities.Persistent
{
    /*
     * 模板会被翻译成activity链持久化
     * 这样可以解决1.历史数据的还原：只需要不删除custom activity即可以找回数据
     * 2.方便编写 用户只需要指定模板version，如果同数据库现有模板不同则重新翻译并持久化
     * 直接在原模板类上更改，不需要重建一个新类。
     * 同时因为只持久化一次，所以model的id是固定的。
     */


    public class StateMachineTemplateModel
    {
        [Key]
        public Guid Version { get; set; }

        /// <summary>
        /// 模板类类型
        /// </summary>
        public string CodeTemplateName { get; set; }

        public virtual RunningActivityModel StartActivityModel { get; set; }

        public virtual List<RunningActivityModel> RunningActivityModels { get; set; }

        public StateMachineTemplateModel()
        {
            RunningActivityModels = new List<RunningActivityModel>();
        }
    }
}