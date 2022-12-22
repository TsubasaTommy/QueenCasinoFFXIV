using System;
using System.Linq;
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
    private Logic logic;

    private Configuration configuration;
    

    public MainWindow(
        Plugin plugin, ChatGui chatGui

        ) : base(
        WINDOWNAME_MAIN, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)

    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        
        this.configuration = plugin.Configuration;
        this.chatGui = chatGui;
        this.Plugin = plugin;
        this.logic = new Logic(chatGui, plugin);

        //Event Chat update
        this.chatGui.ChatMessage += this.OnChat;

    }


    public void OnChat(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        
        
        if (type.ToString() == "2122")// dice or not
        {
           
            this.logic.MainLogic(message.ToString());
        }
        
        switch (type)
        {
            case XivChatType.Say:
            case XivChatType.Party:
                //if (Regex.IsMatch(sender.ToString(), "Tsubasa Roseline")) break;//sayでつぱた無視
                if (!Regex.IsMatch(message.ToString(), @"^bet|^b|^ｂ")) break;
                try
                {
                    string betMessage = Regex.Replace(message.ToString(), "[０-９]", delegate (Match m) {
                        char ch = (char)('0' + (m.Value[0] - '０'));
                        return ch.ToString();
                    });

                   
                    int bet;
                    Logic.betType betType;
           
                    bet = int.Parse(Regex.Match(betMessage.ToString(), @"[1-9][0-9]+").ToString());

                    if (BET_MAX < bet)
                    {
                        bet = BET_MAX;
                    }

                    betType = DetectBetType(betMessage.ToString());

                    if(betType != Logic.betType.none && 10 <= bet)
                    {
                        logic.AddPlayer(sender.ToString());
                        logic.SetBet(sender.ToString(), bet,betType);
                        
                    }
                }
                catch(FormatException e)
                {
                    PluginLog.Debug("MainWindow Onchat でえらー");
                }
                break;
            case XivChatType.Yell:
                if (Regex.IsMatch(message.ToString(), @"運命のダイス"))
                {
                    logic.InitializeDice();
                    PluginLog.Debug("logic.dice initialized");
                }
                break;
        }


    }

    public void Dispose()
    {
        this.chatGui.ChatMessage -= this.OnChat;
        this.logic.Dispose();
    }

    public override void Draw()
    {
        var flags =  ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg ;
        
        if (ImGui.BeginTable("setting", 1, flags))
        {
            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            if (ImGui.Button("All reset")) this.logic = new Logic(chatGui,Plugin);

            ImGui.EndTable();
        }

        if (ImGui.BeginTable("number", logic.num.Length,flags))
        {
            for(int i = 0; i < logic.num.Length; i++)
            {
                ImGui.TableSetupColumn($"Num:{i + 1}");
            }
            ImGui.TableHeadersRow();
            foreach (var (x, i) in logic.num.Select((value, index) => (value, index)))
            {
                ImGui.TableNextColumn();
                ImGui.Text($"{x}");
            }
            ImGui.EndTable();
        }


        if(ImGui.BeginTable("player", 6,flags))
        {
            ImGui.TableSetupColumn($"Premium");
            ImGui.TableSetupColumn($"Name");
            ImGui.TableSetupColumn($"Score");
            ImGui.TableSetupColumn($"Type");
            ImGui.TableSetupColumn($"Bet");
            ImGui.TableSetupColumn($"Delete");

            ImGui.TableHeadersRow();

            for (int row = 0; row < logic.players.Count; row++)
            {
                ImGui.TableNextColumn();
                bool isPremium = configuration.name_P.Contains(logic.players[row].name);
                if (ImGui.Button($"{isPremium}"))
                {
                    if (isPremium) configuration.name_P.Remove(logic.players[row].name);
                    else           configuration.name_P.Add(logic.players[row].name);
                    configuration.Save();
                    logic.RefreshPremium(configuration.name_P);
                }

                ImGui.TableNextColumn();
                ImGui.Text($"{logic.players[row].name}");

                ImGui.TableNextColumn();
                ImGui.Text($"{logic.players[row].score}");

                ImGui.TableNextColumn();
                ImGui.Text($"{logic.players[row].type}");

                ImGui.TableNextColumn();
                ImGui.Text($"{logic.players[row].bet}");

                ImGui.TableNextColumn();
                if (ImGui.Button("X")) logic.players.RemoveAt(row);

                ImGui.TableNextRow();
            }
            ImGui.EndTable();
        }
    }
    private Logic.betType DetectBetType(string str)
    {
        str= str.ToLower();
        str = Regex.Replace(str, @"[1-9][0-9]+", "");

        if (Regex.IsMatch(str, @"high|ﾊｲ|ハイ|はい")) return Logic.betType.high;
        if (Regex.IsMatch(str, @"low|ﾛｳ|ロウ|ロー|ﾛｰ|ろう|ろー|ろｰ")) return Logic.betType.low;
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
