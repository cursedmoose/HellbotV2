namespace Hellbot.Service.Controllers.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public sealed class IncludeUserContextAttribute : Attribute
    {
    }
}
