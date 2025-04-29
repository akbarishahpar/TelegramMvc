<p align="center"><img src="https://raw.githubusercontent.com/akbarishahpar/TelegramMvc/refs/heads/main/Logo.png" /></p>



# TelegramMvc

**TelegramMvc** is a lightweight, modular framework for building **Telegram bots** in **.NET**, inspired by the **Model-View-Controller (MVC)** pattern.

It simplifies the development of Telegram bots by offering:

- 🚀 **Controller-based routing**: Organize your bot’s actions into clean, testable controllers and methods.
- 🖼️ **Views and templates**: Easily create dynamic, reusable message layouts.
- 📬 **Update handling middleware**: Automatically process incoming Telegram updates without boilerplate.
- ⚙️ **Built-in dependency injection**: Leverage .NET’s DI container for services like databases, APIs, or custom logic.
- 🌐 **Webhook-first design**: Perfect for modern serverless or hosted bot applications.



## Features

**Controller Actions**: Map commands, callbacks, and messages directly to methods.

**View Engine**: Generate dynamic responses using lightweight templates or plain text.

**Middleware Integration**: Easily add TelegramMvc into your ASP.NET Core app pipeline.

**Scoped Services**: Inject your own services into controllers.

**Page Navigation**: Build multi-step workflows and wizards naturally.

**Simple Startup**: Just add services and call `UseTelegram()`.



## Why TelegramMvc?

Instead of manually parsing updates and writing endless switch-cases, **TelegramMvc** lets you:

- Focus on **logic**, not plumbing
- **Scale** your bot code as it grows
- Keep everything **organized**, **testable**, and **separated** (Controllers vs Views vs Services)

Whether you're building a **small personal bot** or a **complex multi-page system** — **TelegramMvc** gets out of your way and lets you build faster.



# 📘 Getting Started with TelegramMvc

To get started with **TelegramMvc**, follow the steps below to set up a basic **ASP.NET MVC** application and integrate **TelegramMvc** for building your Telegram bot.



## 1. Create an ASP.NET MVC Application

Before you can integrate **TelegramMvc**, you first need to create an **ASP.NET MVC** application. Here's how:

### Steps:

1. **Open Visual Studio** and create a new project.
2. Select **ASP.NET Core Web App (Model-View-Controller)** from the templates.
3. Choose a project name (e.g., `MyTelegramBot`).
4. Select **.NET 8.0** or higher for the framework.
5. Click **Create** to generate the project.

This will give you the basic skeleton of an **ASP.NET MVC** application.



## 2. Add TelegramMvc Package

Next, you need to add the **TelegramMvc** package to your project.

### Steps:

1. Open **NuGet Package Manager** in Visual Studio (Tools → NuGet Package Manager → Manage NuGet Packages for Solution).
2. Search for `TelegramMvc`.
3. Click **Install** to add the package to your project.

Alternatively, you can install the package using **Package Manager Console**:

```powershell
Install-Package TelegramMvc
```



## 3. Add Configuration for Bot Token and Webhook

In the **`appsettings.json`** file, add your bot’s token and webhook URL.

### Example:

```json
{
  "BotSettings": {
      "Token": "YOUR_BOT_TOKEN",
      "WebhookEndpoint": "https://yourserver.com/",

      // optional: If you provide this field, your controllers and views will be organized under a specified area.
      "AreaName": "/bot",

      // optional: omit to disable rate limiting
      "RateLimitSettings": {
        "Message": "Don't rush honey!😅",
        "DelayBetweenRequestsInSeconds": 1
      },

      // optional: allows telegram bot client to connect telegram servers using http proxy
      "ProxySettings": {
          "Server": "localhost",
          "Port": 10808,
          "UserName": "admin",
          "Password": "admin"
      }
  }
}
```

Replace `YOUR_BOT_TOKEN` with the bot token you get from **@BotFather** on Telegram, and the `WebhookUrl` should be a public `https` endpoint where Telegram will send updates to your bot.



The **AreaName** setting lets you isolate all of your Telegram bot’s controllers and views under a specific URL prefix (for example, “/bot”), keeping them completely separate from the rest of your MVC application. By using an `AreaName`, you can develop and host both traditional web pages—such as control panels, dashboards, or any browser-rendered features—and your Telegram bot logic side by side within the same project, without route conflicts. This clear boundary makes it easy to integrate bot functionality into an existing website or add web-based tools alongside your bot, while keeping your code organized and concerns neatly separated.



The **RateLimitSettings** section lets you protect your bot from message floods by enforcing a minimum delay between user requests. When configured, any chat that sends commands faster than the specified `DelayBetweenRequestsInSeconds` will receive the custom `Message` (e.g., “Don’t rush honey!😅`). Simply omit this section to disable rate limiting entirely. This helps ensure your bot stays responsive and within Telegram’s API rate limits, while giving users a friendly reminder if they’re hammering its endpoints.



The **ProxySettings** section allows your Telegram bot to connect to Telegram’s servers through an HTTP proxy, which can be useful when hosting the bot in regions where access to Telegram servers is restricted, such as certain data centers or countries with network censorship. By configuring the proxy server’s address, port, and (optionally) authentication credentials, you ensure your bot can bypass local network restrictions and securely communicate with Telegram’s API.



## 4. Registering TelegramMvc Services

There are two ways to register **TelegramMvc** services:

1. **Using Configuration Binding**
    If you have your bot settings defined in `appsettings.json`, you can load them automatically:

   ```c#
   builder.Services.AddTelegramMvc(builder.Configuration, nameof(BotSettings));
   ```

2. **Providing Settings Dynamically**
    You can also configure the bot manually by supplying the settings in code:

   ```c#
   builder.Services.AddTelegramMvc(options =>
   {
       options.BotToken           = botConfig.Token;
       options.WebhookUrl         = botConfig.WebhookEndpoint;
       options.AreaName           = botConfig.AreaName;            // optional
       options.RateLimitSettings  = botConfig.RateLimitSettings;   // optional
       options.ProxySettings      = botConfig.ProxySettings;       // optional
   });
   ```



### Important: Register `IChatAccessor`

**TelegramMvc** requires an implementation of `IChatAccessor` to be registered as a **Scoped** service.
 This service is responsible for storing chat sessions and their states across updates.

- A simple, built-in implementation is provided: **`InMemoryChatsRepository`**.
   It stores session data in memory and is ideal for quick prototyping or testing.
- **⚠️ For production use**, it is highly recommended that you create your own `IChatAccessor` implementation, using a persistent storage like a database to avoid losing session data when the application restarts.

Example of registering the built-in memory repository:

```c#
builder.Services.AddScoped<IChatAccessor, InMemoryChatsRepository>();
```



### (Optional) Registering `IMessageWriter`

`IMessageWriter` is an optional service that can be registered as **Scoped**.
 It is used to capture and handle communications between your bot and Telegram servers, exposing both **incoming** and **outgoing** messages.

This can be particularly useful if you want to **log** or **store messages** for monitoring, debugging, or auditing purposes.

A built-in implementation called **`ConsoleMessageWriter`** is provided, which simply writes all messages to the application console.

The method signature invoked when a message is sent or received is:

```c#
Task WriteAsync(long chatId, string message, MessageDirection direction, CancellationToken cancellationToken);
```

- `chatId` — the ID of the Telegram chat.
- `message` — the content of the message.
- `direction` — indicates whether the message is **incoming** or **outgoing**.
- `cancellationToken` — used to handle task cancellation gracefully.

If you want to implement your own logging mechanism (e.g., database logging, file logging), you can create a custom implementation of `IMessageWriter`.



## 5. Configuring HTTP Pipeline

To activate **TelegramMvc** and allow your application to handle Telegram webhook updates, you need to call:

```c#
app.UseTelegramMvc();
```

This middleware must be registered **early** in the pipeline — typically **after** exception handling (`UseExceptionHandler`) and **before** static files and routing.
 It ensures that incoming Telegram requests are properly captured and routed into your MVC controllers.

Your middleware setup should look like this:

```c#
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseTelegramMvc(); // Activate TelegramMvc middleware

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
```

> **Note:**
>
> - `UseTelegramMvc()` should come **before** `UseRouting()`.
> - Always ensure your bot's area route (like `/bot`) is correctly mapped using `MapControllerRoute`.

Without calling `UseTelegramMvc`, **TelegramMvc** won't intercept the webhook requests and no updates will reach your controllers.



## 6. Chat State Management in TelegramMvc

In **TelegramMvc**, each user's chat state is determined by the **last path** they visited. This path corresponds to the action and controller that was triggered during their previous interaction. This state will change whenever:

- The user sends a message starting with a **forward slash ('/')** (commonly used for commands).
- The user interacts with an **inline keyboard**.

When a new request is received from Telegram, the system checks the user's **chat state**, which is the last HTTP path they visited. It then routes the request to the corresponding controller action to handle the request.



### How It Works

1. **State Determination**:
    **TelegramMvc** tracks the current state of each chat, which is essentially the last HTTP endpoint (path) the user visited. For example, if the user last visited `/bot/start`, the state for their chat is considered to be `/bot/start`.
2. **Path-Based State Management**:
    Each chat session is mapped to a specific controller action based on the URL path. When the user sends a new request (message or command), **TelegramMvc** will look up the **path** that corresponds to the user's chat state and execute the associated action.
3. **Changing State**:
    The state is changed when:
   - A message starting with `/` is received (typically a command like `/help` or `/settings`).
   - An inline keyboard button is clicked, which contains an action leading to a new path.



### Developer Responsibility

As a developer, **you need to create a new controller action for each path** or user state you want to handle. Here's how you might structure this:

- **Define a path for each state**: For example, `/start`, `/help`, `/settings`, and so on.
- **Create a controller action for each state**: Ensure each path corresponds to an action that will handle the respective interaction.

Example:

```c#
public class StartController : BotController
{
    // The user is in the '/start' state
    public IActionResult Index()
    {        
        return TelegramMessage();
    }
}

public class HelpController : BotController
{
    // The user is in the '/help' state
    public IActionResult Index()
    {
        return TelegramMessage();
    }
}

public class SettingsController : BotController
{
    public IActionResult Index()
    {
        return TelegramMessage();
    }
}
```

In the above example:

- `/start`, `/help`, and `/settings` are distinct states, each with its own controller and action.



### Key Points to Remember

- **Chat State**: The last HTTP path visited by the user determines the chat state.
- **State Transitions**: Inline keyboard commands or messages with `/` will transition the state to a new path.
- **Controller Actions**: For each path you want to manage, create a corresponding controller action. Ensure paths are mapped to user flows, like commands and settings.

This allows you to effectively manage user interactions and ensure that every chat has a clear progression of states, making your Telegram bot dynamic and organized.



## 7. Returning Different Types of Responses in TelegramMvc

After executing the necessary **business logic** in your action, you can return different types of **Telegram responses** depending on the content you wish to send to the user. **TelegramMvc** provides several ways to send a response, each designed for different types of media or actions.

Here are the available return types:



#### 1. **TelegramMessage**

Use this when you want to send a **text message** to the user. It's the most common response.

```c#
return TelegramMessage();
```

------

#### 2. **TelegramPhoto**

If you want to send a **photo** (image), use `TelegramPhoto`. This can either be a URL or a file ID.

```c#
return TelegramPhoto(InputFile.FromString("https://example.com/path/to/photo.jpg"));
```

You can also send a file directly from the server:

```c#
return TelegramPhoto(InputFile.FromStream(new FileStream("path/to/photo.jpg", FileMode.Open)));
```

------

#### 3. **TelegramVideo**

Use `TelegramVideo` to send a **video**. This can also be a URL or a file ID.

```c#
return TelegramVideo(InputFile.FromString("https://example.com/path/to/video.mp4"));
```

Alternatively, you can send a file directly:

```c#
return TelegramVideo(InputFile.FromStream(new FileStream("path/to/video.mp4", FileMode.Open)));
```

------

#### 4. **TelegramVoice**

For sending a **voice message**, use `TelegramVoice`. The voice message must be in the `.ogg` format or you may use a file ID.

```c#
return TelegramVoice(InputFile.FromString("https://example.com/path/to/voice.ogg"));
```

------

#### 5. **TelegramDocument**

Use `TelegramDocument` to send a **file** (not specifically an image, video, or audio file).

```c#
return TelegramDocument(InputFile.FromString("https://example.com/path/to/document.pdf"));
```

Or a local file:

```c#
return TelegramDocument(InputFile.FromStream(new FileStream("path/to/document.pdf", FileMode.Open)));
```

------

#### 6. **TelegramVoid**

`TelegramVoid` is useful when you don't need to send any message or media but want to signal that the action has been completed successfully (for example, acknowledging a command or performing a background task).

```c#
return TelegramVoid();
```

------

#### 7. **TelegramRedirect**

If you want to **redirect** the user to another path (changing their state), use `TelegramRedirect`. This will take the user to a new controller action.

```c#
return TelegramRedirect("/settings");
```



### Key Points:

- **TelegramMessage**: Send text messages.
- **TelegramPhoto**, **TelegramVideo**, **TelegramVoice**, **TelegramDocument**: Send media like images, videos, voice notes, or documents.
- **TelegramVoid**: Used when no content is needed but the operation is completed successfully.
- **TelegramRedirect**: Redirect users to another state or controller action.

These return types allow your Telegram bot to provide a rich, interactive experience for users, combining text, media, and complex state transitions.



## 8. Adding Keyboards to Your TelegramView

Any `TelegramView` (e.g. `TelegramMessage`, `TelegramPhoto`, etc.) can be enhanced with a keyboard by chaining the `.AddKeyboard(...)` method. There are two categories of keyboards you can attach:

------

#### 1. Reply Keyboards (Custom keyboards)

**Single row**

```c#
public TelegramView AddKeyboard(params KeyboardButton[] keyboardButtons)
```

```c#
return TelegramMessage("Choose an option")
    .AddKeyboard(
        new KeyboardButton("Option 1"),
        new KeyboardButton("Option 2")
    );
```

**Multiple rows**

```c#
public TelegramView AddKeyboard(KeyboardButton[][] keyboardButtons)
```

```c#
return TelegramMessage("Choose an option")
    .AddKeyboard(
        new[] { new KeyboardButton("A"), new KeyboardButton("B") },
        new[] { new KeyboardButton("C"), new KeyboardButton("D") }
    );
```

*All reply keyboards default to `OneTimeKeyboard = false`, `ResizeKeyboard = true`, and `Selective = true`.*

------

#### 2. Inline Keyboards

**Single row**

```c#
public TelegramView AddKeyboard(params InlineKeyboardButton[] keyboardButtons)
```

```c#
return TelegramMessage("Pick an action")
    .AddKeyboard(
        InlineKeyboardButton.WithCallbackData("Start", "start_path"),
        InlineKeyboardButton.WithCallbackData("Help", "help_path")
    );
```

**Multiple rows**

```c#
public TelegramView AddKeyboard(InlineKeyboardButton[][] keyboardButtons)
```

```c#
return TelegramMessage("Navigate:")
    .AddKeyboard(
        new[]
        {
            InlineKeyboardButton.WithCallbackData("View Tickets", "tickets/index"),
            InlineKeyboardButton.WithCallbackData("New Ticket", "tickets/create")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Settings", "settings/index")
        }
    );
```

---

### Important: Preferring Inline Keyboards over Reply Keyboards

Inline keyboards shine because they let you **decouple** what the user sees from the path your bot takes:

- **Display text** (`“View Tickets”`) can be human-friendly, localized, or emoji-rich.
- **Callback data** (`"tickets/index"`) is your internal route or state path.

When a button is clicked, the `callbackData` becomes the new chat state (an HTTP path), and **TelegramMvc** dispatches it to the matching controller action—without ever exposing that technical path on the button itself.

```c#
return TelegramMessage("What would you like to do?")
    .AddKeyboard(
        InlineKeyboardButton.WithCallbackData("📋 See Tickets", "/tickets/index"),
        InlineKeyboardButton.WithCallbackData("➕ Create Ticket", "/tickets/create")
    );
```

Here, users click friendly labels, but your bot routes to `/tickets/index` and `/tickets/create` under the hood—keeping your UI clean and your logic organized.



## 9. TelegramView and View.cshtml in TelegramMvc

In **TelegramMvc**, every `TelegramView` you return from a controller action is **mapped to a corresponding `.cshtml` view file**, very similar to how traditional MVC handles web pages.

Each `TelegramView` (like `TelegramMessage`, `TelegramPhoto`, `TelegramVideo`, etc.) expects a Razor view file located inside the Views folder structure — organized based on **Area**, **Controller**, and **Action** names.
 When a `TelegramView` is returned, **the framework automatically locates and compiles** the matching `.cshtml` view using the powerful **Razor** engine.

------

### Key Features:

- **Razor Syntax**:
   Views are written using **standard Razor syntax**, allowing clean mixing of C# logic with text templates. You can easily generate dynamic messages based on model data.

- **Strongly Typed Models**:
   You can bind a specific **model** to a view. Just like normal MVC views, you declare a model at the top of the `.cshtml`:

  ```c#
  @model MyApp.Models.OrderSummary
  ```

  Then you can render dynamic content based on it:

  ```c#
  Hello @Model.CustomerName, your order number @Model.OrderId has been confirmed!
  ```

- **ViewBag Access**:
   If you prefer a more dynamic or flexible approach (without strictly typed models), you can use `ViewBag` to pass arbitrary data:

  ```c#
  ViewBag.Greeting = "Welcome back!";
  ```

  Then in the view:

  ```c#
  @ViewBag.Greeting
  ```

- **Separation of Logic and Presentation**:
   Business logic stays inside the controller; the **view only focuses on formatting the message** to be sent to the user. This keeps your bot's codebase **clean**, **testable**, and **maintainable**.

- **Lightweight and Fast**:
   Razor views for Telegram are extremely lightweight — usually generating only a few lines of text or markup to be sent via Telegram APIs. Compilation is fast, and caching is supported automatically.

------

### Example Flow:

1. **Controller returns**:

   ```c#
   return TelegramMessage("OrderConfirmation", model);
   ```

2. **TelegramMvc looks for**:
    `Views/{Area}/{Controller}/OrderConfirmation.cshtml`

3. **The Razor view is compiled** using the provided `model` or `ViewBag`.

4. **Resulting text** is sent back as a **Telegram message** to the user.



## 10. Accessible Variables inside TelegramMvc Actions

Inside every controller action that is executed in **TelegramMvc**, developers have direct access to three important variables:

1. **Update**:
    This object represents the raw **Telegram request** received from the Telegram server.
    You can use `Update` to access information such as:

   - `Update.Message.Chat.Id` — the **Chat ID** of the user.
   - `Update.Message.Text` — the **text** message sent by the user.
   - `Update.CallbackQuery.Data` — the **data** sent by an inline button.
   - and many other fields like user info, attachments, etc.

2. **TelegramBotClient**:
    This is a **fully configured client instance** of the official **Telegram Bot API**.
    It allows you to **send independent messages**, **edit messages**, **answer callback queries**, and interact with the Telegram server manually — outside of the normal framework flow. For example, you could send a direct message to another user anytime:

   ```c#
   await TelegramBotClient.SendTextMessageAsync(chatId, "Hello from outside!");
   ```

3. **BotSettings**:
    This holds the bot's **configuration** and **settings** (such as the BotToken, Webhook URL, Proxy Settings, etc.).
    You can access extra settings or dynamic runtime configurations easily if needed inside your actions.

------

### Why is this Important?

- **Update** is essential for **contextual handling** of user input.
- **TelegramBotClient** provides **full control** when you want to perform custom actions not automatically handled by the framework.
- **BotSettings** helps when you want your logic to adapt based on bot configuration without hardcoding values.

This setup gives developers **maximum flexibility** while still keeping the framework **lightweight and structured**.



## 11. Handling Long CallbackData with Encoder

Telegram imposes a **maximum length limit** on the `CallbackData` field when creating **InlineKeyboardButtons**.
 If the full path you want to assign exceeds this limit, Telegram will reject the button or truncate the data, leading to broken navigation.

To **bypass this restriction**, **TelegramMvc** provides a special service called **`Encoder`**.

- **Encoder** temporarily **stores the full path** (or any long data) **in memory**, and
- **returns a short unique key** that can safely fit into the Telegram `CallbackData` field.
- Later, when a user clicks the button, **TelegramMvc automatically decodes** the key back to the full original path behind the scenes.

------

### How to Use Encoder

When creating inline keyboards, developers should:

1. **Inject** `Encoder` into the controller constructor:

   ```c#
   public StartController(Encoder encoder)
   {
       _encoder = encoder;
   }
   ```

2. **Use** `Encode.Push(string path)` to generate the shortened key:

   ```c#
   var button = InlineKeyboardButton.WithCallbackData("Next Step", _encoder.Push("/Start/NextStep"));
   ```

This ensures that the user is redirected properly without worrying about Telegram’s length limitations.

------

### Notes

- `Encoder` is automatically **registered** when you call `AddTelegramMvc()`, so you don't need any extra setup.
- Memory storage is **temporary** — it is tied to the application lifetime. For extremely long-running bots or bots with persistence requirements, you may extend the Encoder to use persistent storage if needed.
- Using `Encoder` is **highly recommended** anytime you are forwarding users to **dynamic** or **deep paths** with **Inline Keyboards**.