namespace ComputerRepairSystem_winui.Services;

public static class CurrentUser
{
    public static bool IsLoggedIn { get; private set; }

    public static string UserId { get; private set; } = string.Empty;

    public static string UserName { get; private set; } = string.Empty;

    public static string Role { get; private set; } = string.Empty;

    public static int? CompanyId { get; private set; }


    public static void Login(
        string userId,
        string userName,
        string role,
        int? companyId)
    {
        UserId = userId;
        UserName = userName;
        Role = role;
        CompanyId = companyId;

        IsLoggedIn = true;
    }


    public static void Logout()
    {
        UserId = string.Empty;
        UserName = string.Empty;
        Role = string.Empty;
        CompanyId = null;

        IsLoggedIn = false;
    }
}
