using System.Collections.Generic;
using System;

namespace HREngine.Bots
{
    public class Behavior狂野鱼人萨 : Behavior
    {
        public override string BehaviorName() { return "狂野鱼人萨"; }
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
            else if(p.enemyHero.Hp + p.enemyHero.armor <= aggroboarder && p.enemyHero.Hp + p.enemyHero.armor > aggroboarder / 2 )
            {
                retval += 4 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor);
            }
            else {
                retval += 6 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor);
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
                retval += p.owncarddraw * 5;
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
            // int deletecardsAtLast = 0;
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
                        case "甜水鱼人斥候":
                        case "鱼人招潮者":
                        case "下水道渔人":
                        case "火焰术士弗洛格尔":
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
            if (p.manaTurnEnd >= p.ownHeroAblility.manacost && !useAbili && p.ownAbilityReady)
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
            // retval -= p.manaTurnEnd;
            // if(p.manaTurnEnd == 1 && !useAbili && p.ownAbilityReady){
            //     retval -= 10;
            // }
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
            if (!m.frozen && !(m.cantAttack && m.name != CardDB.cardName.argentwatchman))
            {
                retval += m.Angr * 2;
                if (m.windfury) retval += m.Angr * 2;
                if (m.Angr > 4) retval += 20;
                if (m.Angr > 7) retval += 20;                
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
                case "巫师学徒":
                case "塔姆辛·罗姆":
                case "导师火心":
                case "伊纳拉·碎雷":
                case "暗影珠宝师汉纳尔":
                case "伦萨克大王":
                case "洛卡拉":
                case "火焰术士弗洛格尔":
                case "布莱恩·铜须":
                case "观星者露娜":
                case "大法师瓦格斯":
                case "火妖":
                case "下水道渔人":
                case "对空奥术法师":
                case "空中炮艇":
                case "船载火炮":
                    retval += 100;
                    break;
                // 不解巨大劣势
                case "甩笔侏儒":
                case "聒噪怪":
                case "精英牛头人酋长，金属之神":
                case "科卡尔驯犬者":
                case "巡游领队":
                case "团队核心":
                case "螃蟹骑士":
                case "前沿哨所":
                case "莫尔杉哨所":
                case "凯瑞尔·罗姆":
                case "鱼人领军":
                case "南海船长":
                case "末日预言者":
                case "坎雷萨德·埃伯洛克":
                case "人偶大师多里安":
                case "甜水鱼人斥候":
                case "暗鳞先知":
                case "灭龙弩炮":
                case "神秘女猎手":
                case "鲨鳍后援":
                case "怪盗图腾":
                case "矮人神射手":
                case "任务达人":
                case "贪婪的书虫":
                case "战马训练师":
                case "相位追猎者":
                case "鱼人宝宝车队":
                case "科多兽骑手":
                case "奥秘守护者":
                    retval += 30;
                    break;
                // 算有点用
                case "战斗邪犬":
                case "低阶侍从":

                    retval += 10;
                    break;
                // 带点异能随手解一下吧
                case "锈水海盗":
                    retval += 3;
                    break;
            }
            return retval;
        }

        public override int getMyMinionValue(Minion m, Playfield p)
        {
            int retval = 2;
            retval += m.Hp * 2;
            if(!m.cantAttack || !m.Ready || !m.frozen){
                retval += m.Angr ;
            }else {
                retval += m.Angr / 4;
            }
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
            // retval += m.synergy;
            // retval += getSpeicalCardVal(m.handcard.card);
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
            { CardDB.cardName.demonclaws, 10 },
            { CardDB.cardName.daggermastery, 5 },
            { CardDB.cardName.reinforce, 4 },
            { CardDB.cardName.armorup, 2 },
            { CardDB.cardName.steadyshot, 8 }
        };

        public override int getAttackWithMininonPenality(Minion m, Playfield p, Minion target)
        {
            // 鼓励拥有智慧圣契的随从送死
            // if(m.libramofwisdom > 0 && !target.isH
            //     if(target.Angr >= m.Hp) return - m.libramofwisdom * 10;
            //     return - m.libramofwisdom * 5;
            // }
            // 鼓励圣盾攻击
            // if(m.divineshild && !target.isHero && target.Angr > 2){
            //     return -10;
            // }
            // 保留，别送
            int pen = 0;
            switch (m.handcard.card.卡名)
            {
                case "甜水鱼人斥候":
                case "火焰术士弗洛格尔":
                case "鱼人领军":
                case "暗鳞先知":
                case "下水道渔人":
                    if(!target.isHero) pen += 10;
                    break;
            }
            //不要主动解亡语怪
            if(target.handcard.card.deathrattle){
                pen += 10;
            }
            // 剧毒
            if( m.poisonous && m.handcard.card.卡名 != "火焰术士弗洛格尔" && !target.isHero ){
                pen -= target.Angr * target.Hp;
            }
            // 嫖一个圣盾
            if(target.divineshild && target.Angr == 1){
                pen -= 5;
            }
            // 能击杀
            if(!target.divineshild && target.Hp == m.Angr && target.Hp > 3){
                if(m.divineshild || m.Hp > target.Angr || (m.Hp < 3 && target.Angr > 3)){
                    pen -= target.Hp * 3;
                }
            }
            return pen;
        }

        public int getSpeicalCardVal(CardDB.Card card){
            switch (card.卡名)
            {
                case "甜水鱼人斥候":
                case "火焰术士弗洛格尔":
                case "鱼人领军":
                case "暗鳞先知":
                case "下水道渔人":
                    return 20;
            }
            return 0;
        }

        public override int GetSpecialCardComboPenalty(CardDB.Card card, Minion target, Playfield p)
        {
            // 初始惩罚值
            int pen = 0;      
            // pen += getSpeicalCardVal(card);
            switch (card.卡名)
            {
                case "下水道渔人":
                    if(p.enemyMinions.Count == 0) pen -= 10;
                    pen -= 2;
                    break;
                case "火焰术士弗洛格尔":
                    pen -= 2;
                    break;
                case "幸运币":
                    if(p.ownMaxMana > 1) break;
                    int cost1 = 0;
                    int cost2 = 0;
                    foreach(Handmanager.Handcard hc in p.owncards){
                        if(hc.card.cost == 1) cost1 ++;
                        else if(hc.card.cost == 2) cost2 ++;
                    }
                    if(cost2 == 0 || ( cost1 == 2 || cost1 == 3 )  ) pen += 20;
                    if(cost2 == 1 && cost1 < 2) pen += 20;
                    break;
                case "石塘猎人":
                    if(target == null) pen += 2;
                    break;
                case "冰钓术":
                    if(p.owncards.Count > 3) pen += 20;
                    break;
                case "南海岸酋长":
                    if(p.ownMinions.Count == 0) pen += 5;
                    break;
                case "寒光先知":
                case "鱼人领军":
                case "暗鳞先知":
                    if(p.ownMinions.Count == 0) pen += 20;
                    if(p.ownMinions.Count < 3) pen += 10;
                    pen -= p.ownMinions.Count * 2;
                    break;
                case "鱼人招潮者":
                    pen -= 5;
                    break;
                case "毒鳍鱼人":
                    if(target != null && target.handcard.card.卡名 == "火焰术士弗洛格尔") pen -= 30;
                    else pen += 10;
                    break;
                case "鱼人恩典":
                    if(p.ownMinions.Count >=4) pen -= 10 * p.ownMinions.Count;
                    break;
                case "甜水鱼人斥候":
                    pen -=10;
                    break;
                case "鱼勇可贾":
                    if(p.ownMinions.Count == 0) pen += 100;
                    if(p.ownMinions.Count < 3) pen += 10;
                    pen -= 2 * p.ownMinions.Count;
                    break;
                case "图腾召唤":
                    pen += 1;
                    break;
            }
            if(TAG_RACE.MURLOC == (TAG_RACE)card.race) pen -=1;
            return pen;
        }           
    }
}


