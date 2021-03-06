using Deposit.Application;
using Deposit.Application.GraphTypes;
using Deposit.Infrastructure;
using Deposit.Infrastructure.GraphQl;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(4004);
});

// GraphQL
builder.Services
    .AddGraphQL(opts =>
    {
        opts.UnhandledExceptionDelegate = context =>
        {
            Console.WriteLine(context.Exception.Message);
            Console.WriteLine(context.Exception.StackTrace);
        };
        opts.EnableMetrics = true;
    })
    .AddNewtonsoftJson()
    .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
    .AddDataLoader()
    .AddGraphTypes(typeof(DepositSchema))
    .AddFederation(new[]
    {
        typeof(DepositGraphType)
    });

builder.Services.AddSingleton<DepositSchema>();
builder.Services.AddSingleton<IDbContext, DemoDbContext>();

builder.Services.Configure<KestrelServerOptions>(opts => opts.AllowSynchronousIO = true);
builder.Services.Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true);

var app = builder.Build();

app.UseGraphQL<DepositSchema>();
app.UseGraphQLPlayground(options: new PlaygroundOptions());

app.Run();