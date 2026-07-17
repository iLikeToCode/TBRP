using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Services.ApplicationCommands;
using TBRP.DiscordBot.Structs;
using TBRP.ErlcAPI;

namespace TBRP.DiscordBot;

public class DiscordBotClient
{
    private readonly GatewayClient _client;
    private readonly ServiceProvider _provider;
    private readonly ApplicationCommandService<ApplicationCommandContext> _applicationCommandService;

    public DiscordBotClient(string token, string erlcApiKey)
    {
        var services = new ServiceCollection();

        services.AddSingleton(new ApiClient(erlcApiKey));

        services.AddSingleton(new ApplicationCommandService<ApplicationCommandContext>());

        services.AddSingleton<GatewayClient>(sp =>
            new GatewayClient(
                new BotToken(token),
                new GatewayClientConfiguration
                {
                    Logger = new ConsoleLogger(),
                    Intents = GatewayIntents.GuildMessages
                              | GatewayIntents.DirectMessages
                              | GatewayIntents.MessageContent
                }));

        foreach (var type in Assembly.GetExecutingAssembly()
                     .GetTypes()
                     .Where(t => typeof(IEventHandler).IsAssignableFrom(t)
                                 && t is { IsInterface: false, IsAbstract: false }))
        {
            services.AddTransient(type);
        }

        _provider = services.BuildServiceProvider();

        _client = _provider.GetRequiredService<GatewayClient>();
        
        _applicationCommandService = _provider.GetRequiredService<ApplicationCommandService<ApplicationCommandContext>>();
        
        _applicationCommandService.AddModules(
            Assembly.GetExecutingAssembly());
        
        
        foreach (var type in Assembly.GetExecutingAssembly()
                     .GetTypes()
                     .Where(t => typeof(IEventHandler).IsAssignableFrom(t)
                                 && t is { IsInterface: false, IsAbstract: false }))
        {
            var handler = (IEventHandler)_provider.GetRequiredService(type);
            handler.Register();
        }
    }

    public async Task Login()
    {
        await _applicationCommandService.RegisterCommandsAsync(_client!.Rest, _client.Id);
        await _client.StartAsync();
    }
}
