using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Game.Text.SeStringHandling;
using ImGuiNET;
using ImGuiScene;
using static SamplePlugin.Constants;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Game.Text;
using Dalamud.Game.Gui;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private ChatGui chatGui;
    private Plugin Plugin;
    private SeString sestring = new SeString();
    private string print_string;
    

    public MainWindow(
        Plugin plugin, ChatGui chatGui

        ) : base(
        WINDOWNAME_MAIN, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)

    {
        this.chatGui = chatGui;
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;

        PluginLog.Debug("Fuckyou!");
        //Event Chat update
        this.chatGui.ChatMessage += this.OnChat;

    }


    public void OnChat(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        //this.chatGui.Print("detect chatlog updated");
        PluginLog.Debug("detect chatlog updated");
        this.print_string = message.ToString();
    }

    public void Dispose()
    {
        this.chatGui.ChatMessage-= this.OnChat;
    }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {this.Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            this.Plugin.DrawConfigUI();
        }
        if (ImGui.Button("Print message"))
        {
            this.chatGui.Print("hello");
        }


        ImGui.Spacing();

        ImGui.Text(this.print_string);

        ImGui.Indent(55);
        
        ImGui.Unindent(55);

    }
}
