using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Linq;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Persistent;
using WorkflowFacilities.Running;

namespace WorkflowFacilities
{
    public class WorkflowFact : IWorkflowTemplateRegister
    {
        internal static Dictionary<string, Type> AllTemplateTypes { get; } = new Dictionary<string, Type>();


        public IWorkflowTemplateRegister Register<T>() where T : StateMachineTemplate
        {
            var type = typeof(T);
            if (AllTemplateTypes.Values.Contains(type)) {
                return this;
            }

            try {
                var instance = Activator.CreateInstance<T>();
                if (instance.Version == Guid.Empty) {
                    throw new NullReferenceException($"模板{type.Name}的version没有在构造函数里初始化");
                }

                var instanceName = instance.Name;
                if (string.IsNullOrEmpty(instanceName)) {
                    throw new NullReferenceException($"模板{type.Name}的name没有在构造函数里初始化！");
                }

                if (AllTemplateTypes.ContainsKey(instanceName)) {
                    throw new DuplicateNameException($"已经存在名为{instanceName}的模板类！");
                }
                AllTemplateTypes.Add(instanceName, type);
                
            }
            catch (Exception e) {
                throw new ArgumentOutOfRangeException($"无法生成模板{type.Name}实例！", e);
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