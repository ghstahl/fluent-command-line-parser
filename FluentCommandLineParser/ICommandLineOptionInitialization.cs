using Fclp.Internals.Validators;

namespace Fclp
{
    /// <summary>
    /// Provides the internal intialization interface for a newly created option"/> object.
    /// </summary>
    public interface ICommandLineOptionInitialization
    {
        /// <summary>
        /// Sets the option validator so that prior to accepting any option name it gets checked
        /// </summary>
        /// <param name="optionValidator"></param>
        void SetOptionValidator(ICommandLineOptionValidator optionValidator);
    }
}