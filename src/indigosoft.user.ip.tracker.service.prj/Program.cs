using Indigosoft.User.Ip.Tracker.Database;
using Indigosoft.User.Ip.Tracker.Database.Configuration;
using Indigosoft.User.Ip.Tracker.Database.Services;
using Indigosoft.User.Ip.Tracker.Infrastructure;
using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using Indigosoft.User.Ip.Tracker.Service.HealthApi;
using Indigosoft.User.Ip.Tracker.Service.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Indigosoft.User.Ip.Tracker.Service;

public class Program
{
    private static IConfiguration Configuration { get; set; }

	private static Serilog.ILogger Log { get; set; }

	public static void Main(string[] args)
	{
		ProfileLocationStorage.ProfileRootDir = PathConstants.AppProgramDataPath;

		if(args.Length != 0)
		{
			ProfileLocationStorage.ProfileRootDir = args[0];
		}

		Directory.CreateDirectory(ProfileLocationStorage.ConfigDirPath);

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			MissingMemberHandling = MissingMemberHandling.Error
		};

        Configuration = new ConfigurationBuilder()
			.SetBasePath(ProfileLocationStorage.ConfigDirPath)
			.AddJsonFile(ProfileLocationStorage.ConfigFileName, optional: true, reloadOnChange: false)
			.AddCommandLine(args)
			.Build();

        Serilog.Debugging.SelfLog.Enable(Console.Error);
        var dbConfig = Configuration.GetSection("PostgreSql").Get<DatabaseConfiguration>() ?? new DatabaseConfiguration();
        Serilog.Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.PostgreSQL(
                connectionString: dbConfig.PostgreSql.ToConnectionString(),
                tableName: "logs",
                needAutoCreateTable: false,
				useCopy: false,
                columnOptions: new Dictionary<string, ColumnWriterBase>
                {
                    { "message", new RenderedMessageColumnWriter() },
                    { "level", new LevelColumnWriter() },
                    { "timestamp", new TimestampColumnWriter() },
                    { "exception", new ExceptionColumnWriter() },
                    { "properties", new LogEventSerializedColumnWriter() }
                })
            .CreateLogger();

        Log = Serilog.Log.ForContext<Program>();

        try
		{
			Log.Information($"Using profile dir: {ProfileLocationStorage.ProfileRootDir:profileDir}");
			Log.Information("Starting web host");
			var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();
            AddServices(builder);

			var app = builder.Build();

			Configure(app);

			app.Run();
		}
		catch(Exception exc)
		{
			Log.Fatal(exc, "Host terminated unexpectedly");
		}
	}


	internal static void Configure(WebApplication app)
	{
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "IndigoSoft");
		});

		app.UseAuthorization();

		app.UseCors(builder => builder
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());

		app.UseRouting()
			.UseWebSockets()
			.UseExceptionHandler("/Error");

		app.Use(async (context, next) =>
		{
			context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
			{
				NoCache = true,
				NoStore = true,
			};

			await next();
		});

		app.MapHealthEndpoints()
			.MapIPConnectionEndpoints()
			.MapUsersEndpoints();
	}

	internal static void AddServices(WebApplicationBuilder builder)
	{
		builder.Services.AddAuthorization();

		if(File.Exists(ProfileLocationStorage.ConfigPath))
		{
			builder.Configuration.Sources.Clear();
			builder.Configuration.AddConfiguration(Configuration);
		}
		else
		{
			Log.Information($"Configuration file: {ProfileLocationStorage.ConfigPath:configPath} couldn't be found");
		}


        var dbConfig = Configuration.GetSection("PostgreSql").Get<DatabaseConfiguration>() ?? new DatabaseConfiguration();
        var eventConfig = Configuration.GetSection("EventProcessorSettings").Get<EventProcessorSettings>() ?? new EventProcessorSettings();
        builder.Services.AddSingleton(dbConfig);
        builder.Services.AddSingleton(eventConfig);
        builder.Services.AddSingleton<EventQueue<IPConnectionRequest>>();

        builder.Services
			.AddDbContext<ApplicationContext>(
				(s, o) => o.UseNpgsql(
					s.GetRequiredService<DatabaseConfiguration>().PostgreSql.ToConnectionString()))
			.AddSingleton<IApplicationDbProvider, ApplicationDbProvider>();

		builder.Services.AddOptions();
		builder.Services.AddMemoryCache();
		builder.Services.AddSingleton<IInitializable, DatabaseInitializer>();

		builder.Services.AddSingleton<IIPConnectionDatabaseService, IPConnectionDatabaseService>();
		builder.Services.AddSingleton<IUsersDatabaseService, UsersDatabaseService>();

        builder.Services.AddHostedService<InitializationService>();
        builder.Services.AddHostedService<EventProcessor>();

        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

        builder.Services
			.AddCors(opt => opt.AddDefaultPolicy(corsBuilder => corsBuilder
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowAnyOrigin()));

		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc(
				"v1",
				new OpenApiInfo
				{
					Version = "v1",
					Title = "IndigoSoft.Test",
					Description = "IndigoSoft.Test"
				});
		});

	}
}