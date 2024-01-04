using System.Security.Claims;

namespace linc.Utility;

public static class SiteRolesHelper
{
    public const string AdministratorRoleName = "ADMINISTRATOR";
    public const string HeadEditorRoleName = "HEAD_EDITOR";
    public const string EditorRoleName = "EDITOR";
    public const string UserPlusRoleName = "USER_PLUS";
    public const string UserRoleName = "USER";

    public const int AdministratorWeight = 1;
    public const int HeadEditorWeight = 10;
    public const int EditorWeight = 15;
    public const int UserPlusWeight = 25;
    public const int UserWeight = 50;

    /// <summary>
    /// Assigns weights to roles, the smaller the weight the broader the access rights.
    /// </summary>
    public static readonly Dictionary<int, string> KnownRoles = new()
    {
        {AdministratorWeight, AdministratorRoleName},
        {HeadEditorWeight, HeadEditorRoleName},
        {EditorWeight, EditorRoleName},
        {UserPlusWeight, UserPlusRoleName},
        {UserWeight, UserRoleName}
    };

    /// <summary>
    /// Assigns preliminary guid id's to roles in order to be inserted in the database.
    /// </summary>
    public static readonly Dictionary<Guid, string> DatabaseRolesSeed = new()
    {
        {Guid.Parse(SiteConstant.ZeroGuid), AdministratorRoleName},
        {Guid.Parse("5e1199d7-7725-4900-aa34-5496365bf5a0"), HeadEditorRoleName},
        {Guid.Parse("05cbe4c7-108e-40bc-bee7-65438875026e"), EditorRoleName},
        {Guid.Parse("6b1acea8-2d26-4c82-b6ad-7281b7d621ae"), UserPlusRoleName},
        {Guid.Parse("90667439-9058-4956-96e6-d23bac481443"), UserRoleName}
    };

    public static SiteRole GetRole(string role)
    {
        if (string.IsNullOrEmpty(role))
        {
            return SiteRole.User;
        }

        if (!KnownRoles.ContainsValue(role))
        {
            throw new InvalidOperationException(
                $"Role '{role}' is not part of the known roles for the application.");
        }

        var knownRoleKey = KnownRoles.Single(x => x.Value == role).Key;
        return (SiteRole) knownRoleKey;
    }

    public static string GetRoleName(SiteRole role)
    {
        var roleWeight = (int) role;
        if (!KnownRoles.ContainsKey(roleWeight))
        {
            throw new InvalidOperationException(
                $"Role '{role}' ({roleWeight}) is not part of the known roles for the application.");
        }

        return KnownRoles[roleWeight];
    }

    public static IEnumerable<string> GetRoleNamesAbove(SiteRole role)
    {
        var roleWeight = (int)role;
        if (!KnownRoles.ContainsKey(roleWeight))
        {
            throw new InvalidOperationException(
                $"Role '{role}' ({roleWeight}) is not part of the known roles for the application.");
        }

        return KnownRoles.Where(x => x.Key <= roleWeight).Select(x => x.Value);
    }

    public static bool Is(this ClaimsPrincipal user, SiteRole role)
    {
        if (user.Identity is { IsAuthenticated: false })
        {
            // user is not authenticated
            // challenge him to log in
            return false;
        }

        var roleWeight = (int)role;
        if (!KnownRoles.ContainsKey(roleWeight))
        {
            throw new InvalidOperationException(
                $"Role '{role}' ({roleWeight}) is not part of the known roles for the application.");
        }

        return user.IsInRole(KnownRoles[roleWeight]);
    }

    public static bool IsAtLeast(this ClaimsPrincipal user, SiteRole role)
    {
        if (user.Identity is { IsAuthenticated: false })
        {
            // user is not authenticated
            // challenge him to log in
            return false;
        }

        var roleWeight = (int)role;
        if (!KnownRoles.ContainsKey(roleWeight))
        {
            throw new InvalidOperationException(
                $"Role '{role}' ({roleWeight}) is not part of the known roles for the application.");
        }

        foreach (var knownRole in KnownRoles)
        {
            if (user.IsInRole(knownRole.Value))
            {
                return knownRole.Key <= roleWeight;
            }
        }

        return false;
    }
}