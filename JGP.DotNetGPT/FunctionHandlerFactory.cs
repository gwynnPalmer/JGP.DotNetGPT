namespace JGP.DotNetGPT;

/// <summary>
///     Class function handler factory
/// </summary>
public class FunctionHandlerFactory
{
    /// <summary>
    ///     The function handlers
    /// </summary>
    private readonly Dictionary<string, Func<string, Task<object>>> _functionHandlers = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="FunctionHandlerFactory" /> class
    /// </summary>
    private FunctionHandlerFactory()
    {
    }

    /// <summary>
    ///     Creates
    /// </summary>
    /// <returns>FunctionHandlerFactory</returns>
    public static FunctionHandlerFactory Create()
    {
        return new FunctionHandlerFactory();
    }

    /// <summary>
    ///     Adds the function handler using the specified function name
    /// </summary>
    /// <param name="functionName">The function name</param>
    /// <param name="functionHandler">The function handler</param>
    /// <returns>FunctionHandlerFactory</returns>
    public FunctionHandlerFactory AddFunctionHandler(string functionName, Func<string, Task<object>> functionHandler)
    {
        _functionHandlers.Add(functionName, functionHandler);
        return this;
    }

    /// <summary>
    ///     Executes the function handler using the specified function name
    /// </summary>
    /// <param name="functionName">The function name</param>
    /// <param name="parameter">The parameter</param>
    /// <exception cref="InvalidOperationException">No handler found for function: {functionName}</exception>
    /// <returns>Task&lt;object&gt;</returns>
    public async Task<object> ExecuteFunctionHandlerAsync(string functionName, string parameter)
    {
        if (_functionHandlers.TryGetValue(functionName, out var handler))
        {
            return await handler(parameter);
        }

        throw new InvalidOperationException($"No handler found for function: {functionName}");
    }
}