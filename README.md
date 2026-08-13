# Chat bot

A .NET console agent that runs on the Xians platform and forwards user chat
messages to a Microsoft Agent Framework (MAF) sub-agent backed by Azure
OpenAI. The sub-agent is instructed to act as a calculator assistant: it
performs mathematical calculations and returns the result, showing steps
when helpful.

## Configuration

Set these environment variables (see `.env.example`), loaded from a `.env`
file at startup via `DotNetEnv`:

- `XIANS_SERVER_URL` — URL of the Xians server to connect to (use the
  hosted server URL, or `http://localhost:5001` for a local server).
- `XIANS_API_KEY` — API key used to authenticate with the Xians server.
- `AZURE_OPENAI_ENDPOINT` — Endpoint URL of the Azure OpenAI resource.
- `AZURE_OPENAI_API_KEY` — API key for the Azure OpenAI resource.
- `AZURE_OPENAI_DEPLOYMENT` — Name of the Azure OpenAI chat deployment to use.
- `DOTNET_ENVIRONMENT` — .NET runtime environment name (e.g. `Development`).
- `APP_VERSION` — Application version identifier.

`XIANS_SERVER_URL`, `XIANS_API_KEY`, `AZURE_OPENAI_ENDPOINT`,
`AZURE_OPENAI_API_KEY`, and `AZURE_OPENAI_DEPLOYMENT` are required —
`Program.cs` throws an `InvalidOperationException` at startup if any of
them is missing.

## Running

1. Copy `.env.example` to `.env` and fill in the values above.
2. `dotnet run`

This starts the Xians agent ("My Agent"), registers a built-in
conversational (Supervisor) workflow, and begins handling incoming chat
messages.

## Public surface

- `MafSubAgent(string endpoint, string apiKey, string deploymentName)` —
  builds a MAF agent wired to the given Azure OpenAI deployment,
  instructed to act as a calculator assistant.
- `MafSubAgent.RunAsync(string message): Task<string>` — sends `message`
  to the underlying AI agent and returns its text response.

`Program.cs` wires a `MafSubAgent` into the Xians conversational
workflow's `OnUserChatMessage` handler, so every incoming chat message is
answered by the calculator sub-agent.
