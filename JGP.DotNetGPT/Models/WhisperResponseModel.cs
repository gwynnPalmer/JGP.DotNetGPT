using System.Text.Json.Serialization;

namespace JGP.DotNetGPT.Models;

/// <summary>
///     Class whisper response model
/// </summary>
public class WhisperResponseModel
{
    /// <summary>
    ///     Gets or sets the value of the text
    /// </summary>
    /// <value>System.Nullable&lt;string&gt;</value>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    ///     Gets or sets the value of the error
    /// </summary>
    /// <value>System.Nullable&lt;WhisperErrorModel&gt;</value>
    [JsonPropertyName("error")]
    public WhisperErrorModel? Error { get; set; }

    /// <summary>
    ///     Gets the value of the is success
    /// </summary>
    /// <value>Interop+BOOL</value>
    [JsonIgnore]
    public bool IsSuccess => !string.IsNullOrEmpty(Text);
}

/// <summary>
///     Class whisper error model
/// </summary>
public class WhisperErrorModel
{
    /// <summary>
    ///     Gets or sets the value of the message
    /// </summary>
    /// <value>System.Nullable&lt;string&gt;</value>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    ///     Gets or sets the value of the code
    /// </summary>
    /// <value>System.Nullable&lt;object&gt;</value>
    [JsonPropertyName("code")]
    public object? Code { get; set; }

    /// <summary>
    ///     Gets or sets the value of the param
    /// </summary>
    /// <value>System.Nullable&lt;object&gt;</value>
    [JsonPropertyName("param")]
    public object? Param { get; set; }

    /// <summary>
    ///     Gets or sets the value of the type
    /// </summary>
    /// <value>System.Nullable&lt;string&gt;</value>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}