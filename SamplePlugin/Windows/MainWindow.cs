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
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;



namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private ChatGui chatGui;
    private Plugin Plugin;
    private XivChatEntry xivChat;
    private XivChatType xivType = new XivChatType();
    private Logic logic = new Logic(true);
    private string type;
    private uint print_senderId;
    private string print_message;
    private string print_name;
    private string print_debug;
    

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


        //Event Chat update
        this.chatGui.ChatMessage += this.OnChat;

    }


    public void OnChat(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
   
        string messageStr = Strings.StrConv(message.ToString(), VbStrConv.Narrow, 0);
        


        if (type.ToString() == "2122")// dice or not
        {
            this.print_debug = "dice";
            message.ToString();
            this.logic.MainLogic(messageStr);
        }
        switch (type)
        {
            case XivChatType.Say:
                try
                {
                    if (Regex.IsMatch(sender.ToString(), "Tsubasa Roseline")) break;//sayでつぱた無視
                    int bet = int.Parse(Regex.Match(messageStr, @"[1-9][0-9]+").ToString());
                    Logic.betType betType = DetectBetType(messageStr);
                    
                    if(betType != Logic.betType.none && 10 <= bet)
                    {
                        logic.AddPlayer(sender.ToString());
                        logic.SetBet(sender.ToString(), bet, betType);
                    }
                }
                catch(FormatException e)
                {
                    this.print_debug = "SayでのBetが有効な文字列ではない";
                }
                break;
            default:
                this.print_debug = "";
                break;
        }

        this.type = type.ToString();
        this.print_senderId = senderId;
        this.print_message = message.ToString();
        this.print_name = sender.ToString();
    }

    public void Dispose()
    {
        this.chatGui.ChatMessage -= this.OnChat;
    }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {this.Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            this.Plugin.DrawConfigUI();
        }


        ImGui.Spacing();

        ImGui.Text(this.print_debug);
        ImGui.Text(this.type);
        ImGui.Text(this.print_senderId.ToString());
        ImGui.Text(this.print_name);
        ImGui.Text(this.print_message);

        foreach(Logic.Player x in logic.players)
        {
            ImGui.Text($"{x.name}:{x.score}:{x.type}:{x.bet}");
        }


        ImGui.Indent(55);
        
        ImGui.Unindent(55);

    }
    private Logic.betType DetectBetType(string str)
    {
        str= str.ToLower();
        str = Regex.Replace(str, @"[1-9][0-9]+", "");

        if (Regex.IsMatch(str, @"high|ﾊｲ|はい")) return Logic.betType.high;
        if (Regex.IsMatch(str, @"low|ﾛｳ|ﾛｰ|ろう|ろー|ろｰ")) return Logic.betType.low;
        try
        {
            switch (int.Parse(Regex.Match(str, @"[1-6]").ToString()))
            {
                case 1:
                    return Logic.betType.num1;
                case 2:
                    return Logic.betType.num2;
                case 3:
                    return Logic.betType.num3;
                case 4:
                    return Logic.betType.num4;
                case 5:
                    return Logic.betType.num5;
                case 6:
                    return Logic.betType.num6;
                default:
                    return Logic.betType.none;
            }
        }
        catch
        {
            return Logic.betType.none;
        }


    }
}
