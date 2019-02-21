using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /*
     * 用户定义activity，随后翻译成activity chain，然后持久化
     * workflow实例只存储上下文信息
     * 新workflow从模板直接得到activirychain并关联自定义activity 实例执行
     * 历史workflow从数据库load activitychain并关联自定义activity 实例执行
     * activity可以不停地产生新实例
     * 为了防止出现多线程竞争，每个statemachine都应该使用新的实例
     * 为了创建实例可以有两种方式：反射load，用户自己实现load方法，我选择了第二种
     * 所以创建之后翻译就可以直接执行
     */


    public abstract class StateMachineTemplate
    {
        public Guid Version { get; set; }

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
        /// 允许使用新的customactivities实例作为运行链
        /// </summary>
        public void NewInstance()
        {
            
        }
        
        /// <summary>
        /// generation，创建一个template的新实例
        /// </summary>
        public abstract void OnGeneration();
        
        /// <summary>
        /// new instance initialize
        /// </summary>
        public abstract void OnInitialize(PipelineContext pipelineContext);

        public StateMachine GetStateMachine()
        {
            var stateMachine = new StateMachine {
                Template = this,
                Id = Guid.NewGuid(),
                Context = new PipelineContext(),
                Name = this.Name,
            };
            return stateMachine;
        }
    }
}