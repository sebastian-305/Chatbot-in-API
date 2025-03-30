using System;

namespace Chatbot_in.Domain.Service;

public interface IPromptCounterService
{
    void AddPrompt();

    int PromptsLeftMinute { get; }

    int PromptsLeftDay { get; }

    int TotalPrompts {  get; }
}
