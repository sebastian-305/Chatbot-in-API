namespace Chatbot_in.Domain.Repositories;

public interface IPromptCountRepository
{
    int PromptsLeftMinute { get; }

    int PromptsLeftDay { get; }

    int TotalPrompts { get; }

    Task AddPrompt();
}
