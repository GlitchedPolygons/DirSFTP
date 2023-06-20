namespace GlitchedPolygons.DirSFTP.Models;

public record SftpConfig
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 22;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? PrivateKey { get; init; } = null;
    public string? PrivateKeyPassphrase { get; init; } = null;
}