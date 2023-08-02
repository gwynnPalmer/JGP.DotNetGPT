using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGP.DotNetGPT.Models;

/// <summary>
///     Class response model
/// </summary>
public class ResponseModel
{
    /// <summary>
    ///     Gets or sets the value of the id
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     Gets or sets the value of the object
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    /// <summary>
    ///     Gets or sets the value of the created
    /// </summary>
    /// <value>long</value>
    [JsonPropertyName("created")]
    public long? Created { get; set; }

    /// <summary>
    ///     Gets or sets the value of the model
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    ///     Gets or sets the value of the choices
    /// </summary>
    /// <value>List&lt;Choice&gt;</value>
    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; set; }

    /// <summary>
    ///     Gets or sets the value of the usage
    /// </summary>
    /// <value>System.CommandLine.Help.DefaultHelpText+Usage</value>
    [JsonPropertyName("usage")]
    public Usage Usage { get; set; }

    /// <summary>
    ///     Gets or sets the value of the error
    /// </summary>
    /// <value>System.Nullable&lt;Error&gt;</value>
    [JsonPropertyName("error")]
    public Error? Error { get; set; }

    /// <summary>
    ///     Describes whether this instance is function call
    /// </summary>
    /// <returns>true if this instance is [function call]; otherwise, false.</returns>
    public bool IsFunctionCall()
    {
        return Choices?.Count >= 1 && Choices[0].Message.HasFunctionCall;
    }

    /// <summary>
    ///     Describes whether this instance is success
    /// </summary>
    /// <returns>true if this instance is [success]; otherwise, false.</returns>
    public bool IsSuccess()
    {
        return Error == null;
    }
}

/// <summary>
///     Class choice
/// </summary>
public class Choice
{
    /// <summary>
    ///     Gets or sets the value of the index
    /// </summary>
    /// <value>int</value>
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    /// <summary>
    ///     Gets or sets the value of the message
    /// </summary>
    /// <value>ResponseMessage</value>
    [JsonPropertyName("message")]
    public ResponseMessage? Message { get; set; }

    /// <summary>
    ///     Gets or sets the value of the finish reason
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

/// <summary>
///     Class response message
/// </summary>
/// <seealso cref="Message" />
public class ResponseMessage : Message
{
    /// <summary>
    ///     Gets or sets the value of the function call
    /// </summary>
    /// <value>System.Nullable&lt;FunctionCall&gt;</value>
    [JsonPropertyName("function_call")]
    public FunctionCall? FunctionCall { get; set; }

    /// <summary>
    ///     Gets the value of the has function call
    /// </summary>
    /// <value>Interop+BOOL</value>
    public bool HasFunctionCall => FunctionCall != null;
}

/// <summary>
///     Class function call
/// </summary>
public class FunctionCall
{
    /// <summary>
    ///     Gets or sets the value of the name
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the value of the arguments
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("arguments")]
    public string? Arguments { get; set; }

    /// <summary>
    ///     Returns the function parameters using the specified options
    /// </summary>
    /// <typeparam name="T">The </typeparam>
    /// <param name="options">The options</param>
    /// <returns>T</returns>
    public T? ToFunctionParameters<T>(JsonSerializerOptions? options = null) where T : class
    {
        return JsonSerializer.Deserialize<T>(Arguments, options);
    }
}

/// <summary>
///     Class usage
/// </summary>
public class Usage
{
    /// <summary>
    ///     Gets or sets the value of the prompt tokens
    /// </summary>
    /// <value>int</value>
    [JsonPropertyName("prompt_tokens")]
    public int? PromptTokens { get; set; }

    /// <summary>
    ///     Gets or sets the value of the completion tokens
    /// </summary>
    /// <value>int</value>
    [JsonPropertyName("completion_tokens")]
    public int? CompletionTokens { get; set; }

    /// <summary>
    ///     Gets or sets the value of the total tokens
    /// </summary>
    /// <value>int</value>
    [JsonPropertyName("total_tokens")]
    public int? TotalTokens { get; set; }
}

/// <summary>
///     Class error
/// </summary>
public class Error
{
    /// <summary>
    ///     Gets or sets the value of the message
    /// </summary>
    /// <value>System.Nullable&lt;string&gt;</value>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    ///     Gets or sets the value of the type
    /// </summary>
    /// <value>System.Nullable&lt;string&gt;</value>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}