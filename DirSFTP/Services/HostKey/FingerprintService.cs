using System.Security.Cryptography;
using System.Text.Json;
using GlitchedPolygons.ExtensionMethods;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace GlitchedPolygons.DirSFTP.Services.HostKey;

public class FingerprintService : IFingerprintService
{
    private readonly ILogger<FingerprintService> logger;

    public FingerprintService(ILogger<FingerprintService> logger)
    {
        this.logger = logger;
    }

    public async Task<string> GetFingerprint(string host, int port, int timeoutSeconds = 6)
    {
        string fingerprint = null;

        using SftpClient sftpClient = new(host, port, "guest", "password");

        sftpClient.HostKeyReceived += (s, e) =>
        {
            string hostKeySHA256 = SHA256.HashData(e.HostKey).ToBase64String(true);

            fingerprint = $"{e.HostKeyName} SHA256: {hostKeySHA256}";
        };

        _ = Task.Run(sftpClient.Connect); // We don't need any result here, just call Connect for the HostKeyReceived event to be triggered.

        DateTime timeoutUTC = DateTime.UtcNow + TimeSpan.FromSeconds(timeoutSeconds);

        while (fingerprint is null && DateTime.UtcNow < timeoutUTC)
        {
            await Task.Delay(64);
        }

        return fingerprint;
    }

    public async Task<bool> CheckFingerprint(string host, int port, int timeoutSeconds = 6)
    {
        if (host.NullOrEmpty())
        {
            throw new ArgumentException($"{nameof(FingerprintService)}::{nameof(CheckFingerprint)}: The passed {nameof(host)} parameter is either null or empty!", nameof(host));
        }

        if (port is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port), $"{nameof(FingerprintService)}::{nameof(CheckFingerprint)}: The passed {nameof(port)} is outside of the valid unsigned 16-bit integer range of [0; 65535]");
        }

        string dictionaryKey = $"{host}:{port}";

        string fetchedFingerprint = await GetFingerprint(host, port, timeoutSeconds);

        if (fetchedFingerprint.NullOrEmpty())
        {
            logger.LogError("{Class}::{Method}: Failed to connect to {Host} while checking for host key fingerprint authenticity", nameof(FingerprintService), nameof(CheckFingerprint), dictionaryKey);
            return false;
        }
        
        if (Application.Current?.MainPage is null)
        {
            logger.LogWarning("{Class}::{Method}: Failed to initiate host key fingerprint alert dialogs due to Application.Current?.MainPage being null!", nameof(FingerprintService), nameof(CheckFingerprint));
            return false;
        }

        IDictionary<string, string> storedFingerprints = await GetAllStoredFingerprints();

        if (storedFingerprints.TryGetValue(dictionaryKey, out string storedFingerprint))
        {
            if (storedFingerprint == fetchedFingerprint)
            {
                return true;
            }

            string warningTitle = $"Host key fingerprint mismatch for {dictionaryKey}";
            string warningMessage = $"The host \"{dictionaryKey}\" reported the following public key fingerprint:\n\n{fetchedFingerprint}\n\nDuring setup of the associated synchronized directory entry, you accepted the following as the trusted host key fingerprint:\n\n{storedFingerprint}\n\nThese two are different! This could either be due to the host having changed keys, or a man-in-the-middle attack (hopefully not!).\n\nHow should this be handled?\n\nClicking on \"Yes\" will accept the new host key fingerprint and overwrite the currently stored one; \"No\" will reject the key returned by the server and keep everything as it was (connection won't happen in this case).";

            if (await Application.Current.MainPage.DisplayAlert(warningTitle, warningMessage, "Yes, trust & update", "No, abort now"))
            {
                storedFingerprints[dictionaryKey] = fetchedFingerprint;

                await UpdateStoredFingerprints(storedFingerprints);

                return true;
            }

            return false;
        }

        const string dialogTitle = "Host key fingerprint check";
        string dialogMessage = $"The host \"{dictionaryKey}\" reported the following public key fingerprint:\n\n{fetchedFingerprint}\n\nIs this correct? Do you trust it?";

        if (await Application.Current.MainPage.DisplayAlert(dialogTitle, dialogMessage, "Yes, trust & save", "No, abort now"))
        {
            storedFingerprints[dictionaryKey] = fetchedFingerprint;

            await UpdateStoredFingerprints(storedFingerprints);

            return true;
        }
        
        return false;
    }

    public async Task<IDictionary<string, string>> GetAllStoredFingerprints()
    {
        string jsonBase64 = await SecureStorage.GetAsync("StoredFingerprints");

        if (jsonBase64.NullOrEmpty())
        {
            return new Dictionary<string, string>();
        }

        string json = jsonBase64.FromBase64String();

        return JsonSerializer.Deserialize<IDictionary<string, string>>(json);
    }

    public Task<bool> RemoveStoredFingerprint(string host, int port)
    {
        return RemoveStoredFingerprint($"{host}:{port}");
    }

    public async Task<bool> RemoveStoredFingerprint(string hostId)
    {
        IDictionary<string, string> storedFingerprints = await GetAllStoredFingerprints();

        if (!storedFingerprints.ContainsKey(hostId))
        {
            return false;
        }

        try
        {
            storedFingerprints.Remove(hostId);

            await UpdateStoredFingerprints(storedFingerprints);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task RemoveAllStoredFingerprints()
    {
        return UpdateStoredFingerprints(new Dictionary<string, string>());
    }

    private Task UpdateStoredFingerprints(IDictionary<string, string> fingerprints)
    {
        string json = JsonSerializer.Serialize(fingerprints);

        return SecureStorage.SetAsync("StoredFingerprints", json.ToBase64String());
    }
}