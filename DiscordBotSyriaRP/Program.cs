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
    private string ConfigFileName = "config.json";

    public static void Main(string[] args)
    {
        _ = new Program().PreStartAsync();
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);
    }

    public async Task PreStartAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(ConfigFileName)
            .Build();

        var AppConfig = config.GetSection("AppConfig").Get<Config>();

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.DirectMessages,
                    UseInteractionSnowflakeDate = false,
                    AlwaysDownloadUsers = true,
                }))
                .AddSingleton(AppConfig)
                .AddSingleton(x => new CommandService())
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<Prefixhandler>()
                .AddSingleton<MenuHandler>()
                .AddSingleton<ModalHandler>()
                .AddScoped<KPKContext>()
                .AddScoped<MessageEncryptService>()
                .AddTransient<ConsoleLoger>()
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
    }

    private async Task RunAsync(IHost host)
    {
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        var client = provider.GetRequiredService<DiscordSocketClient>();

        var AppConfig = provider.GetRequiredService<Config>();
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

        client.Log += _ => provider.GetRequiredService<ConsoleLoger>().Log(_);

        client.Ready += async () =>
        {
            provider.GetRequiredService<ConsoleLoger>().Log(new LogMessage(LogSeverity.Info, "Server", "Bot ready"));
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