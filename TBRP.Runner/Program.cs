using TBRP.DiscordBot;

DotNetEnv.Env.Load();

var discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
                   ?? throw new InvalidOperationException("Missing DISCORD_TOKEN");

var erlcApiKey = Environment.GetEnvironmentVariable("ERLC_API_KEY")
                 ?? throw new InvalidOperationException("Missing ERLC_API_KEY");

var bot = new DiscordBotClient(discordToken, erlcApiKey);

await bot.Login();

await Task.Delay(-1);