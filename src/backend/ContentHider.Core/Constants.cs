namespace ContentHider.Core;

public static class Constants
{
    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
    }

    public static class Routes
    {
        public const string AuthRoute = "/auth";

        public const string OrganizationRoute = "/organizations";
        public const string OrganizationPreviewRoute = $"{OrganizationRoute}/preview";

        public const string TextFormatRoute = $"{OrganizationRoute}/{{orgId}}/formats";
        public const string RuleRoute = $"{TextFormatRoute}/{{formatId}}/rules";
    }
}