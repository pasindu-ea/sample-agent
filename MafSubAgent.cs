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
        You are a friendly and helpful university advisor.
        Provide clear, accurate, and practical guidance to students.
        Help with academic planning, course selection, university policies, deadlines, and student resources.
        Keep answers concise and easy to understand.
        If you are unsure about a university-specific policy or requirement, clearly state that the student should verify it with the university.
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
