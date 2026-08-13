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
                     Instructions = """
        You are a calculator assistant.
        Perform mathematical calculations accurately.
        Return the final answer clearly and concisely.
        Show the calculation steps when helpful.
        Do not add unnecessary explanations or unrelated information.
        """
                }
            });
    }

    public async Task<string> RunAsync(string message)
    {
        var response = await _agent.RunAsync(message);
        return response.Text;
    }
}
