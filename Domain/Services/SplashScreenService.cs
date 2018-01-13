using System;
using System.Drawing;
using System.Threading;
using Colorful;
using CryptoKeeper.Domain.Services.Interfaces;
using Console = System.Console;

namespace CryptoKeeper.Domain.Services
{
    public class SplashScreenService
    {
        private readonly IAsciiImageConverterService _asciiImageConverterService;
        private readonly IResourceService _resourceService;
        private readonly IClearConsoleService _clearConsoleService;

        public SplashScreenService(): this(new AsciiImageConverterService(), new ResourceService(), new ClearConsoleService())
        { }

        public SplashScreenService(IAsciiImageConverterService asciiImageConverterService, IResourceService resourceService, IClearConsoleService clearConsoleService)
        {
            _asciiImageConverterService = asciiImageConverterService;
            _resourceService = resourceService;
            _clearConsoleService = clearConsoleService;
        }
        public void ShowSplashScreen()
        {
            var bytes = _resourceService.ExtractResource("CryptoKeeper.Domain.Resources.cryptkeeper.jpg");
            var asciiImage = _asciiImageConverterService.ImageToAscii(bytes);
            foreach (var character in asciiImage)
            {
                Colorful.Console.Write(character.Character, character.Color);
                Thread.Sleep(1);
            }
            var font = FigletFont.Load(_resourceService.ExtractResource("CryptoKeeper.Domain.Resources.chunky.flf"));
            var figlet = new Figlet(font);
            Colorful.Console.WriteLine(figlet.ToAscii("CryptoKeeper"), Color.Orange);
            Colorful.Console.WriteLine(figlet.ToAscii("CryptoKeeper"), Color.OrangeRed);
            Colorful.Console.WriteLine(figlet.ToAscii("CryptoKeeper"), Color.Red);
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            //_clearConsoleService.Clear();
        }
    }
}