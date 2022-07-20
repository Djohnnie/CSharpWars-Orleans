namespace CSharpWars.WebApi.Middleware;

public class JwtAuthorization
{
    private static readonly JwtAuthorization _singleton = new JwtAuthorization();

    public static JwtAuthorization Instance => _singleton;
}