using Azure.Identity;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel.Primitives;

namespace reko_mini_project.Server.Configurations;

public static class ChatClientExtensions
{
    public const string DEFAULT_CHAT_CLIENT_SCOPE = "https://ai.azure.com/.default";
    public const string AI_MODEL_NAME = "OpenAI:ModelName";
    public const string AI_MODEL_ENDPOINT_CONFIG_KEY = "OpenAI:Endpoint";

    public static IServiceCollection SetupChatClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton((provider) =>
        {
            BearerTokenPolicy tokenPolicy = new(
                new DefaultAzureCredential(),
                DEFAULT_CHAT_CLIENT_SCOPE);

            string model = AI_MODEL_NAME;
            string? endpoint = configuration[AI_MODEL_ENDPOINT_CONFIG_KEY];

            if (endpoint == null)
            {
                // Absence of endpoint configuration should be handled gracefully 
                // as it might be intentional in certain environments,
                // (e.g., local development without AI resources deployed).
                Console.WriteLine($"Warning: {AI_MODEL_ENDPOINT_CONFIG_KEY} is not configured. ChatClient will not be initialized.");
                return null!;
            }

            // Disable the OPENAI001 warning for this specific instance, 
            // as we are intentionally using the ChatClient constructor that accepts an authentication policy.
#pragma warning disable OPENAI001
            return new ChatClient(
                model: model,
                authenticationPolicy: tokenPolicy,
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri(endpoint)
                });
#pragma warning restore OPENAI001
        });

        return services;
    }
}