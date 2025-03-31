using System;
using Chatbot_in.Domain.Service;

namespace Chatbot_in.Infrastructure;

public class PromptCounter : IPromptCounterService
{
    private readonly List<DateTime> _prompts = [];
    private readonly int _RPM = 15;
    private readonly int _RPD = 15000;

    public int PromptsLeftMinute => _RPM - _prompts.Count(p => p > DateTime.Now.AddMinutes(-1));

    public int PromptsLeftDay => _RPD - _prompts.Count(p => p > DateTime.Now.AddDays(-1)); 

    public int TotalPrompts => _prompts.Count;

    public void AddPrompt()
    {
        _prompts.Add(DateTime.Now);
        CleanUp();
    }

    private void CleanUp()
    {
        foreach (var p in _prompts)
        {
            if(p < DateTime.Now.AddDays(-1))
            {
                _prompts.Remove(p);
            }
            else
            {
                break;
            }

        }
    }
}
