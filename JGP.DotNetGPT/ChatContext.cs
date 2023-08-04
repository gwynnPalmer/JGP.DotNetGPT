using System.Text.Json;
using JGP.DotNetGPT.Core.Constants;
using JGP.DotNetGPT.Core.Models;
using SharpToken;

namespace JGP.DotNetGPT;

/// <summary>
///     Interface chat context
/// </summary>
public interface IChatContext
{
    /// <summary>
    ///     Gets the value of the length
    /// </summary>
    /// <value>int</value>
    int Length { get; }

    /// <summary>
    ///     Gets or sets the value of the upper token limit
    /// </summary>
    /// <value>int</value>
    int UpperTokenLimit { get; }

    /// <summary>
    ///     Appends the message using the specified message
    /// </summary>
    /// <param name="message">The message</param>
    void AppendMessage(Message? message);

    /// <summary>
    ///     Gets the safe context
    /// </summary>
    /// <returns>The context</returns>
    List<Message> GetSafeContext();

    /// <summary>
    ///     Dumps the context
    /// </summary>
    /// <returns>The message history</returns>
    List<Message> DumpContext();
}

/// <summary>
///     Class chat context
/// </summary>
/// <seealso cref="IChatContext" />
public class ChatContext : IChatContext
{
    /// <summary>
    ///     The GPT encoding token counter
    /// </summary>
    private static readonly GptEncoding TokenCounter = GptEncoding.GetEncoding("cl100k_base");

    /// <summary>
    ///     The message history
    /// </summary>
    private readonly List<Message> _messageHistory = new();


    /// <summary>
    ///     Initializes a new instance of the <see cref="ChatContext" /> class
    /// </summary>
    /// <param name="model">The model</param>
    public ChatContext(string model)
    {
        UpperTokenLimit = GetContextLimit(model);
    }

    /// <summary>
    ///     Gets the value of the length
    /// </summary>
    /// <value>int</value>
    public int Length => GetContextLength();

    /// <summary>
    ///     Gets or sets the value of the upper token limit
    /// </summary>
    /// <value>int</value>
    public int UpperTokenLimit { get; }

    /// <summary>
    ///     Appends the message using the specified message
    /// </summary>
    /// <param name="message">The message</param>
    public void AppendMessage(Message? message)
    {
        if (message == null) return;
        _messageHistory.Insert(_messageHistory.Count, message);
    }

    /// <summary>
    ///     Gets the safe context
    /// </summary>
    /// <returns>The context</returns>
    public List<Message> GetSafeContext()
    {
        var context = new List<Message>();
        var tokenCount = 0;
        var index = _messageHistory.Count - 1;

        while (tokenCount < UpperTokenLimit && index >= 0)
        {
            var message = _messageHistory[index];
            if (MessageExists(context, message)) break;

            var messageLength = TokenCounter.Encode(JsonSerializer.Serialize(message)).Count;
            if (messageLength == 0 || messageLength > UpperTokenLimit) message.Content = "Truncated message";

            if (tokenCount + messageLength > UpperTokenLimit) break;
            context.Insert(0, message);

            tokenCount += messageLength;
            index--;
        }

        //TODO: this should ideally never happen, we should prevent this earlier.
        HandleEmptyContext(context);

        return context;
    }


    /// <summary>
    ///     Dumps the context
    /// </summary>
    /// <returns>The message history</returns>
    public List<Message> DumpContext()
    {
        return _messageHistory;
    }

    #region PRIVATE METHODS

    /// <summary>
    ///     Gets the context length
    /// </summary>
    /// <returns>int</returns>
    private int GetContextLength()
    {
        var json = JsonSerializer.Serialize(_messageHistory);
        return TokenCounter.Encode(json).Count;
    }

    /// <summary>
    ///     Gets the context limit using the specified model
    /// </summary>
    /// <param name="model">The model</param>
    /// <returns>int</returns>
    private static int GetContextLimit(string model)
    {
        return model switch
        {
            ModelConstants.GPT4 => 7500,
            ModelConstants.GPT40613 => 7500,
            ModelConstants.GPT432k => 31050,
            ModelConstants.GPT432k0613 => 31500,
            ModelConstants.GPT35Turbo => 3500,
            ModelConstants.GPT35Turbo16k => 15500,
            ModelConstants.GPT35Turbo0613 => 3500,
            ModelConstants.GPT35Turbo16k0613 => 15500,
            _ => 3500
        };
    }

    /// <summary>
    ///     Handles the empty context using the specified context
    /// </summary>
    /// <param name="context">The context</param>
    private static void HandleEmptyContext(ICollection<Message> context)
    {
        if (context.Count == 0)
        {
            context.Add(new Message
            {
                Content = "The chat context was reset due to an error.",
                Role = ChatConstants.SystemRole
            });
        }
    }

    /// <summary>
    ///     Describes whether message exists
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="message">The message</param>
    /// <returns>Interop+BOOL</returns>
    private static bool MessageExists(List<Message> context, Message message)
    {
        return context.Exists(match => match.Equals(message));
    }

    #endregion
}