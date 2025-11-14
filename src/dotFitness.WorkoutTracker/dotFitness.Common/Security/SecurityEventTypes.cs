namespace dotFitness.Common.Security;

public static class SecurityEventTypes
{
    public const string LoginSuccess = nameof(LoginSuccess);
    public const string LoginFailure = nameof(LoginFailure);
    public const string Logout = nameof(Logout);
    public const string TokenRefresh = nameof(TokenRefresh);
    public const string UnauthorizedAccess = nameof(UnauthorizedAccess);
    public const string ForbiddenAccess = nameof(ForbiddenAccess);
    public const string ResourceAccessAttempt = nameof(ResourceAccessAttempt);
    public const string ResourceModificationAttempt = nameof(ResourceModificationAttempt);
    public const string UserCreation = nameof(UserCreation);
    public const string UserUpdate = nameof(UserUpdate);
    public const string RoleAssignment = nameof(RoleAssignment);
    public const string PasswordChange = nameof(PasswordChange);
    public const string AccountLockout = nameof(AccountLockout);
    public const string AccountUnlock = nameof(AccountUnlock);
    public const string RateLimitExceeded = nameof(RateLimitExceeded);
    
    // Legacy constants for backward compatibility
    public const string AdminAction =       nameof(AdminAction);
    public const string PtClientAccess = nameof(PtClientAccess);
    public const string ResourceAccessDenied = nameof(ResourceAccessDenied);
    public const string CrossUserAccessAttempt = nameof(CrossUserAccessAttempt);
}

public static class ResourceTypes
{
    public const string User = nameof(User);
    public const string Exercise = nameof(Exercise);
    public const string Workout = nameof(Workout);
    public const string WorkoutLog = nameof(WorkoutLog);
    public const string UserMetric = nameof(UserMetric);
    public const string Routine = nameof(Routine);
}

public static class Actions
{
    public const string Create = nameof(Create);
    public const string Read = nameof(Read);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string List = nameof(List);
    public const string Assign = nameof(Assign);
}
