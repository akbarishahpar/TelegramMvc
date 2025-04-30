using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramMvc.Extensions;
using TelegramMvc.Filters;
using TelegramMvc.Models.Settings;
using TelegramMvc.Models.Views;

namespace TelegramMvc.Controllers
{
    [TelegramResultFilter]
    public abstract class TelegramController : Controller
    {
        protected TelegramBotClient TelegramBotClient = null!;
        protected BotSettings BotSettings = null!;
        protected Update Update = null!;

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            BotSettings = HttpContext.RequestServices.GetRequiredService<BotSettings>();
            TelegramBotClient = new TelegramBotClient(BotSettings.Token);
            Update = await context.HttpContext.Request.ToTelegramUpdateAsync(context.HttpContext.RequestAborted);

            base.OnActionExecuting(context);
        }

        #region Message

        public TelegramView TelegramMessage()
            => TelegramView(new TelegramMessage(View().ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramMessage(object model)
            => TelegramView(new TelegramMessage(View(model).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramMessage(string viewName)
            => TelegramView(new TelegramMessage(View(viewName).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramMessage(string viewName, object model)
            => TelegramView(new TelegramMessage(View(viewName, model).ToHtml(HttpContext, ControllerContext)));

        #endregion

        #region Photo

        public TelegramView TelegramPhoto(InputFile inputFile)
            => TelegramView(new TelegramPhoto(inputFile));

        public TelegramView TelegramPhotoWithView(InputFile inputFile)
            => TelegramView(new TelegramPhoto(inputFile, View().ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramPhotoWithView(InputFile inputFile, object model)
            => TelegramView(new TelegramPhoto(inputFile, View(model).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramPhotoWithView(InputFile inputFile, string viewName)
            => TelegramView(new TelegramPhoto(inputFile, View(viewName).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramPhotoWithView(InputFile inputFile, string viewName, object model)
            => TelegramView(new TelegramPhoto(inputFile, View(viewName, model).ToHtml(HttpContext, ControllerContext)));

        #endregion

        #region Audio

        public TelegramView TelegramAudio(InputFile inputFile)
            => TelegramView(new TelegramAudio(inputFile));

        public TelegramView TelegramAudioWithView(InputFile inputFile)
            => TelegramView(new TelegramAudio(inputFile, View().ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramAudioWithView(InputFile inputFile, object model)
            => TelegramView(new TelegramAudio(inputFile, View(model).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramAudioWithView(InputFile inputFile, string viewName)
            => TelegramView(new TelegramAudio(inputFile, View(viewName).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramAudioWithView(InputFile inputFile, string viewName, object model)
            => TelegramView(new TelegramAudio(inputFile, View(viewName, model).ToHtml(HttpContext, ControllerContext)));

        #endregion

        #region Voice

        public TelegramView TelegramVoice(InputFile inputFile)
            => TelegramView(new TelegramVoice(inputFile));

        public TelegramView TelegramVoiceWithView(InputFile inputFile)
            => TelegramView(new TelegramVoice(inputFile, View().ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramVoiceWithView(InputFile inputFile, object model)
            => TelegramView(new TelegramVoice(inputFile, View(model).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramVoiceWithView(InputFile inputFile, string viewName)
            => TelegramView(new TelegramVoice(inputFile, View(viewName).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramVoiceWithView(InputFile inputFile, string viewName, object model)
            => TelegramView(new TelegramVoice(inputFile, View(viewName, model).ToHtml(HttpContext, ControllerContext)));

        #endregion

        #region Video

        public TelegramView TelegramVideo(InputFile inputFile)
            => TelegramView(new TelegramVideo(inputFile));

        public TelegramView TelegramVideoWithView(InputFile inputFile)
            => TelegramView(new TelegramVideo(inputFile, View().ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramVideoWithView(InputFile inputFile, object model)
            => TelegramView(new TelegramVideo(inputFile, View(model).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramVideoWithView(InputFile inputFile, string viewName)
            => TelegramView(new TelegramVideo(inputFile, View(viewName).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramVideoWithView(InputFile inputFile, string viewName, object model)
            => TelegramView(new TelegramVideo(inputFile, View(viewName, model).ToHtml(HttpContext, ControllerContext)));

        #endregion

        #region Document

        public TelegramView TelegramDocument(InputFile inputFile)
            => TelegramView(new TelegramDocument(inputFile));

        public TelegramView TelegramDocumentWithView(InputFile inputFile)
            => TelegramView(new TelegramDocument(inputFile, View().ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramDocumentWithView(InputFile inputFile, object model)
            => TelegramView(new TelegramDocument(inputFile, View(model).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramDocumentWithView(InputFile inputFile, string viewName)
            => TelegramView(new TelegramDocument(inputFile, View(viewName).ToHtml(HttpContext, ControllerContext)));

        public TelegramView TelegramDocumentWithView(InputFile inputFile, string viewName, object model)
            => TelegramView(new TelegramDocument(inputFile,
                View(viewName, model).ToHtml(HttpContext, ControllerContext)));

        #endregion

        #region Redirect

        public TelegramView TelegramRedirect(string path)
            => TelegramView(new TelegramRedirect(path));

        public TelegramView TelegramRedirectToAction(string action)
            => TelegramView(new TelegramRedirect(Url.Action(action).EmptyIfNull()));

        public TelegramView TelegramRedirectToAction(string action, object values)
            => TelegramView(new TelegramRedirect(Url.Action(action, values).EmptyIfNull()));

        public TelegramView TelegramRedirectToAction(string action, string controller)
            => TelegramView(new TelegramRedirect(Url.Action(action, controller).EmptyIfNull()));

        public TelegramView TelegramRedirectToAction(string action, string controller, object values)
            => TelegramView(new TelegramRedirect(Url.Action(action, controller, values).EmptyIfNull()));

        #endregion

        public TelegramView TelegramVoid(bool tryDeleteHistory = true)
            => TelegramView(new TelegramVoid(tryDeleteHistory));

        public TelegramView TelegramView(params TelegramResult[] viewResults)
            => new(ControllerContext, HttpContext, BotSettings, Update, viewResults.ToList());
    }
}