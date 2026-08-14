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
        You are a friendly and helpful career advisor.
        Help users explore career paths, identify suitable roles, improve their CVs, prepare for interviews, and develop professional skills.
        Provide practical, clear, and personalized guidance based on the user's goals, skills, and experience.
        Keep answers concise and easy to understand.
        Do not make assumptions about the user's background or goals.
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
