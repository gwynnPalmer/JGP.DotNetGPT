using System.Text;
using JGP.DotNetGPT.Core;

namespace JGP.DotNetGPT.Builders;

/// <summary>
///     Class request builder
/// </summary>
public static class RequestBuilder
{
    /// <summary>
    ///     Builds the chat request using the specified chat url
    /// </summary>
    /// <param name="chatUrl">The chat url</param>
    /// <param name="apiKey">The api key</param>
    /// <param name="content">The content</param>
    /// <param name="deploymentType">The deployment type</param>
    /// <returns>HttpRequestMessage</returns>
    public static HttpRequestMessage BuildChatRequest(string chatUrl, string apiKey, string content,
        DeploymentType deploymentType)
    {
        var keyHeader = BuildApiKeyHeader(deploymentType, apiKey);

        return new HttpRequestMessage(HttpMethod.Post, chatUrl)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json"),
            Headers =
            {
                { keyHeader.Key, keyHeader.Value }
            }
        };
    }

    /// <summary>
    ///     Builds the api key header using the specified deployment type
    /// </summary>
    /// <param name="deploymentType">The deployment type</param>
    /// <param name="apiKey">The api key</param>
    /// <returns>KeyValuePair&lt;string, string&gt;</returns>
    private static KeyValuePair<string, string> BuildApiKeyHeader(DeploymentType deploymentType, string apiKey)
    {
        return deploymentType == DeploymentType.Direct
            ? new KeyValuePair<string, string>("Authorization", "Bearer " + apiKey)
            : new KeyValuePair<string, string>("api-key", apiKey);
    }
}