# MafSubAgent

A university advisor chatbot built using Azure OpenAI and Microsoft Agents framework.

## Description

This agent acts as a friendly and helpful university advisor. It gives students clear, accurate, practical guidance on academic planning, course selection, university policies, deadlines, and student resources, keeping answers concise and easy to understand. When it is unsure about a university-specific policy or requirement, it says so and tells the student to verify with the university, rather than guessing.

## Configuration

Copy `.env.example` to `.env` and set the following variables (same order as `.env.example`):

- `XIANS_SERVER_URL` — URL of the Xians server (a hosted URL, or `http://localhost:5001` for local dev).
- `XIANS_API_KEY` — API key for the Xians platform.
- `AZURE_OPENAI_ENDPOINT` — endpoint URL of your Azure OpenAI resource.
- `AZURE_OPENAI_API_KEY` — API key for your Azure OpenAI resource.
- `AZURE_OPENAI_DEPLOYMENT` — name of the Azure OpenAI chat deployment to use (e.g. `gpt-5-mini-pay`).
- `DOTNET_ENVIRONMENT` — .NET runtime environment name (e.g. `Development`).
- `APP_VERSION` — application version string.

`XIANS_SERVER_URL`, `XIANS_API_KEY`, `AZURE_OPENAI_ENDPOINT`, `AZURE_OPENAI_API_KEY`, and `AZURE_OPENAI_DEPLOYMENT` are read at startup in `Program.cs` and are required — the app throws on launch if any of them is missing.

## Running

```
dotnet restore
dotnet run
```

On startup the app loads `.env`, initializes the Xians platform, registers the agent, and starts a supervisor workflow that forwards each incoming chat message to `MafSubAgent` and replies with its response.

## Public surface

- `MafSubAgent(string endpoint, string apiKey, string deploymentName)` — builds the agent against an Azure OpenAI chat deployment, configured with the university-advisor instructions above (`MafSubAgent.cs`).
- `MafSubAgent.RunAsync(string message) -> Task<string>` — sends `message` to the underlying chat agent and returns its response text.

## Doc gaps

- `DOTNET_ENVIRONMENT` and `APP_VERSION` are listed in `.env.example` but are not read via `Environment.GetEnvironmentVariable` (or any other visible mechanism) anywhere in this repo's source — likely consumed implicitly by the .NET runtime or the `Xians.Lib` package, but that could not be confirmed by reading this repo alone.
- `Program.cs` sets `IsTemplate = false` with a comment "See important notes below", but no such notes exist in this file or elsewhere in the repo — the intended caveat is unknown.
- The exact behavior/contract of `XiansPlatform.InitializeAsync`, `Agents.Register`, and `Workflows.DefineSupervisor()` comes from the external `Xians.Lib` package (v3.28.1) and was not verified by running the code.
- `MafSubAgent.cs`'s persona instructions have changed at least twice recently (this repo's history shows a summarization-assistant wording and, before that, other personas). This README describes the persona as of the commit it was written against; if the instructions change again, this section will need a follow-up update.
