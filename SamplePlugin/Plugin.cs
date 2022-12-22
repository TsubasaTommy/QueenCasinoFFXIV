using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;
using Dalamud.Game.Gui;
using Dalamud.Game.Text.SeStringHandling;
using SamplePlugin.Windows;
using static SamplePlugin.Constants;
using Dalamud.Game.Text;

namespace SamplePlugin
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Sample Plugin";
        private const string CommandName = "/pm";

        public MainWindow Main1;

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("SamplePlugin");

        public static string ?assetPath { get; set; }
        public ChatGui chatGui;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            [RequiredVersion("1.0")] ChatGui chatGui
            )
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            assetPath = PluginInterface.AssemblyLocation.Directory?.FullName!;
            var imagePath = Path.Combine(assetPath, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);


            Main1 = new MainWindow(this, chatGui);


            WindowSystem.AddWindow(new ConfigWindow(this));
            WindowSystem.AddWindow(Main1);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;


        }


        public void Dispose()
        {
            Main1.Dispose();
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(CommandName);
            
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            WindowSystem.GetWindow(WINDOWNAME_MAIN).IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            WindowSystem.GetWindow(WINDOWNAME_CONF).IsOpen = true;
        }
    }
}
