namespace ApacheLogParser.Common.Services.Interfaces
{
    /// <summary>
    /// Interface for logging.
    /// </summary>
    public interface ILogger
    {
        void Error(string message);
        void Info(string message);
        void Debug(string messaga);
    }
}
