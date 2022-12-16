using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Game.Text.SeStringHandling;
using ImGuiNET;
using ImGuiScene;
using static SamplePlugin.Constants;
using Dalamud.IoC;
using Dalamud.Game.Gui;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private ChatGui chatgui;
    private Plugin Plugin;
    private SeString sestring = new SeString();
    private string print_string; 

    public MainWindow(
        Plugin plugin, ChatGui chatgui
        
        ) : base(
        WINDOWNAME_MAIN, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)

    {
        this.chatgui = chatgui;
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;

    }

    public void Dispose()
    {

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
            this.chatgui.Print("hello");
        }


        ImGui.Spacing();

        ImGui.Text("");

        ImGui.Indent(55);
        
        ImGui.Unindent(55);

    }
}
