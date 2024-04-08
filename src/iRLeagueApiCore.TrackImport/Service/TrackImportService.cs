using iRLeagueApiCore.TrackImport.Models;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace iRLeagueApiCore.TrackImport.Service;

public sealed class TrackImportService
{
    public const double m2km = 1.60934;

    private readonly HttpClient httpClient;
    private const string AuthenticationUrl = "https://members-ng.iracing.com/auth";
    private const string GetTrackDataUrl = "https://members-ng.iracing.com/data/track/get";
    private const string GetTrackAssetsUrl = "https://members-ng.iracing.com/data/track/assets";

    public TrackImportService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<bool> Authenticate(string userName, string password, CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, string>()
            {
                { "email", userName },
                { "password", EncodePassword(userName, password) },
            };
        var content = new FormUrlEncodedContent(data);
        var response = await httpClient.PostAsync(AuthenticationUrl, content, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<IRacingAuthResponse>();
        if (result?.Authcode == null || result.Authcode == "0")
        {
            return false;
        }
        return true;
    }

    public async Task<IEnumerable<TrackImportModel>> GetTracksData(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetAsync(GetTrackDataUrl);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Result was: {result.StatusCode}");
        }
        var linkResponse = await result.Content.ReadFromJsonAsync<LinkResponseModel>(cancellationToken: cancellationToken);
        result = await httpClient.GetAsync(linkResponse.link);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Result was: {result.StatusCode}");
        }
        var data = await result.Content.ReadFromJsonAsync<IEnumerable<TrackImportModel>>(cancellationToken: cancellationToken);
        return data!;
    }

    public async Task<IDictionary<int, TrackAssetsModel>> GetTrackAssets(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetAsync(GetTrackAssetsUrl);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Result was: {result.StatusCode}");
        }
        var linkResponse = await result.Content.ReadFromJsonAsync<LinkResponseModel>(cancellationToken: cancellationToken);
        result = await httpClient.GetAsync(linkResponse.link);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Result was: {result.StatusCode}");
        }
        var data = await result.Content.ReadFromJsonAsync<IDictionary<int, TrackAssetsModel>>(cancellationToken: cancellationToken);
        return data!;
    }

    public async Task<TrackMapLayersModel> LoadTrackSvgs(TrackAssetsModel trackAssets, CancellationToken cancellationToken = default)
    {
        var svgLayers = new TrackMapLayersModel
        {
            active = await GetSvg(trackAssets.track_map + trackAssets.track_map_layers.active, cancellationToken),
            background = await GetSvg(trackAssets.track_map + trackAssets.track_map_layers.background, cancellationToken),
            pitroad = await GetSvg(trackAssets.track_map + trackAssets.track_map_layers.pitroad, cancellationToken),
            inactive = await GetSvg(trackAssets.track_map + trackAssets.track_map_layers.inactive, cancellationToken),
            start_finish = await GetSvg(trackAssets.track_map + trackAssets.track_map_layers.start_finish, cancellationToken),
            turns = await GetSvg(trackAssets.track_map + trackAssets.track_map_layers.turns, cancellationToken)
        };

        return svgLayers;
    }

    private async Task<string> GetSvg(string link, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetAsync(link, cancellationToken);
        if (result.IsSuccessStatusCode == false)
        {
            throw new Exception($"Result was: {result.StatusCode}");
        }
        return await result.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
    }

    public static string EncodePassword(string username, string password)
    {
        var stringData = $"{password}{username.ToLower()}";
        var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringData));
        return Convert.ToBase64String(bytes);
    }
}
