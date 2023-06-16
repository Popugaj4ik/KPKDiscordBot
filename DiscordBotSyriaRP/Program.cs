using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBotSyriaRP.Configs;
using DiscordBotSyriaRP.EF;
using DiscordBotSyriaRP.Handlers;
using DiscordBotSyriaRP.Logger;
using DiscordBotSyriaRP.Modules;
using DiscordBotSyriaRP.Modules.ModalModules;
using DiscordBotSyriaRP.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    private string StartrupConfigFileName = "StartupConfig.json";
    private string DynamicConfigFileName = "DynamicConfig.json";
    private static ConfigWatcher<DynamicConfig> configWatcher = null;

    public static void Main(string[] args)
    {
        _ = new Program().PreStartAsync();
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);
    }

    public async Task PreStartAsync()
    {
        var startupCfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(StartrupConfigFileName)
            .Build();

        var dynamicCfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(DynamicConfigFileName)
            .Build();

        var StartupConfig = startupCfg.Get<StartupConfig>();
        var DynamicConfig = dynamicCfg.Get<DynamicConfig>();

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.DirectMessages,
                    UseInteractionSnowflakeDate = false,
                    AlwaysDownloadUsers = true,
                }))
                .AddSingleton(StartupConfig)
                .AddSingleton(DynamicConfig)
                .AddSingleton<ConfigWatcher<DynamicConfig>>()
                .AddSingleton(x => new CommandService())
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<Prefixhandler>()
                .AddSingleton<MenuHandler>()
                .AddSingleton<ModalHandler>()
                .AddScoped<KPKContext>()
                .AddScoped<MessageEncryptService>()
                .AddTransient<ILoger, FileLogger>()
                ;
            })
            .Build();

        try
        {
            await RunAsync(host);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static void OnExit(object sender, EventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        if (configWatcher != null)
        {
            configWatcher.Stop();
        }
    }

    private async Task RunAsync(IHost host)
    {
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        configWatcher = provider.GetRequiredService<ConfigWatcher<DynamicConfig>>();
        configWatcher.Start();

        var client = provider.GetRequiredService<DiscordSocketClient>();

        var AppConfig = provider.GetRequiredService<StartupConfig>();
        var pCommands = provider.GetRequiredService<Prefixhandler>();
        pCommands.AddModule<HelpModule>(provider);

        var menuCommands = provider.GetRequiredService<MenuHandler>();
        menuCommands.AddModule<ModalModule>(provider);
        menuCommands.AddModule<HandleModalCallbackButton>(provider);

        var modalCommands = provider.GetRequiredService<ModalHandler>();
        modalCommands.AddModule<ModalModule>(provider);
        menuCommands.AddModule<HandleModalCallbackButton>(provider);

        await pCommands.InitializeAsync();
        await menuCommands.InitializeAsync();
        await modalCommands.InitializeAsync();

        client.Log += _ => provider.GetRequiredService<ILoger>().Log(_);

        client.Ready += async () =>
        {
            provider.GetRequiredService<ILoger>().Log(new LogMessage(LogSeverity.Info, "Server", "Bot ready"));
        };

        client.ButtonExecuted += async (msg) =>
        {
            Console.WriteLine(msg.Id);
        };

        await client.LoginAsync(TokenType.Bot, AppConfig.BotToken);
        await client.StartAsync();

        Task.Delay(-1).Wait();
    }
}