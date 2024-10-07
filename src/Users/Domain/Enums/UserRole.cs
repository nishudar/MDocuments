namespace Users.Domain.Enums;

internal static class UserRole
{
    public const string Operator = "operator";
    public const string Customer = "customer";
    public static string[] Roles => [Operator, Customer];
}