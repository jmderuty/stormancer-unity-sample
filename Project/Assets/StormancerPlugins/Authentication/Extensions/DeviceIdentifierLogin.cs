
namespace Stormancer
{
    // ** NOTE GRI (2018/07/31) : : Il faut passer par la classe Account.
    public static class DeviceIdentifierLogin
    {
        //public static Task Authenticate(this AuthenticationService authenticationService)
        //{
        //    string identifier = GetIdentifier();         

        //    ClientProvider.GetService<ILogger>().Log(Diagnostics.LogLevel.Debug, "authenticationservice", "Logging in with identifier " + identifier);

        //    return authenticationService.Login();
        //}

        public static string GetIdentifier()
        {
            var identifier = UnityEngine.SystemInfo.deviceUniqueIdentifier;
#if UNITY_EDITOR
            identifier = identifier + "editor";
#endif
            return identifier;
        }
    }
}
