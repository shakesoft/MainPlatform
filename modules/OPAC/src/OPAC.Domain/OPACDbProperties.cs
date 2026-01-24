namespace OPAC;

public static class OPACDbProperties
{
    public static string DbTablePrefix { get; set; } = "OPAC";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "OPAC";
}
