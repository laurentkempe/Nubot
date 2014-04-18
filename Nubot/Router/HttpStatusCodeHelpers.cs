namespace Nubot.Router
{
    public static class HttpStatusCodeHelpers
    {
        public static Nancy.HttpStatusCode ToNancyHttpStatusCode(this Interfaces.HttpStatusCode nubotStatus)
        {
            return (Nancy.HttpStatusCode) nubotStatus;
        }
    }
}