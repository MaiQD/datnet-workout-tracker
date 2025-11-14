namespace dotFitness.Common.Authorization;

public static class AuthorizationPolicies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string PtOnly = nameof(PtOnly);
    public const string UserOnly = nameof(UserOnly);
    public const string SelfOrAdmin = nameof(SelfOrAdmin);
    public const string ResourceOwner = nameof(ResourceOwner);
    public const string PtAssignedOrAdmin = nameof(PtAssignedOrAdmin);
    public const string OwnerOrPtOrAdmin = nameof(OwnerOrPtOrAdmin);
}

public static class Roles
{
    public const string Admin = nameof(Admin);
    public const string Pt = nameof(Pt);
    public const string User = nameof(User);
}

public static class ClaimTypes
{
    public const string UserId = "user_id";
    public const string Email = "email";
    public const string DisplayName = "display_name";
    public const string GoogleId = "google_id";
}
