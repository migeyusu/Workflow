using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    public interface ICustomActivity : IActivity
    {
        /// <summary>
        /// 表示是否执行成功，默认返回true，返回false将导致工作流结束
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool Execute(PipelineContext context);

        void BookmarkCallback(PipelineContext context,string bookmarkName,object value);
    }
}