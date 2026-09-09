namespace ComputerRepairSystem_winui.Services;

public static class CurrentUser
{
    public static bool IsLoggedIn { get; private set; }

    public static string UserId { get; private set; }
        = string.Empty;

    public static string UserName { get; private set; }
        = string.Empty;

    public static string Role { get; private set; }
        = string.Empty;


    public static void Login(
        string userId,
        string userName,
        string role)
    {
        UserId = userId;
        UserName = userName;
        Role = role;

        IsLoggedIn = true;
    }


    public static void Logout()
    {
        UserId = string.Empty;
        UserName = string.Empty;
        Role = string.Empty;

        IsLoggedIn = false;
    }
}