using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using WeatherService.Domain.Interfaces;
using WeatherService.Domain.Options;

namespace WeatherService.Infrastructure.DataClients;

public class HttpDataClient<TIn, TOut, TOptions> : IDataClient<TIn, TOut>
    where TOptions : class, IHttpDataOptions<TIn>
{
    private readonly HttpClient _client;
    private readonly TOptions _options;

    private readonly AsyncPolicy<HttpResponseMessage> _retryPolicy;

    public HttpDataClient(IOptionsFactory<TOptions> options, HttpClient client)
    {
        _options = options.Create(TOptions.Name);
        _client = client;

        _retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.Unauthorized)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.1, retryAttempt)), async (response, _) =>
            {
                if (response.Result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.Warning("Unauthorized, re-authenticating");

                    await AuthenticateAsync();
                }
            });
    }

    public async Task<TOut?> GetDataAsync(TIn @in, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _client.GetAsync(_options.BuildUrl(@in), cancellationToken));

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<TOut>(responseBody);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while fetching data");
            throw;
        }
    }

    private async Task AuthenticateAsync()
    {
        _client.DefaultRequestHeaders.Clear();
        var responseMessage = await _client.PostAsync($"{_options.BaseUrl}/{_options.AuthenticationEndpoint}", new StringContent(JsonConvert.SerializeObject(new
        {
            _options.Username, _options.Password
        }), Encoding.UTF8, "application/json"));

        responseMessage.EnsureSuccessStatusCode();

        var responseBody = await responseMessage.Content.ReadAsStringAsync();

        var token = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody)?["token"];

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }
}