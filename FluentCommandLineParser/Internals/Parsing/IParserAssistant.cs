namespace Fclp.Internals.Parsing
{
    /// <summary>
    /// Parser assistant, lets the user indicate information about options as they are being parsed
    /// </summary>
    public interface IParserAssistant
    {
        /// <summary>
        /// Indicates that this option is in our optionName list
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
        bool IsBeingWatchedFor(string optionName);
    }
}