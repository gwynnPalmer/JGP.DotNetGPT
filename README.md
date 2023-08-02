# README

This README provides an overview and documentation for the `JGP.DotNetGPT` C# library.

## ChatConstants

The `ChatConstants` class contains constant values for different roles in a chat application.

- `UserRole` - Represents a user role.
- `SystemRole` - Represents a system role.
- `AssistantRole` - Represents an assistant role.
- `FunctionRole` - Represents a function role.

## FunctionHandlerFactory

The `FunctionHandlerFactory` class is responsible for managing function handlers. It allows adding and executing function handlers.

- `AddFunctionHandler` - Adds a function handler with the specified function name and handler function.
- `ExecuteFunctionHandlerAsync` - Executes the function handler for the specified function name and parameter.

## Models

### RequestModel

The `RequestModel` class represents a request model for sending messages and functions to the chat API.

- `Model` - The model to use for the chat. Default value is "gpt-3.5-turbo-0613".
- `Messages` - The list of messages in the chat.
- `Functions` - The list of functions to execute.
- `FunctionCall` - The function call value for the executed functions.

### Message

The `Message` class represents a message in a chat.

- `Role` - The role of the message.
- `Content` - The content of the message.
- `Name` - The name of the message.

### Function

The `Function` class represents a function that can be executed in the chat.

- `Name` - The name of the function.
- `Description` - The description of the function.
- `Parameters` - The parameters of the function.

### Parameter

The `Parameter` class represents a parameter of a function.

- `Type` - The type of the parameter.
- `Properties` - The properties of the parameter.
- `Required` - The required properties of the parameter.

### Property

The `Property` class represents a property of a parameter.

- `Type` - The type of the property.
- `Description` - The description of the property.
- `Properties` - The sub-properties of the property.
- `Enum` - The enum values of the property.

### ResponseModel

The `ResponseModel` class represents a response model from the chat API.

- `Id` - The ID of the response.
- `Object` - The object type of the response.
- `Created` - The timestamp when the response was created.
- `Model` - The model used for the response.
- `Choices` - The list of choices in the response.
- `Usage` - The usage information of the response.
- `Error` - The error information of the response.

### Choice

The `Choice` class represents a choice in the response.

- `Index` - The index of the choice.
- `Message` - The response message of the choice.
- `FinishReason` - The finish reason of the choice.

### ResponseMessage

The `ResponseMessage` class represents a response message in the response.

- `FunctionCall` - The function call of the message.
- `HasFunctionCall` - Indicates whether the message has a function call.

### FunctionCall

The `FunctionCall` class represents a function call in a response message.

- `Name` - The name of the function.
- `Arguments` - The arguments of the function call.

### Usage

The `Usage` class represents the token usage information of the response.

- `PromptTokens` - The number of tokens used for prompt.
- `CompletionTokens` - The number of tokens used for completion.
- `TotalTokens` - The total number of tokens used.

### Error

The `Error` class represents an error in the response.

- `Message` - The error message.
- `Type` - The type of the error.

### WhisperResponseModel

The `WhisperResponseModel` class represents a response model for whisper messages.

- `Text` - The text content of the message.
- `Error` - The error information of the message.

### WhisperErrorModel

The `WhisperErrorModel` class represents an error model for whisper messages.

- `Message` - The error message.
- `Code` - The error code.
- `Param` - The error parameter.
- `Type` - The type of the error.

## Clients

### IChatClient

The `IChatClient` interface defines the contract for a chat client.

- `AppendSystemMessage` - Appends a system message to the chat context.
- `AppendFunction` - Appends a function to the chat context.
- `RemoveFunction` - Removes a function from the chat context.
- `ClearFunctions` - Clears all functions from the chat context.
- `SubmitAsync` - Submits a request model to the chat API and returns the response.
- `SubmitFunctionResponseAsync` - Submits a function response to the chat API and returns the response.

### ChatClient

The `ChatClient` class implements the `IChatClient` interface and provides a client for the chat API.

- `Context` - The list of messages in the chat.
- `Functions` - The list of functions in the chat.
- `AppendSystemMessage` - Appends a system message to the chat context.
- `AppendFunction` - Appends a function to the chat context.
- `RemoveFunction` - Removes a function from the chat context.
- `ClearFunctions` - Clears all functions from the chat context.
- `SubmitAsync` - Submits a request model to the chat API and returns the response.
- `SubmitFunctionResponseAsync` - Submits a function response to the chat API and returns the response.

## Usage

1. Create an instance of `ChatClient` using the `Create` method.
2. Use the `AppendSystemMessage` method to add system messages.
3. Use the `AppendFunction` and `RemoveFunction` methods to add or remove functions.
4. Use the `SubmitAsync` method to send a message to the chat API and get the response.
5. Use the `SubmitFunctionResponseAsync` method to send a function response to the chat API and get the response.

Example usage:

```csharp
// Create a chat client
var client = ChatClient.Create("API_KEY");

// Append a system message
client.AppendSystemMessage("Welcome!");

// Submit a message to the chat API
var response = await client.SubmitAsync("Hello");

// Get the response text
if (response.IsSuccess())
{
    var text = response.Choices[0].Message.Content;
    Console.WriteLine(text);
}
```

Please refer to the individual class documentation for more information on the available methods and properties.