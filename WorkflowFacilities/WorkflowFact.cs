using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Persistent;
using WorkflowFacilities.Running;

namespace WorkflowFacilities
{
    public interface IWorkflowTemplateRegister
    {
        IWorkflowTemplateRegister Register<T>() where T : StateMachineTemplate;
    }

    public class WorkflowFact : IWorkflowTemplateRegister
    {
//        private static Type baseRegisterType = typeof(StateMachineTemplate);

        private static readonly List<Type> templateTypes = new List<Type>();

        public static List<Type> AllTemplateTypes => templateTypes;

        public IWorkflowTemplateRegister Register<T>() where T : StateMachineTemplate
        {
            var type = typeof(T);
            if (!templateTypes.Contains(type)) {
                templateTypes.Add(type);
            }

            return this;
        }

        /// <summary>
        /// 访问的入口
        /// </summary>
        /// <param name="constring">ef配置字段：
        ///  <add  name="constring"  connectionString="xx"  providerName="System.Data.SqlClient" />
        /// </param>
        /// <returns></returns>
        public static Field OpenField(string constring)
        {
            var workflowDbContext = string.IsNullOrEmpty(constring)
                ? new WorkflowDbContext()
                : new WorkflowDbContext(constring);
            return new Field(workflowDbContext);
        }
    }
}