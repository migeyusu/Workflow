using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /*
     * 模板：用户定义activity，使用即时代码描述状态机，随后翻译成activity chain，持久化
     * statemachine实例只存储上下文信息
     * 新workflow从模板直接得到activirychain并关联自定义activity 实例执行
     * 历史workflow从数据库load activitychain并关联自定义activity 实例执行
     * 为了防止出现多线程竞争，每个statemachine都应该使用新的实例，即调用generate
     * 为了创建实例可以有两种方式：反射load，用户自己实现load方法，我选择了第二种
     * 所以创建之后翻译就可以直接执行
     * 由于只要template不更改，翻译后的结果是不变的，所以statemachine只存储每次运行的上下文
     * 而每次调用前都会检查是否出现了新的template，出现了则翻译后更新到数据库
     * 这样，形成了两个版本机制:template类类型和version，都在数据库里以version存在
     */


    public abstract class StateMachineTemplate
    {
        /// <summary>
        /// 版本号，必须在构造函数结束前定义，可以在重构后更改
        /// </summary>
        public Guid Version { get; set; }

        /// <summary>
        /// 模板名，必须在构造函数结束前定义，禁止在此后的重构中更改
        /// </summary>
        public string Name { get; set; }

        public List<State> States { get; set; }

        public List<Transition> Transitions { get; set; }

        public List<TransitionPath> TransitionPaths { get; set; }

        public List<ICustomActivity> CustomActivities { get; set; }

        public State StartState { get; set; }

        protected StateMachineTemplate()
        {
            this.States = new List<State>();
            this.Transitions = new List<Transition>();
            this.TransitionPaths = new List<TransitionPath>();
            this.CustomActivities = new List<ICustomActivity>();
        }
        
        /// <summary>
        /// 创建activity实例
        /// </summary>
        public abstract void Generation();
    }
}
