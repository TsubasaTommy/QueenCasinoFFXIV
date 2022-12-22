using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SamplePlugin.Constants;

namespace SamplePlugin
{
    public class Logic: IDisposable
    {
        private Plugin Plugin;
        private ChatGui chatGui;
        private Configuration configuration;

        private string msgs;
        private string results;

        private double rateBonus;
        private double[] rateNum = new double[3] ;
        private double rateHigh;
        private double rateLow;

        private int[] dice = {0,0,0};
        public byte[] num = new byte[6];
        public bool high = false;
        public bool low = false;
        public List<Player> players = new List<Player>();
       
        public  enum betType
        {
            none,
            num1,
            num2,
            num3,
            num4,
            num5,
            num6,
            high,
            low,
        };
        public struct Player
        {
            public bool premium;
            public string name;
            public double score;
            public int bet;
            public betType type;
        }

        public Logic(ChatGui chatGui,Plugin plugin) 
        {
            this.chatGui = chatGui;
            this.Plugin = plugin;
            this.configuration = plugin.Configuration;
            RefreshPremium(configuration.name_P);
        }
        private int TrimString(string dice)
        {
            if(Regex.IsMatch(dice, @"Tsubasa Roseline")&& Regex.IsMatch(dice, @"6"))
            {
                
                string result = Regex.Replace(dice, @".+?は、", "");
                return int.Parse(Regex.Replace(result, @"[^0-9]", ""));
            }
            return 0;
        }
        public void InitializeDice()
        {
            for(int i = 0; i< this.dice.Length; i++)
            {
                this.dice[i] = 0;
            }
        }
        public async void MainLogic(string dice)
        {
            if (!Regex.IsMatch(dice, "Tsubasa Roseline")) return; //Tsubasa Roseline 以外のダイスを無視
            if (this.players.Count == 0) return;

            for (int i=0; i< this.dice.Length; i++)
            {
                

                if (this.dice[i] == 0) 
                {     
                    this.dice[i] = this.TrimString(dice);
                   

                    break;
                }//dice うまってないところにトリムdiceぶちこむ
            }



            for (int i = 0; i < this.num.Length ; i++)//num初期化 前ので汚れてるから
            {
                this.num[i] = 0;
            }

            if (this.dice[this.dice.Length - 1] != 0)//dice 3つうまったら勝負判定
            {
                PluginLog.Debug($"{this.dice[0]}");
                PluginLog.Debug($"{this.dice[1]}");
                PluginLog.Debug($"{this.dice[2]}");

                if (this.dice.Sum() >= 12) //high
                {
                    this.high = true;
                    this.low = false;
                }
                if (this.dice.Sum() <= 9) //low
                {
                    this.high = false;
                    this.low = true;
                }
                foreach (int x in this.dice) //num
                {
                    switch (x)
                    {
                        case 1:
                            this.num[0] += 1;
                            break;
                        case 2:
                            this.num[1] += 1;
                            break;
                        case 3:
                            this.num[2] += 1;
                            break;
                        case 4:
                            this.num[3] += 1;
                            break;
                        case 5:
                            this.num[4] += 1;
                            break;
                        case 6:
                            this.num[5] += 1;
                            break;
                        default: break;
                    }
                }

                

                bool isBonus = false;
                foreach (int x in num)
                {
                    if (x == 3) {
                        isBonus = true;
                        break;
                    }
                }

                for (int i = 0; i < players.Count; i++) //playerに勝敗結果ぶちこんでく
                {
                    Player p = players[i];
                    RatePremiumOrNot(p.premium);
                    double bonus = isBonus?rateBonus:0;
                    switch (p.type)
                    {
                        case betType.high:
                            p.score += high ?
                                p.bet * (rateHigh + bonus) :
                                -p.bet;
                            break;
                        case betType.low:
                            p.score += low ?
                                p.bet * (rateLow + bonus) : 
                                -p.bet;
                            break;
                        case betType.num1:
                                p.score += num[0] > 0 ?
                                p.bet * (rateNum[num[0] - 1] + bonus) :
                            -p.bet;
                            break;
                        case betType.num2:
                            p.score += num[1] > 0 ?
                                p.bet * (rateNum[num[1] - 1] + bonus) :
                                -p.bet;
                            break;
                        case betType.num3:
                            p.score += num[2] > 0 ? 
                                p.bet * (rateNum[num[2] - 1] + bonus) :
                                -p.bet;
                            break;
                        case betType.num4:
                            p.score += num[3] > 0 ? 
                                p.bet * (rateNum[num[3] - 1] + bonus) :
                                -p.bet;
                            break;
                        case betType.num5:
                            p.score += num[4] > 0 ? 
                                p.bet * (rateNum[num[4] - 1] + bonus) :
                                -p.bet;
                            break;
                        case betType.num6:
                            p.score += num[5] > 0 ? 
                                p.bet * (rateNum[num[5] - 1] + bonus) :
                                -p.bet;
                            break;
                        default:
                            break;
                    }
                    p.type = betType.none;
                    var name = Regex.Replace(p.name, @"(?<= )(.*)","");
                    msgs += this.players[i].score < p.score ?
                        $"【{name}:{p.score}万G】 " :
                        $" 【{name}:{p.score}万G】 ";
                    //await Task.Delay(4000);
                    //SendMessage(msg, XivChatType.Yell);                 

                    this.players[i] = p;
                }
                chatGui.Print(msgs);
                msgs = "";
                results = $"{this.dice.Sum()} WIN:";
                if (high) results += "【High】";
                if (low) results += "【low】";
                for(int i =0;i < num.Length;i++)
                {
                    if (num[i] > 0) results += $"【{i + 1}】";
                }
                chatGui.Print(results);
                results = "";

            }
        }

        public void RefreshPremium(List<string> player_p)
        {
            for (int i = 0; i < this.players.Count;i++) 
            {
                if(player_p.Contains(this.players[i].name))
                {
                    Player x = this.players[i];
                    x.premium = true;
                    players[i] = x;
                }
                else
                {
                    Player x = this.players[i];
                    x.premium = false;
                    players[i] = x;
                }
            }
        }
        public void RatePremiumOrNot(bool isPremium)
        {
            if (isPremium)
            {
                rateBonus = RATE_P["bonus"];
                rateNum[0] = RATE_P["num1"];
                rateNum[1] = RATE_P["num2"];
                rateNum[2] = RATE_P["num3"];
                rateHigh = RATE_P["high"];
                rateLow = RATE_P["low"];
            }
            else
            {
                rateBonus = RATE["bonus"];
                rateNum[0] = RATE["num1"];
                rateNum[1] = RATE["num2"];
                rateNum[2] = RATE["num3"];
                rateHigh = RATE["high"];
                rateLow = RATE["low"];
            }
        }
        public void AddPlayer(string name)
        {
            if (this.players.Any(p => p.name == name))
            {//なまえとうろくされてたらreturn
                PluginLog.Debug("AddPlayer:登録済み");
                return;
            }

            this.players.Insert(0,new Player {name = name,score =0, bet = 0,type = betType.none});
            if(20<players.Count) players.RemoveAt(20);
            RefreshPremium(configuration.name_P);
        }
        public void RemovePlayer(string name)
        {
            this.players.RemoveAll(s => s.name == name);
        }
        public void SetBet(string name ,int bet,betType type)
        {
            for(int i = 0; i < this.players.Count;i++)
            {
                if (players[i].name == name)
                {
                    Player x = this.players[i];
                    x.bet = bet;
                    x.type = type;
                    this.players[i] = x;
                    PluginLog.Debug($"bet:{players[i].bet} type:{players[i].type}");
                    name = Regex.Replace(name, @"(?<= )(.*)", "");

                }
            }
        }
        public void SendMessage(string msg,XivChatType type,string name = "Tsubasa Roseline")
        {
            var xivChat = new XivChatEntry();
            var builder = new SeStringBuilder();
            xivChat.Message = builder.Append(msg).Build();
            builder = new SeStringBuilder();
            xivChat.Name = builder.Append(name).Build();
            xivChat.Type = type;
            xivChat.SenderId= 0;

            chatGui.PrintChat(xivChat);

        }
 
        public void Dispose()
        {
            players.Clear();
        }
    }

}
