using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using TBRP.DiscordBot.Structs;
using TBRP.Api;
using TBRP.DiscordBot.Jobs;

namespace TBRP.DiscordBot;

public partial class DiscordBotClient
{
    private readonly GatewayClient _client;
    private readonly ServiceProvider _provider;
    private readonly ApplicationCommandService<ApplicationCommandContext> _applicationCommandService;
    private readonly ComponentInteractionService<ButtonInteractionContext> _buttonService;
    private readonly DiscordBotJobRunner _jobRunner;
    private readonly CancellationTokenSource _jobCancellationTokenSource = new();

    public DiscordBotClient(string token, string erlcApiKey)
    {
        var services = new ServiceCollection();

        services.AddSingleton(new ApiClient(erlcApiKey));

        services.AddSingleton(new ApplicationCommandService<ApplicationCommandContext>());
        services.AddSingleton(new ComponentInteractionService<ButtonInteractionContext>());

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

        foreach (var type in Assembly.GetExecutingAssembly()
                     .GetTypes()
                     .Where(t => typeof(IDiscordBotJob).IsAssignableFrom(t)
                                 && t is { IsInterface: false, IsAbstract: false }))
        {
            services.AddSingleton(typeof(IDiscordBotJob), type);
        }

        services.AddSingleton<DiscordBotJobRunner>();

        _provider = services.BuildServiceProvider();

        _client = _provider.GetRequiredService<GatewayClient>();
        _jobRunner = _provider.GetRequiredService<DiscordBotJobRunner>();
        
        _applicationCommandService = _provider.GetRequiredService<ApplicationCommandService<ApplicationCommandContext>>();
        
        _applicationCommandService.AddModules(
            Assembly.GetExecutingAssembly());
        
        _buttonService = _provider.GetRequiredService<ComponentInteractionService<ButtonInteractionContext>>();
        
        _buttonService.AddModules(
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
        await Task.Delay(3000);
        _jobRunner.StartAll(_jobCancellationTokenSource.Token);
    }
}
