namespace ContentHider.Presentation.Api;

public static class Constants
{
    public static class Routes
    {
        public const string OrganizationRoute = "/org";
        public const string TextFormatRoute = $"{OrganizationRoute}/{{orgId}}/format";
    }
}