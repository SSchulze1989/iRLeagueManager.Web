using System.Collections;
using System.Net;

namespace iRLeagueApiCore.Server.Models;

// Modified from Microsoft documentation
// https://learn.microsoft.com/de-de/dotnet/api/system.net.icredentials.getcredential?view=net-7.0
internal sealed class CredentialList : ICredentials
{
    private readonly Dictionary<(Uri uri, string authenticationType), NetworkCredential> storedCredentials;

    public CredentialList()
    {
        storedCredentials = new();
    }

    public CredentialList(IConfiguration configuration) : this()
    {
        foreach(var credentialConfig in configuration.GetChildren()) 
        {
            var credential = new NetworkCredential(credentialConfig["Username"], credentialConfig["Password"]);
            var uri = new Uri(credentialConfig["Uri"]);
            var authenticationType = credentialConfig["AuthenticationType"];
            Add(uri, authenticationType, credential);
        }
    }

    public void Add(Uri uri, string authenticationType, NetworkCredential credential)
    {
        // Add a 'CredentialInfo' object into a list.
        var key = (uri, authenticationType);
        if (storedCredentials.TryAdd(key, credential) == false)
        {
            storedCredentials[key] = credential;
        }
    }
    // Remove the 'CredentialInfo' object from the list that matches to the given 'Uri' and 'AuthenticationType'
    public void Remove(Uri uri, string authenticationType)
    {
        storedCredentials.Remove((uri, authenticationType));
    }
    public NetworkCredential? GetCredential(Uri uri, string authenticationType)
    {
        return storedCredentials.TryGetValue((uri, authenticationType), out var credential) ? credential : null;
    }
};
