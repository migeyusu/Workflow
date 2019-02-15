namespace WorkflowFacilities
{
    public interface ICustomActivity : IActivity
    {
        string Bookmark { get; set; }

        void Execute(PipelineContext context);

        void BookmarkCallback(PipelineContext context);
    }
}