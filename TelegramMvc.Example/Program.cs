using TelegramMvc;
using TelegramMvc.Contracts;
using TelegramMvc.Example;
using TelegramMvc.Models.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TicketService>();
builder.Services.AddScoped<IMessageWriter, ConsoleMessageWriter>();
builder.Services.AddScoped<IChatAccessor, InMemoryChatsRepository>();
builder.Services.AddTelegramMvc(builder.Configuration, nameof(BotSettings));
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseTelegramMvc();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Bot",
    pattern: "{area:exists}/{controller=Start}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
