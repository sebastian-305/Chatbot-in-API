using System;

namespace Chatbot_in.Application.Service;


//DEPRECATED
public interface IPromptCounterService
{
    void AddPrompt();

    int PromptsLeftMinute { get; }

    int PromptsLeftDay { get; }

    int TotalPrompts {  get; }
}
