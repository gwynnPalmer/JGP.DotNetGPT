using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using JGP.DotNetGPT.Models;
using SharpToken;

namespace JGP.DotNetGPT.Clients;

/// <summary>
///     Interface chat client
/// </summary>
public interface IChatClient
{
    /// <summary>
    ///     Appends the system message using the specified message
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns>ChatClient</returns>
    ChatClient AppendSystemMessage(string message);

    /// <summary>
    ///     Appends the function using the specified function
    /// </summary>
    /// <param name="function">The function</param>
    /// <returns>ChatClient</returns>
    ChatClient AppendFunction(Function function);

    /// <summary>
    ///     Removes the function using the specified name
    /// </summary>
    /// <param name="name">The name</param>
    /// <returns>ChatClient</returns>
    ChatClient RemoveFunction(string name);

    /// <summary>
    ///     Clears the functions
    /// </summary>
    /// <returns>ChatClient</returns>
    ChatClient ClearFunctions();

    /// <summary>
    ///     Submits the request model
    /// </summary>
    /// <param name="requestModel">The request model</param>
    /// <returns>Task&lt;Nullable&lt;ResponseModel?&gt;&gt;</returns>
    Task<ResponseModel?> SubmitAsync(RequestModel requestModel);

    /// <summary>
    ///     Submits the prompt
    /// </summary>
    /// <param name="prompt">The prompt</param>
    /// <param name="systemMessage">The system message</param>
    /// <returns>Task&lt;Nullable&lt;ResponseModel?&gt;&gt;</returns>
    Task<ResponseModel?> SubmitAsync(string prompt, string? systemMessage = null);

    /// <summary>
    ///     Submits the function response using the specified function name
    /// </summary>
    /// <param name="functionName">The function name</param>
    /// <param name="response">The response</param>
    /// <returns>Task&lt;Nullable&lt;ResponseModel?&gt;&gt;</returns>
    Task<ResponseModel?> SubmitFunctionResponseAsync(string functionName, string response);
}

/// <summary>
///     Class chat client
/// </summary>
/// <seealso cref="IChatClient" />
public class ChatClient : IChatClient
{
    /// <summary>
    ///     The from seconds
    /// </summary>
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    /// <summary>
    ///     The when writing null
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    ///     The get encoding
    /// </summary>
    private static readonly GptEncoding _encoding = GptEncoding.GetEncoding("cl100k_base");

    /// <summary>
    ///     The api key
    /// </summary>
    private readonly string _apiKey;

    /// <summary>
    ///     The chat url
    /// </summary>
    private readonly string _chatUrl;

    /// <summary>
    ///     The model
    /// </summary>
    private readonly string _model;

    /// <summary>
    ///     Gets the value of the context
    /// </summary>
    /// <value>List&lt;Message&gt;</value>
    public List<Message> Context { get; private set; } = new();

    /// <summary>
    ///     Gets the value of the functions
    /// </summary>
    /// <value>List&lt;Function&gt;</value>
    public List<Function> Functions { get; } = new();

    /// <summary>
    ///     Appends the system message using the specified message
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns>ChatClient</returns>
    public ChatClient AppendSystemMessage(string message)
    {
        Context.Insert(Context.Count, new Message
        {
            Role = ChatConstants.SystemRole,
            Content = message
        });

        return this;
    }

    #region CONSTRUCTORS

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChatClient" /> class
    /// </summary>
    /// <param name="chatUrl">The base url</param>
    /// <param name="apiKey">The api key</param>
    /// <param name="model">The model</param>
    private ChatClient(string chatUrl, string apiKey, string? model)
    {
        _chatUrl = chatUrl;
        _apiKey = apiKey;
        _model = string.IsNullOrEmpty(model)
            ? "gpt-3.5-turbo-0613"
            : model;
    }

    /// <summary>
    ///     Creates the api key
    /// </summary>
    /// <param name="apiKey">The api key</param>
    /// <param name="model">The model</param>
    /// <returns>ChatClient</returns>
    public static ChatClient Create(string apiKey, string? model = null)
    {
        return new ChatClient("https://api.openai.com/v1/chat/completions", apiKey, model);
    }

    #endregion

    #region FUNCTIONS

    /// <summary>
    ///     Appends the function using the specified function
    /// </summary>
    /// <param name="function">The function</param>
    /// <returns>ChatClient</returns>
    public ChatClient AppendFunction(Function function)
    {
        if (Functions.Count >= 3) return this;

        if (Functions.TrueForAll(x => !x.Name.Equals(function.Name, StringComparison.OrdinalIgnoreCase)))
        {
            Functions.Add(function);
        }

        return this;
    }

    /// <summary>
    ///     Removes the function using the specified name
    /// </summary>
    /// <param name="name">The name</param>
    /// <returns>ChatClient</returns>
    public ChatClient RemoveFunction(string name)
    {
        if (Functions.Count == 0) return this;

        Functions.RemoveAll(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        return this;
    }

    /// <summary>
    ///     Clears the functions
    /// </summary>
    /// <returns>ChatClient</returns>
    public ChatClient ClearFunctions()
    {
        Functions.Clear();

        return this;
    }

    #endregion

    #region SUBMISSION

    /// <summary>
    ///     Submits the request model
    /// </summary>
    /// <param name="requestModel">The request model</param>
    /// <returns>Task&lt;ResponseModel&gt;</returns>
    public async Task<ResponseModel?> SubmitAsync(RequestModel requestModel)
    {
        //TODO: Review Context handling in this situation
        Context = requestModel.Messages;
        TrimContext();
        requestModel.Messages = Context;

        var json = JsonSerializer.Serialize(requestModel, Options);

        using var chatRequest = new HttpRequestMessage(HttpMethod.Post, _chatUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
            Headers =
            {
                { "Authorization", "Bearer " + _apiKey }
            }
        };

        using var chatResponse = await _httpClient.SendAsync(chatRequest);
        var responseContent = await chatResponse.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<ResponseModel>(responseContent, Options);

        if (response is { Choices.Count: >= 1 })
        {
            Context.Insert(Context.Count, response.Choices[0].Message);
        }

        return response;
    }

    /// <summary>
    ///     Submits the prompt
    /// </summary>
    /// <param name="prompt">The prompt</param>
    /// <param name="systemMessage">The system message</param>
    /// <returns>Task&lt;ResponseModel?&gt;</returns>
    public async Task<ResponseModel?> SubmitAsync(string prompt, string? systemMessage = null)
    {
        var request = BuildRequest(prompt);
        return await SubmitAsync(request);
    }

    /// <summary>
    ///     Submits the function response using the specified function name
    /// </summary>
    /// <param name="functionName">The function name</param>
    /// <param name="response">The response</param>
    /// <returns>Task&lt;ResponseModel?&gt;</returns>
    public async Task<ResponseModel?> SubmitFunctionResponseAsync(string functionName, string response)
    {
        Context.Insert(Context.Count, new Message
        {
            Role = ChatConstants.FunctionRole,
            Name = functionName,
            Content = response
        });

        var request = BuildRequest();

        return await SubmitAsync(request);
    }

    #endregion

    #region HELPER METHODS

    /// <summary>
    ///     Trims the context
    /// </summary>
    private void TrimContext()
    {
        var uniqueMessages = new List<Message>();
        var seenMessages = new HashSet<string>();

        for (var i = Context.Count - 1; i >= 0; i--)
        {
            var message = Context[i];
            if (!string.IsNullOrEmpty(message.Content) && seenMessages.Add(message.Content))
            {
                uniqueMessages.Add(message);
            }
        }

        uniqueMessages.Reverse();
        Context = uniqueMessages;

        var contextString = Context
            .Aggregate(new StringBuilder(), (sb, message) => sb.Append(message.Content))
            .ToString();

        while (_encoding.Encode(contextString).Count > 3500)
        {
            Context.RemoveAt(0);
            contextString = Context
                .Aggregate(new StringBuilder(), (sb, message) => sb.Append(message.Content))
                .ToString();
        }
    }

    /// <summary>
    ///     Builds the request using the specified prompt
    /// </summary>
    /// <param name="prompt">The prompt</param>
    /// <param name="systemMessage">The system message</param>
    /// <returns>The request</returns>
    private RequestModel BuildRequest(string? prompt = null, string? systemMessage = null)
    {
        var request = new RequestModel
        {
            Model = _model,
            Messages = Context
        };

        if (Functions.Count > 0) request.AppendFunctions(Functions);

        if (!string.IsNullOrWhiteSpace(prompt))
        {
            request.Messages.Insert(request.Messages.Count, new Message
            {
                Role = ChatConstants.UserRole,
                Content = prompt
            });
        }

        if (!string.IsNullOrWhiteSpace(systemMessage))
        {
            request.Messages.Insert(request.Messages.Count, new Message
            {
                Role = ChatConstants.SystemRole,
                Content = systemMessage
            });
        }

        return request;
    }

    #endregion
}