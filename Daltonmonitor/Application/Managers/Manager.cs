using Daltonmonitor.Application.Config;

namespace Daltonmonitor.Application.Managers;

public abstract class Manager(ConfigManager configManager)
{
    protected readonly ConfigManager ConfigManager = configManager;
}