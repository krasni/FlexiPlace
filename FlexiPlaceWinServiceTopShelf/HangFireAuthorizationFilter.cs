using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Owin;

public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var owinContext = new OwinContext(context.GetOwinEnvironment());

        // Allow all authenticated users to see the Dashboard (potentially dangerous).
        return true;
    }
}