namespace Collaboration;

public static class CollaborationDbProperties
{
    public static string DbTablePrefix { get; set; } = "Collaboration";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "Collaboration";
}
