namespace Presentation_Layer.Policies
{
    public static class RateLimitPolicies
    {
        public const string AuthPolicy = "AuthPolicy";
        public const string PublicRead = "PublicRead";
        public const string LargePublicRead = "LargePublicRead";
        public const string UserRead = "UserRead";
        public const string Write = "Write";
        public const string Upload = "Upload";
        public const string ExternalOperation = "ExternalOperation";
        public const string Webhook = "Webhook";
    }
}
