using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

public class MafSubAgent
{
    private readonly AIAgent _agent;

    public MafSubAgent(string endpoint, string apiKey, string deploymentName)
    {
        _agent = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsAIAgent(new ChatClientAgentOptions
            {
                Name = "MafSubAgent",
                ChatOptions = new ChatOptions
                {
                    Instructions = "You are a friendly assistant. Keep your answers brief."
                }
            });
    }

    public async Task<string> RunAsync(string message)
    {
        var response = await _agent.RunAsync(message);
        return response.Text;
    }
}