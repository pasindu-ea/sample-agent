using DotNetEnv;
using Xians.Lib.Agents.Core;

// Load environment variables from .env file
Env.Load();

// Get configuration from environment variables
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") 
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found in environment variables");
var azureApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY not found in environment variables");
var azureDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") 
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT not found in environment variables");
var serverUrl = Environment.GetEnvironmentVariable("XIANS_SERVER_URL") 
    ?? throw new InvalidOperationException("XIANS_SERVER_URL not found in environment variables");
var xiansApiKey = Environment.GetEnvironmentVariable("XIANS_API_KEY") 
    ?? throw new InvalidOperationException("XIANS_API_KEY not found in environment variables");

// Initialize Xians Platform
var xiansPlatform = await XiansPlatform.InitializeAsync(new ()
{
    ServerUrl = serverUrl,
    ApiKey = xiansApiKey
});

// Register a new agent with Xians
var xiansAgent = xiansPlatform.Agents.Register(new ()
{
    Name = "My Agent",
    IsTemplate = false  // See important notes below
});

// Define a built-in conversational workflow. 
// `DefineSupervisor()` is a shortcut method to `DefineBuiltIn(name: "Supervisor Workflow")`
var conversationalWorkflow = xiansAgent.Workflows.DefineSupervisor();

// Create your MAF agent instance
var mafSubAgent = new MafSubAgent(azureEndpoint, azureApiKey, azureDeployment);

// Handle incoming user messages
conversationalWorkflow.OnUserChatMessage(async (context) =>
{
    var response = await mafSubAgent.RunAsync(context.Message.Text);
    await context.ReplyAsync(response);
});

// Start the agent and all workflows
await xiansAgent.RunAllAsync();