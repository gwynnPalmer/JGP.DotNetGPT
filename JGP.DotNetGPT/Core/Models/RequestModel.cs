using System.Text.Json.Serialization;

namespace JGP.DotNetGPT.Core.Models;

/// <summary>
///     Class request model
/// </summary>
public class RequestModel
{
    /// <summary>
    ///     Gets or sets the value of the model
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("model")]
    public string? Model { get; set; } = "gpt-3.5-turbo-0613";

    /// <summary>
    ///     Gets or sets the value of the messages
    /// </summary>
    /// <value>List&lt;Message&gt;</value>
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; } = new();

    /// <summary>
    ///     Gets or sets the value of the functions
    /// </summary>
    /// <value>List&lt;Function&gt;</value>
    [JsonPropertyName("functions")]
    public List<Function> Functions { get; private set; }

    /// <summary>
    ///     Gets or sets the value of the function call
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("function_call")]
    public string? FunctionCall { get; set; }

    /// <summary>
    ///     Appends the function using the specified function
    /// </summary>
    /// <param name="function">The function</param>
    /// <param name="functionCallValue">The function call value</param>
    public void AppendFunction(Function? function, string functionCallValue = "auto")
    {
        if (function == null) return;

        Functions ??= new List<Function>();
        Functions.Add(function);
        FunctionCall = functionCallValue;
    }

    /// <summary>
    ///     Sets the functions using the specified functions
    /// </summary>
    /// <param name="functions">The functions</param>
    /// <param name="functionCallValue">The function call value</param>
    public void AppendFunctions(List<Function>? functions, string functionCallValue = "auto")
    {
        if (functions == null || functions.Count == 0) return;

        Functions ??= new List<Function>();
        Functions.AddRange(functions);
        FunctionCall = functionCallValue;
    }
}

/// <summary>
///     Class message
/// </summary>
public class Message
{
    /// <summary>
    ///     Gets or sets the value of the role
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    /// <summary>
    ///     Gets or sets the value of the content
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    ///     Gets or sets the value of the name
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

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
    [JsonIgnore]
    public bool HasFunctionCall => FunctionCall != null;

    /// <summary>
    ///     Describes whether this instance equals
    /// </summary>
    /// <param name="obj">The obj</param>
    /// <returns>Interop+BOOL</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return obj is Message message
               && Role == message.Role
               && Content == message.Content
               && Name == message.Name
               && FunctionCall?.Equals(message.FunctionCall) == true;
    }
}

/// <summary>
///     Class function
/// </summary>
public class Function
{
    /// <summary>
    ///     Gets or sets the value of the name
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the value of the description
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the value of the parameters
    /// </summary>
    /// <value>System.Reflection.Metadata.Parameter</value>
    [JsonPropertyName("parameters")]
    public Parameter? Parameters { get; set; }
}

/// <summary>
///     Class parameter
/// </summary>
public class Parameter
{
    /// <summary>
    ///     Gets or sets the value of the type
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    ///     Gets or sets the value of the properties
    /// </summary>
    /// <value>Dictionary&lt;string, Property&gt;</value>
    [JsonPropertyName("properties")]
    public Dictionary<string, Property> Properties { get; set; } = new();

    /// <summary>
    ///     Gets or sets the value of the required
    /// </summary>
    /// <value>List&lt;string&gt;</value>
    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new();
}

/// <summary>
///     Class property
/// </summary>
public class Property
{
    /// <summary>
    ///     Gets or sets the value of the type
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    ///     Gets or sets the value of the description
    /// </summary>
    /// <value>System.String</value>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the value of the properties
    /// </summary>
    /// <value>Dictionary&lt;string, Property&gt;</value>
    [JsonPropertyName("properties")]
    public Dictionary<string, Property> Properties { get; set; } = new();

    /// <summary>
    ///     Gets or sets the value of the enum
    /// </summary>
    /// <value>List&lt;string&gt;</value>
    [JsonPropertyName("enum")]
    public List<string> Enum { get; set; } = new();
}