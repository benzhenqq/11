using System.Collections.Generic;
using System;

namespace HREngine.Bots
{
    public class Behavior动物园 : Behavior
    {
        public override string BehaviorName() { return "动物园"; }
        PenalityManager penman = PenalityManager.Instance;
        // 核心，场面值
        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value >= -2000000) return p.value;
            // 场面值
            int retval = 0;
            // 危险血线
            int hpboarder = 6;
            // 抢脸血线
            int aggroboarder = 20;
            // --------------------------卡组特性---------------------------------
            // 圣契奖励
            retval += p.libram > 2 ? 60 + p.libram * 10 : p.libram * 30;
            // --------------------------通用---------------------------------
            // 计算额外惩罚
            retval -= p.evaluatePenality;
            // 手牌价值（可能导致不出牌）
            // retval += p.owncards.Count * 5;
            retval -= p.enemycarddraw * 5;

            // 最大法力水晶
            retval += p.ownMaxMana * 20 - p.enemyMaxMana * 20;
            // 英雄技能
            // retval += (p.enemyHeroAblility.manacost - p.ownHeroAblility.manacost) * 4;
            // if (p.ownHeroPowerAllowedQuantity != p.enemyHeroPowerAllowedQuantity)
            // {
            //     if(p.ownHeroPowerAllowedQuantity > p.enemyHeroPowerAllowedQuantity) retval += 3;
            //     else retval -= 3;
            // }
            // 对法师，需要考虑法强效果
            if (p.enemyHeroName == HeroEnum.mage ) retval -= 4 * p.enemyspellpower;
            // 血线安全
            if (p.ownHero.Hp + p.ownHero.armor > hpboarder)
            {
                retval += p.ownHero.Hp + p.ownHero.armor;
            }
            // 快死了
            else
            {
                if (p.nextTurnWin()) retval -= (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
                else retval -= 2 * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor) * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
            }
            // 对手血线安全
            if (p.enemyHero.Hp + p.enemyHero.armor > aggroboarder)
            {
                retval += -p.enemyHero.Hp - p.enemyHero.armor;
            }
            // 开始打脸
            else
            {
                retval += 4 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor);
            }
            // 武器价值
            if (p.ownWeapon.Angr > 0)
            {
                if (p.ownWeapon.Angr > 1) retval += p.ownWeapon.Angr * p.ownWeapon.Durability;
                else retval += p.ownWeapon.Angr * p.ownWeapon.Durability + 1;
            }
            // 敌方武器价值
            if (!p.enemyHero.frozen)
            {
                retval -= p.enemyWeapon.Durability * p.enemyWeapon.Angr;
            }
            else
            {
                if (p.enemyWeapon.Durability >= 1)
                {
                    retval += 12;
                }
            }
            // 抽牌价值
            //RR card draw value depending on the turn and distance to lethal
            //RR if lethal is close, carddraw value is increased
            if (p.lethalMissing() <= 5) //RR
            {
                retval += p.owncarddraw * 100;
            }
            if (p.ownMaxMana < 4)
            {
                retval += p.owncarddraw * 2;
            }
            else
            {
                retval += p.owncarddraw * 10;
            }
            // 卡差
            // if (p.owncarddraw + 1 >= p.enemycarddraw) retval -= p.enemycarddraw * 7;
            // else retval -= (p.owncarddraw + 1) * 7 + (p.enemycarddraw - p.owncarddraw - 1) * 12;

            // 计算我方随从价值
            //int owntaunt = 0;
            // int readycount = 0;
            // int ownMinionsCount = 0;
            foreach (Minion m in p.ownMinions)
            {
                retval += this.getMyMinionValue(m, p);
                // if (m.Ready) readycount++;
                // if (m.Hp <= 4 && (m.Angr > 2 || m.Hp > 3)) ownMinionsCount++;
            }
            // 克苏恩计数器
            // retval += p.anzOgOwnCThunAngrBonus;
            retval += p.anzOwnExtraAngrHp - p.anzEnemyExtraAngrHp;

            /*if (p.enemyMinions.Count >= 0)
            {
                int anz = p.enemyMinions.Count;
                if (owntaunt == 0) retval -= 10 * anz;
                retval += owntaunt * 10 - 11 * anz;
            }*/

            // 使用英雄技能
            bool useAbili = false;
            // 使用硬币
            int usecoin = 0;
            //soulfire etc
            int deletecardsAtLast = 0;
            int wasCombo = 0;
            bool firstSpellToEnHero = false;
            // 出牌序列数量
            int count = p.playactions.Count;
            int ownActCount = 0;
            // 排序问题！！！！
            for (int i = 0; i < count; i++)
            {
                Action a = p.playactions[i];
                ownActCount++;
                switch (a.actionType)
                {
                    // 英雄攻击
                    case actionEnum.attackWithHero:
                        // 抢脸血线
                        if (p.enemyHero.Hp <= p.attackFaceHP) retval++;
                        // 战士
                        // if (p.ownHeroName == HeroEnum.warrior && useAbili) retval -= 1;
                        continue;
                    case actionEnum.useHeroPower:
                        useAbili = true;
                        continue;
                    case actionEnum.playcard:
                        break;
                    default:
                        continue;
                }
                // 优先打出的牌
                if(i == 0) {
                    switch(a.card.card.卡名){
                        // 排序优先
                        case "生命分流":
                            retval ++;
                            break;
                        case "飞刀杂耍者":
                            retval ++;
                            break;
                    }
                }
                // 最后打出的牌
                if(i == count - 1) {
                    switch(a.card.card.卡名){
                        case "雷霆绽放":
                            retval -= 100;
                            break;
                        case "激活":
                        case "幸运币":
                            retval -= 10;
                            break;
                    }
                }
                switch (a.card.card.卡名)
                {
                    case "雷霆绽放":
                        usecoin+=2;
                        break;
                    case "激活":
                    case "幸运币":
                        usecoin++;
                        break;
                }
                // 连击
                if (a.card.card.Combo && i > 0) wasCombo++;
                if (a.target == null) continue;
                //save spell for all classes
                // 法术不打脸？
                if (a.card.card.type == CardDB.cardtype.SPELL && (a.target.isHero && !a.target.own))
                {
                    if (i == 0) firstSpellToEnHero = true;
                    retval -= 11;
                }
            }
            if (wasCombo > 0 && firstSpellToEnHero)
            {
                if (wasCombo + 1 == ownActCount) retval += 10;
            }
            if (usecoin > 0)
            {
                // 别跳币英雄技能
                if (useAbili && p.ownMaxMana <= 2) retval -= 40;
                retval -= 5 * p.manaTurnEnd;
                if (p.manaTurnEnd + usecoin > 10) retval -= 5 * usecoin;
            }
            // 法力水晶还剩下 2 个并且还能用英雄技能
            if (p.manaTurnEnd >= 2 && !useAbili && p.ownAbilityReady)
            {
                switch (p.ownHeroAblility.card.name)
                {
                    case CardDB.cardName.heal: goto case CardDB.cardName.lesserheal;
                    case CardDB.cardName.lesserheal:
                        bool wereTarget = false;
                        if (p.ownHero.Hp < p.ownHero.maxHp) wereTarget = true;
                        if (!wereTarget)
                        {
                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.wounded) { wereTarget = true; break;}
                            }
                        }
                        if (wereTarget && !(p.anzOwnAuchenaiSoulpriest > 0 || p.embracetheshadow > 0)) retval -= 10;
                        break;
                    case CardDB.cardName.poisoneddaggers: goto case CardDB.cardName.daggermastery;
                    case CardDB.cardName.daggermastery:
                         if (!(p.ownWeapon.Durability > 1 || p.ownWeapon.Angr > 1)) retval -= 10;
                         break;
                    case CardDB.cardName.totemicslam: goto case CardDB.cardName.totemiccall;
                    case CardDB.cardName.totemiccall:
                        if (p.ownMinions.Count < 7) retval -= 10;
                        else retval -= 3;
                        break;
                    case CardDB.cardName.thetidalhand: goto case CardDB.cardName.reinforce;
                    case CardDB.cardName.thesilverhand: goto case CardDB.cardName.reinforce;
                    case CardDB.cardName.reinforce:
                        if (p.ownMinions.Count < 7) retval -= 10;
                        else retval -= 3;
                        break;
                    case CardDB.cardName.soultap: 
                        if (p.owncards.Count < 10 && p.ownDeckSize > 0) retval -= 10;
                        break;
                    case CardDB.cardName.lifetap: 
                        if (p.owncards.Count < 10 && p.ownDeckSize > 0)
                        {
                            retval -= 30;
                            if (p.ownHero.immune) retval-= 10;
                        }
                        break;
                    default:
                        retval -= 10;
                        break;
                }
            }
            // if (usecoin && p.mana >= 1) retval -= 20;
            // 尽量耗尽法力
            retval -= p.manaTurnEnd;
            // 手里的随从
            int mobsInHand = 0;
            int bigMobsInHand = 0;
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if (hc.card.type == CardDB.cardtype.MOB)
                {
                    mobsInHand++;
                    if (hc.card.Attack + hc.addattack >= 3) bigMobsInHand++;
                    retval += hc.addattack + hc.addHp + hc.elemPoweredUp;
                }
                // 如果手上有侏儒就很赚
                // if (hc.card.卡名 == "甩笔侏儒"){
                //     retval += 50;
                // }
            }
            // 手里大随从数量
            // if (ownMinionsCount - p.enemyMinions.Count >= 4 && bigMobsInHand >= 1)
            // {
            //     retval += bigMobsInHand * 25;
            // }

            // 敌方随从
            //bool hasTank = false;
            foreach (Minion m in p.enemyMinions)
            {
                retval -= this.getEnemyMinionValue(m, p);
                //hasTank = hasTank || m.taunt;
            }
            retval -= p.enemyMinions.Count * 2;
            /*foreach (SecretItem si in p.enemySecretList)
            {
                if (readycount >= 1 && !hasTank && si.canbeTriggeredWithAttackingHero)
                {
                    retval -= 100;
                }
                if (readycount >= 1 && p.enemyMinions.Count >= 1 && si.canbeTriggeredWithAttackingMinion)
                {
                    retval -= 100;
                }
                if (si.canbeTriggeredWithPlayingMinion && mobsInHand >= 1)
                {
                    retval -= 25;
                }
            }*/
            // 敌方奥秘
            retval -= p.enemySecretCount;
            // 溢出伤害
            retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2
            // 溢出武器伤害
            retval -= p.lostWeaponDamage;

            //if (p.ownMinions.Count == 0) retval -= 20;
            //if (p.enemyMinions.Count == 0) retval += 20;
            // 已斩杀
            if (p.enemyHero.Hp <= 0)
            {
                retval += 10000;
                if (retval < 10000) retval = 10000;
            }
            // 感觉神抽能斩
            if (p.enemyHero.Hp >= 1 && p.guessingHeroHP <= 0)
            {
                if (p.turnCounter < 2) retval += p.owncarddraw * 100;
                retval -= 1000;
            }
            // 濒死
            if (p.ownHero.Hp <= 0) retval -= 10000;
            // 场面值
            p.value = retval;
            return retval;
        }


        // 敌方随从价值 主要等于 
        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            int retval = 5;
            retval += m.Hp * 2;
            retval +=  m.Angr * m.Hp / 8;
            if (!m.frozen && !(m.cantAttack && m.name != CardDB.cardName.argentwatchman))
            {
                retval += m.Angr * 2;
                if (m.windfury) retval += m.Angr * 2;
            }

            if (!m.handcard.card.isSpecialMinion)
            {
                if (m.Angr == 0) retval -= 7;
                else if (m.Angr <= 2 && m.Hp <= 2 && !m.divineshild) retval -= 5;
            }
            else retval += m.handcard.card.rarity;
			
            if (m.taunt) retval += 5;
            if (m.divineshild) retval += m.Angr;
            if (m.divineshild && m.taunt) retval += 5;
            if (m.stealth) retval += 1;

            if (m.poisonous)
            {
                retval += 4;
                if (p.ownMinions.Count < p.enemyMinions.Count) retval += 10;
            }
            if (m.lifesteal) retval += m.Angr;

            if (m.handcard.card.targetPriority >= 1 && !m.silenced)
            {
                retval += m.handcard.card.targetPriority;
            }
            if (m.name == CardDB.cardName.nerubianegg && m.Angr <= 3 && !m.taunt) retval = 0;
            retval += m.synergy;

            switch(m.handcard.card.卡名){
                // 解不掉游戏结束
                case "加基森拍卖师":
                case "任务达人":
                case "苔原犀牛":
                    retval += 100;
                    break;
                // 解不掉大劣势
                case "飞刀杂耍者":
                case "迦顿男爵":
                case "大法师安东尼达斯":
                case "巫师学徒":
                case "火舌图腾":
                case "法力之潮图腾":
                    retval += 20;
                    break;
                // 我劝你最好解了
                case "北郡牧师":
                case "狂野炎术士":
                case "奥金尼灵魂祭司":
                case "暴风城勇士":
                case "法力浮龙":
                case "雷欧克":
                case "森林狼":
                case "饥饿的秃鹫":
                case "恐狼前锋":
                case "食腐土狼":
                case "先知维纶":
                case "炎魔之王拉格纳罗斯":
                case "铸甲师":
                    retval += 10;
                    break;
                // 带点异能随手解一下吧
                case "末日预言者":
                case "年轻的女祭司":
                    retval += 3;
                    break;
                // 大哥，求别解
                case "希尔瓦娜斯":
                    retval -= 10;
                    break;                
            }
            return retval;
        }

        public override int getMyMinionValue(Minion m, Playfield p)
        {
            int retval = 5;
            retval += m.Hp * 2;
            if(!m.cantAttack || !m.Ready || !m.frozen){
                retval += m.Angr * 2;
            }else {
                retval += m.Angr / 2;
            }
            retval +=  m.Angr * m.Hp / 8;
            // 风怒价值
            if ((!m.playedThisTurn || m.rush == 1 || m.charge == 1 )  && m.windfury) retval += m.Angr;
            // 圣盾价值
            if (m.divineshild) retval += m.Angr / 2 + 1;
            // 潜行价值
            if (m.stealth) retval += m.Angr / 3 + 1;
            // 吸血
            if (m.lifesteal) retval += m.Angr / 3 + 1;
            // 圣盾嘲讽
            if (m.divineshild && m.taunt) retval += 4;
            retval += m.synergy;
            switch(m.handcard.card.卡名){
                case "飞刀杂耍者":
                    retval += 5;
                    break;
            }
            return retval;
        }

        public override int getSirFinleyPriority(List<Handmanager.Handcard> discoverCards)
        {
            
            return -1; //comment out or remove this to set manual priority
            int sirFinleyChoice = -1;
            int tmp = int.MinValue;
            for (int i = 0; i < discoverCards.Count; i++)
            {
                CardDB.cardName name = discoverCards[i].card.name;
                if (SirFinleyPriorityList.ContainsKey(name) && SirFinleyPriorityList[name] > tmp)
                {
                    tmp = SirFinleyPriorityList[name];
                    sirFinleyChoice = i;
                }
            }
            return sirFinleyChoice;
        }

        private Dictionary<CardDB.cardName, int> SirFinleyPriorityList = new Dictionary<CardDB.cardName, int>
        {
            //{HeroPowerName, Priority}, where 0-9 = manual priority
            { CardDB.cardName.lesserheal, 0 }, 
            { CardDB.cardName.shapeshift, 6 },
            { CardDB.cardName.fireblast, 7 },
            { CardDB.cardName.totemiccall, 1 },
            { CardDB.cardName.lifetap, 9 },
            { CardDB.cardName.daggermastery, 5 },
            { CardDB.cardName.reinforce, 4 },
            { CardDB.cardName.armorup, 2 },
            { CardDB.cardName.steadyshot, 8 }
        };

        public override int getAttackWithMininonPenality(Minion m, Playfield p, Minion target)
        {
            // 鼓励拥有智慧圣契的随从送死
            // if(m.libramofwisdom > 0 && !target.isHero  && p.ownMinions.Count > 1){
            //     if(target.Angr >= m.Hp) return - m.libramofwisdom * 10;
            //     return - m.libramofwisdom * 5;
            // }
            // 鼓励圣盾攻击
            // if(m.divineshild && !target.isHero && target.Angr > 2){
            //     return -10;
            // }
            // 保留，别送
            switch (m.handcard.card.卡名)
            {
                case "年轻的女祭司":
                case "飞刀杂耍者":
                case "恐狼前锋":
                    if(!target.isHero) return 5;
                    break;
            }
            //不要主动解亡语怪
            if(target.handcard.card.deathrattle){
                return 20;
            }
            return 0;
        }

        public override int GetSpecialCardComboPenalty(CardDB.Card card, Minion target, Playfield p)
        {
            // 初始惩罚值
            int pen = 0;      
            bool found = false;
            // 飞刀在场上优先下怪
            foreach (Minion m in p.ownMinions){
                if( m.handcard.card.卡名 == "飞刀杂耍者" && card.type == CardDB.cardtype.MOB){
                    pen --;
                }
            }
            switch (card.卡名)
            {
                case "幸运币":
                    return 5;
                //-----------------------------超模补正-----------------------------------
                // case "烈焰小鬼":
                //     return -5;
                // 亡语补正
                case "麦田傀儡":
                    pen -= 5;
                    break;
                case "灵魂缠绕":
                    if(target.Hp > 1)
                        pen += 5;
                    break;                        
                //-----------------------------武器-----------------------------------
                
                //-----------------------------针对卡---------------------------------
                //-----------------------------配合-----------------------------------
                case "年轻的女祭司":
                    if( p.enemyMinions.Count == 0 && p.ownMinions.Count > 0 ) pen -= 10;
                    if( p.ownMinions.Count < 1 && p.ownMaxMana < 2) pen += 5;
                    break;
                case "飞刀杂耍者":   
                    if( p.ownMaxMana < 3 && p.enemyMinions.Count == 0 ) pen -= 20;
                    // 白送
                    else if(p.ownMinions.Count == 0 && p.enemyMinions.Count > 0 && p.mana == 2) pen += 10;
                    break; 
                case "末日守卫":  
                    if(p.owncards.Count == 1) pen -= 20;
                    else if(p.owncards.Count == 2) pen -= 10;
                    else pen += 20;
                    break; 
                case "灵魂之火":   
                    if( p.owncards.Count <= 1 ) pen -= 10;
                    else if( !target.isHero && target.Hp >= 3 ) ;
                    else pen += 20;
                    break; 
                //-----------------------------buff-----------------------------------
                case "阿古斯防御者":
                    if( p.ownMinions.Count > 1) pen -= 5;
                    break;
                
                //-----------------------------英雄技能-----------------------------------
                case "生命分流":
                    pen -= 3;
                    break;
            }
            return pen;
        }           
    }
}