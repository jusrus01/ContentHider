namespace ContentHider.Presentation.Api;

public static class Constants
{
    public static class Routes
    {
        public const string OrganizationRoute = "/organizations";
        public const string OrganizationPreviewRoute = $"{OrganizationRoute}/preview";

        public const string TextFormatRoute = $"{OrganizationRoute}/{{orgId}}/formats";
        public const string RuleRoute = $"{TextFormatRoute}/{{formatId}}/rules";
    }
}