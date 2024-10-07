namespace Documents.Domain.Enums;

public static class UserRole
{
    public const string Operator = "operator";
    public const string Customer = "customer";

    public static string[] Roles => [Operator, Customer];
}