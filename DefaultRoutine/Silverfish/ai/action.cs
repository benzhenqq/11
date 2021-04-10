
 //TODO:

//cardids of duplicate + avenge
//nozdormu (for computing time :D)
//faehrtenlesen (tracking)
// lehrensucher cho
//scharmuetzel kills all :D
//todo deathlord-guessing
//todo kelthuzad dont know which minion died this turn in rl
namespace HREngine.Bots
{
    using System;
    using System.Text;

    public enum actionEnum
    {
        endturn = 0,
        playcard,
        attackWithHero,
        useHeroPower,
        attackWithMinion
    }
    //todo make to struct

    public class Action
    {

        public actionEnum actionType;
        public Handmanager.Handcard card;
        //public int cardEntitiy;
        public int place; //= target where card/minion is placed
        public Minion own;
        public Minion target;
        public int druidchoice; // 1 left card, 2 right card
        public int penalty;
        public int turn = -1;
        public int prevHpOwn = -1;
        public int prevHpTarget = -1;

        public Action(actionEnum type, Handmanager.Handcard hc, Minion ownM, int place, Minion targetM, int pen, int choice)
        {
            this.actionType = type;
            this.card = hc;
            this.own = ownM;
            this.place = place;
            this.target = targetM;
            this.penalty = pen;
            this.druidchoice = choice;
            if (ownM != null) prevHpOwn = ownM.Hp;
            if (targetM != null) prevHpTarget = targetM.Hp;
        }
        
        public Action(Action a)
        {
            this.actionType = a.actionType;
            this.card = a.card;
            this.place = a.place;
            this.own = a.own;
            this.target = a.target;
            this.druidchoice = a.druidchoice;
            this.penalty = a.penalty;
            this.prevHpOwn = a.prevHpOwn;
            this.prevHpTarget = a.prevHpTarget;
        }

        public void print(bool tobuffer = false)
        {
            Helpfunctions help = Helpfunctions.Instance;
            if (tobuffer)
            {
                if (this.actionType == actionEnum.playcard)
                {
                    string playaction = "play ";

                    playaction += "id " + this.card.entity;
                    if (this.target != null)
                    {
                        playaction += " target " + this.target.entitiyID;
                    }

                    if (this.place >= 0)
                    {
                        playaction += " pos " + this.place;
                    }

                    if (this.druidchoice >= 1) playaction += " choice " + this.druidchoice;

                    help.writeToBuffer(playaction);
                }
                if (this.actionType == actionEnum.attackWithMinion)
                {
                    help.writeToBuffer("attack " + this.own.entitiyID + " enemy " + this.target.entitiyID);
                }
                if (this.actionType == actionEnum.attackWithHero)
                {
                    help.writeToBuffer("heroattack " + this.target.entitiyID);
                }
                if (this.actionType == actionEnum.useHeroPower)
                {

                    if (this.target != null)
                    {
                        help.writeToBuffer("useability on target " + this.target.entitiyID);
                    }
                    else
                    {
                        help.writeToBuffer("useability");
                    }
                }
                return;
            }
            if (this.actionType == actionEnum.playcard)
            {
                string playaction = "play ";

                playaction += "id " + this.card.entity;
                if (this.target != null)
                {
                    playaction += " target " + this.target.entitiyID;
                }

                if (this.place >= 0)
                {
                    playaction += " pos " + this.place;
                }

                if (this.druidchoice >= 1) playaction += " choice " + this.druidchoice;

                help.logg(playaction);
            }
            if (this.actionType == actionEnum.attackWithMinion)
            {
                help.logg("attacker: " + this.own.entitiyID + " enemy: " + this.target.entitiyID);
            }
            if (this.actionType == actionEnum.attackWithHero)
            {
                help.logg("attack with hero, enemy: " + this.target.entitiyID);
            }
            if (this.actionType == actionEnum.useHeroPower)
            {
                help.logg("useability ");
                if (this.target != null)
                {
                    help.logg("on enemy: " + this.target.entitiyID);
                }
            }
            help.logg("");

            // 打印行动说明
            if (printUtils.printNextMove)
            {
                if (this.penalty == 0) return;
                StringBuilder str = new StringBuilder("", 100);
                switch (this.actionType)
                {
                    case actionEnum.endturn:
                        str.Append("回合结束");
                        break;
                    case actionEnum.playcard:
                        str.Append("打出");
                        str.Append(this.card == null ? "无" : this.card.card.卡名);
                        str.Append("，目标 ");
                        str.Append(this.target == null ? "无" : this.target.handcard.card.卡名);
                        break;
                    case actionEnum.attackWithHero:
                        str.Append("让英雄攻击 ");
                        str.Append(this.target == null ? "无" : this.target.handcard.card.卡名);
                        break;
                    case actionEnum.useHeroPower:
                        str.Append("使用英雄技能");
                        str.Append("，目标 ");
                        str.Append(this.target == null ? "无" : this.target.handcard.card.卡名);
                        break;
                    case actionEnum.attackWithMinion:
                        str.Append("使用随从 ");
                        str.Append(this.own == null ? "无" : this.own.handcard.card.卡名);
                        str.Append(" 攻击 ");
                        str.Append(this.target == null ? "无" : this.target.handcard.card.卡名);
                        break;
                }
                str.Append("，当前受到 " + this.penalty + " 点惩罚！");
                Helpfunctions.Instance.ErrorLog(str.ToString());
            }
            help.logg("");
        }
        
        public string printString()
        {
            string retval = "";

            if (this.actionType == actionEnum.playcard)
            {
                retval += "play ";

                retval += "id " + this.card.entity;
                if (this.target != null)
                {
                    retval += " target " + this.target.entitiyID;
                }

                if (this.place >= 0)
                {
                    retval += " pos " + this.place;
                }
                if (this.druidchoice >= 1) retval += " choice " + this.druidchoice;
            }
            if (this.actionType == actionEnum.attackWithMinion)
            {
                retval += ("attacker: " + this.own.entitiyID + " enemy: " + this.target.entitiyID);
            }
            if (this.actionType == actionEnum.attackWithHero)
            {
                retval += ("attack with hero, enemy: " + this.target.entitiyID);
            }
            if (this.actionType == actionEnum.useHeroPower)
            {
                retval += ("useability ");
                if (this.target != null)
                {
                    retval += ("on target: " + this.target.entitiyID);
                }
            }

            return retval;
        }

    }

    
}