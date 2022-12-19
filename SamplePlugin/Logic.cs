using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SamplePlugin
{
    internal class Logic
    {
        private bool bonus;
        private double rateBonus = 0;
        private double[] rateNum = { 1, 2, 10 };
        private double rateHigh = 1.5;
        private double rateLow = 1.5;

        private int[] dice = {0,0,0};
        private byte[] num = new byte[5];
        private bool high = false;
        private bool low = false;
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
            public string name;
            public double score;
            public int bet;
            public betType type;
        }

        public Logic(bool bonus) 
        {
            this.bonus = bonus;
        }
        private int TrimString(string dice)
        {
            if(Regex.IsMatch(dice, @"Tsubasa Roseline"))
            {
                string pattern = @"(?<=面)(.*)";
                string result = Regex.Replace(dice, pattern, "");
                pattern = @"[0-9]";
                return int.Parse(Regex.Replace(result, pattern, ""));
            }
            return 0;
        }
        private void InitializeDice()
        {
            for(int i = 0; i< this.dice.Length; i++)
            {
                this.dice[i] = 0;
            }
        }
        public void MainLogic(string dice)
        {
            if (Regex.IsMatch(dice, "Tsubasa Roseline")) return; //Tsubasa Roseline 以外のダイスを無視
            if (this.players.Count == 0) return;

            if (this.dice[this.dice.Length - 1] != 0) this.InitializeDice();//diceが３つ埋まってたら初期化

            for (int i=0; i< this.dice.Length; i++)
            {
                if (this.dice[i] == 0) this.dice[i] = this.TrimString(dice);//dice うまってないところにトリムdiceぶちこむ
            }

            for(int i = 0; i < this.num.Length; i++)//num初期化 前ので汚れてるから
            {
                this.num[i] = 0;
            } 

            if (this.dice[this.dice.Length - 1] != 0)//dice 3つうまったら勝負判定
            {
                if(this.dice.Sum() >=12 ) //high
                {
                    this.high = true;
                    this.low = false;
                }
                if (this.dice.Sum() <= 9) //low
                { 
                    this.high = false;
                    this.low = true;
                }
                foreach(int x in this.dice) //num
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
                        default:break;
                    }
                }
                foreach(int x in this.num)//ボーナス判定
                {
                    this.rateBonus = (x == 3)&&this.bonus ? 4.0 : 0;
                }
                this.players.ForEach(p => //playerに勝敗結果ぶちこんでく
                {
                    switch (p.type)
                    {
                        case betType.high:
                            p.score += this.high ?
                            p.bet * (this.rateHigh+this.rateBonus) :
                            -p.bet;
                            break;
                        case betType.low:
                            p.score += this.low ?
                            p.bet * (this.rateLow + this.rateBonus) : 
                            -p.bet;
                            break;
                        case betType.num1:
                            p.score += this.num[0] > 0 ?
                            p.bet * (rateNum[this.num[0]--] + this.rateBonus) :
                            -p.bet;
                            break;
                        case betType.num2:
                            p.score += this.num[1] > 0 ?
                            p.bet * (rateNum[this.num[1]--] + this.rateBonus) :
                            -p.bet;
                            break;
                        case betType.num3:
                            p.score += this.num[2] > 0 ?
                            p.bet * (rateNum[this.num[2]--] + this.rateBonus) :
                            -p.bet;
                            break;
                        case betType.num4:
                            p.score += this.num[3] > 0 ?
                            p.bet * (rateNum[this.num[3]--] + this.rateBonus) :
                            -p.bet;
                            break;
                        case betType.num5:
                            p.score += this.num[4] > 0 ?
                            p.bet * (rateNum[this.num[4]--] + this.rateBonus) :
                            -p.bet;
                            break;
                        case betType.num6:
                            p.score += this.num[5] > 0 ?
                            p.bet * (rateNum[this.num[5]--] + this.rateBonus) :
                            -p.bet;
                            break;
                        default:
                            break;
                    }
                    p.type = betType.none;
                });

            }
        }
        public void AddPlayer(string name)
        {
            if (this.players.Any(p => p.name == name)) return;//なまえとうろくされてたらreturn
            this.players.Insert(0,new Player {name = name,score =0, bet = 0,type = betType.none});
            if(20<players.Count) players.RemoveAt(20);
        }
        public void RemovePlayer(string name)
        {
            this.players.RemoveAll(s => s.name == name);
        }
        public void SetBet(string name ,int bet,betType type)
        {

            this.players.ForEach(s =>
            {
                if(s.name == name)
                {
                    s.bet = bet;
                    s.type = type;
                }
            });
        }
    }

}
