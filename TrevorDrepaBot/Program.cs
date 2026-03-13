using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.EntityFrameworkCore;
using TrevorDrepaBot.Bots;
using TrevorDrepaBot.Conversation;
using TrevorDrepaBot.Data;
using TrevorDrepaBot.Repositories;
using TrevorDrepaBot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TrevorDbContext>(options =>
    options.UseSqlite("Data Source=trevor.db"));

builder.Services.AddScoped<ISessionRepository, SqliteSessionRepository>();
builder.Services.AddScoped<ISymptomRepository, SqliteSymptomRepository>();

builder.Services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
builder.Services.AddScoped<DrepaConversationEngine>();
builder.Services.AddScoped<IBot, DrepaBot>();

builder.Services.AddSingleton<IIntentService, OpenAIIntentService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TrevorDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();