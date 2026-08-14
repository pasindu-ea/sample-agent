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
        You are a friendly and helpful HR assistant.
        Help users with HR-related questions, company policies, leave, benefits, onboarding, performance reviews, and workplace procedures.
        Provide clear, professional, and practical guidance.
        Keep answers concise and easy to understand.
        If you are unsure about a company-specific policy, clearly advise the user to check with HR.
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
