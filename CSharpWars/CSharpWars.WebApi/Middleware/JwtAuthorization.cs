namespace CSharpWars.WebApi.Middleware;

public class JwtAuthorization
{
    private static readonly JwtAuthorization _singleton = new JwtAuthorization();

    public static JwtAuthorization Instance => _singleton;
}

public class JwtAdminAuthorization
{
    private static readonly JwtAdminAuthorization _singleton = new JwtAdminAuthorization();

    public static JwtAdminAuthorization Instance => _singleton;
}