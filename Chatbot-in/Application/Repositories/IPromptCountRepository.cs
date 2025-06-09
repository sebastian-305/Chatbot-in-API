namespace Chatbot_in.Application.Repositories;

public interface IPromptCountRepository
{
    Task<int> GetPromptsLeftMinuteAsync();

    Task<int> GetPromptsLeftDayAsync();

    Task<int> GetTotalPromptsAsync();

    Task AddPrompt();
}
