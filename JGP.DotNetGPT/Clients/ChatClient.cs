using System.Text.Json;
using System.Text.Json.Serialization;
using JGP.DotNetGPT.Builders;
using JGP.DotNetGPT.Core;
using JGP.DotNetGPT.Core.Constants;
using JGP.DotNetGPT.Core.Models;

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
    ///     The when writing null
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    ///     The api key
    /// </summary>
    private readonly string _apiKey;

    /// <summary>
    ///     The chat url
    /// </summary>
    private readonly string _chatUrl;

    /// <summary>
    ///     The deployment type
    /// </summary>
    private readonly DeploymentType _deploymentType;

    /// <summary>
    ///     The model
    /// </summary>
    private readonly string _model;

    /// <summary>
    ///     The from seconds
    /// </summary>
    private HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(60)
    };

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChatClient" /> class
    /// </summary>
    /// <param name="chatUrl">The base url</param>
    /// <param name="apiKey">The api key</param>
    /// <param name="model">The model</param>
    private ChatClient(string chatUrl, string apiKey, string? model,
        DeploymentType deploymentType = DeploymentType.Direct)
    {
        _apiKey = apiKey;
        _chatUrl = chatUrl;
        _deploymentType = deploymentType;

        _model = string.IsNullOrEmpty(model)
            ? ModelConstants.GPT35Turbo16k
            : model;

        Context = new ChatContext(_model);

    }

    /// <summary>
    ///     Gets or sets the value of the context
    /// </summary>
    /// <value>IChatContext</value>
    public IChatContext Context { get; }

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
        var systemMessage = new Message
        {
            Role = ChatConstants.SystemRole,
            Content = message
        };

        Context.AppendMessage(systemMessage);

        return this;
    }

    /// <summary>
    ///     Creates the api key
    /// </summary>
    /// <param name="apiKey">The api key</param>
    /// <param name="model">The model</param>
    /// <returns>ChatClient</returns>
    public static ChatClient CreateDirectDeployment(string apiKey, string? model = null)
    {
        return new ChatClient("https://api.openai.com/v1/chat/completions", apiKey, model);
    }

    /// <summary>
    ///     Creates the azure deployment using the specified api key
    /// </summary>
    /// <param name="chatUrl">The chat url</param>
    /// <param name="apiKey">The api key</param>
    /// <param name="model">The model</param>
    /// <returns>ChatClient</returns>
    public static ChatClient CreateAzureDeployment(string chatUrl, string apiKey, string? model = null)
    {
        return new ChatClient(chatUrl, apiKey, model, DeploymentType.Azure);
    }

    /// <summary>
    ///     Sets the client timeout using the specified seconds
    /// </summary>
    /// <param name="seconds">The seconds</param>
    /// <returns>ChatClient</returns>
    public ChatClient SetClientTimeout(int seconds)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(seconds)
        };

        return this;
    }

    #region HELPER METHODS

    /// <summary>
    ///     Builds the request using the specified prompt
    /// </summary>
    /// <param name="prompt">The prompt</param>
    /// <param name="systemMessage">The system message</param>
    /// <returns>The request</returns>
    private RequestModel BuildRequest(string? prompt = null, string? systemMessage = null)
    {
        if (!string.IsNullOrWhiteSpace(prompt))
        {
            var message = new Message
            {
                Role = ChatConstants.UserRole,
                Content = prompt
            };

            Context.AppendMessage(message);
        }

        if (!string.IsNullOrWhiteSpace(systemMessage))
        {
            var message = new Message
            {
                Role = ChatConstants.SystemRole,
                Content = systemMessage
            };

            Context.AppendMessage(message);
        }

        var request = new RequestModel
        {
            Model = _model,
            Messages = Context.GetSafeContext()
        };

        if (Functions.Count > 0) request.AppendFunctions(Functions);

        return request;
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

        for (var i = 0; i < requestModel.Messages.Count; i++)
        {
            Context.AppendMessage(requestModel.Messages[i]);
        }

        requestModel.Messages = Context.GetSafeContext();

        var json = JsonSerializer.Serialize(requestModel, Options);

        using var chatRequest = RequestBuilder.BuildChatRequest(_chatUrl, _apiKey, json, _deploymentType);
        using var chatResponse = await _httpClient.SendAsync(chatRequest);

        var responseContent = await chatResponse.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<ResponseModel>(responseContent, Options);

        if (response is { Choices.Count: >= 1 })
        {
            Context.AppendMessage(response.Choices[0].Message);
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
        var message = new Message
        {
            Role = ChatConstants.FunctionRole,
            Name = functionName,
            Content = response
        };

        Context.AppendMessage(message);

        var request = BuildRequest();

        return await SubmitAsync(request);
    }

    #endregion
}