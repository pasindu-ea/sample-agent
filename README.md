# MafSubAgent

A friendly chatbot assistant built using Azure OpenAI and Microsoft Agents framework.

## Description

This agent provides a conversational interface with brief, helpful responses. It's configured as a general-purpose friendly assistant.

Under the hood, `MafSubAgent` (`MafSubAgent.cs`) wraps an Azure OpenAI chat deployment as a Microsoft Agents Framework `AIAgent`. `Program.cs` is the entry point: it loads configuration from environment variables, connects to the Xians platform, registers this app as a Xians agent, and defines a built-in conversational workflow that forwards each incoming user chat message to `MafSubAgent` and replies with its response.

## Configuration

Copy `.env.example` to `.env` and set the following environment variables (in the order they appear there):

- `XIANS_SERVER_URL` - URL of the Xians server to connect to (a hosted server URL, or `http://localhost:5001` for a local server). Required.
- `XIANS_API_KEY` - API key used to authenticate with the Xians server. Required.
- `AZURE_OPENAI_ENDPOINT` - Endpoint URL of your Azure OpenAI resource. Required.
- `AZURE_OPENAI_API_KEY` - API key for your Azure OpenAI resource. Required.
- `AZURE_OPENAI_DEPLOYMENT` - Name of the Azure OpenAI chat deployment to use. Required.
- `DOTNET_ENVIRONMENT` - .NET environment name (e.g. `Development`). Not read by this repo's own code; see Doc gaps below.
- `APP_VERSION` - Version string for the running app. Not read by this repo's own code; see Doc gaps below.

The first five are read directly in `Program.cs` and throw an `InvalidOperationException` on startup if missing.

## Running

```
dotnet run
```

`Program.cs` loads `.env` via `DotNetEnv`, initializes the Xians platform, registers the agent, and starts the conversational workflow, which stays running to handle incoming chat messages.

## Public surface

### `MafSubAgent` (`MafSubAgent.cs`)

- `MafSubAgent(string endpoint, string apiKey, string deploymentName)` - builds an Azure OpenAI-backed chat agent, instructed (via its `ChatOptions.Instructions`) to act as a summarization assistant that summarizes provided content clearly and concisely, using only information present in the original content.
- `Task<string> RunAsync(string message)` - sends `message` to the underlying agent and returns its text response.

## Doc gaps

- `DOTNET_ENVIRONMENT` and `APP_VERSION` are listed in `.env.example` but a repo-wide search found no code in this repo that reads either — their actual effect, if any (e.g. via the `Xians.Lib` package or external tooling), is unconfirmed without running the packaged dependency's internals.
- `Program.cs` registers the agent with `IsTemplate = false // See important notes below`, but no such notes exist anywhere in this repo. Left undocumented rather than guessed — needs a decision from whoever wrote that comment.
- The top-level description above calls this a "friendly chatbot assistant", but the actual instructions given to the model in `MafSubAgent.cs` describe a "summarization assistant" focused on summarizing content. This looks like a possible mismatch between the intended persona and the configured one; left as-is since resolving it is a product decision, not a doc fix.
- Behavior of `XiansPlatform.InitializeAsync`, `Agents.Register`, and `Workflows.DefineSupervisor()` lives in the external `Xians.Lib` package, not this repo — documented here only at the call-site usage level.

## Metric

No `Scripts/doc-coverage.sh` in this repo — computed by hand.

Public surface: 10 (5 required env vars + 2 optional/unconfirmed env vars + 1 class + 1 constructor + 1 method)
Documented: before 0 -> after 10
COVERAGE: 0% -> 100%
