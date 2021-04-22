using System.Collections.Generic;

namespace HREngine.Bots
{
    public class Behavior骑士 : Behavior
    {
        public override string BehaviorName() { return "骑士"; }
        
        private static int costVal = 10;
        // 抽牌价值
        private static int drawCardVal = 10;
        

        PenalityManager penman = PenalityManager.Instance;

        // 核心，场面值
        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value >= -2000000) return p.value;
            // 场面值
            int retval = 0;
            // 危险血线
            int hpboarder = 10;
            // 抢脸血线
            int aggroboarder = 25;
            // --------------------------卡组特性---------------------------------
            // 圣契奖励
            retval += p.libram > 2 ? 60 + p.libram * 10 : p.libram * 30;

            // --------------------------通用---------------------------------
            // 计算额外惩罚
            retval -= p.evaluatePenality;
            // 手牌价值（可能导致不出牌）
            // retval += p.owncards.Count * 5;

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
                retval += p.owncarddraw * 5;
            }
            // 卡差
            // if (p.owncarddraw + 1 >= p.enemycarddraw) retval -= p.enemycarddraw * 7;
            // else retval -= (p.owncarddraw + 1) * 7 + (p.enemycarddraw - p.owncarddraw - 1) * 12;

            // 计算我方随从价值
            //int owntaunt = 0;
            int readycount = 0;
            int ownMinionsCount = 0;
            foreach (Minion m in p.ownMinions)
            {
                retval += 5;
                retval += m.Hp * 2;
                if(!m.cantAttack || !m.Ready){
                    retval += m.Angr * 2;
                }else {
                    retval += m.Angr / 2;
                }
                // 稀有度
                // retval += m.handcard.card.rarity;

                // 智慧圣契掉落
                retval -= m.libramofwisdom * 2;

                // 保留
                // if(m.handcard.card.卡名 == "前沿哨所" || m.handcard.card.卡名 == "螃蟹骑士"){
                //     retval += 10;
                // }
                // 风怒价值
                if ((!m.playedThisTurn || m.rush == 1 || m.charge == 1 )  && m.windfury) retval += m.Angr;
                // 圣盾价值
                if (m.divineshild) retval += m.Angr / 2 + 1;
                // 潜行价值
                if (m.stealth) retval += m.Angr / 3 + 1;
                // 特殊随从价值
                // if (m.handcard.card.isSpecialMinion && !m.silenced)
                // {
                //     retval += 1;
                //     if (!m.taunt && m.stealth) retval += 20;
                // }
                // else
                // {
                //     if (m.Angr <= 2 && m.Hp <= 2 && !m.divineshild) retval -= 5;
                // }
                //if (!m.taunt && m.stealth && penman.specialMinions.ContainsKey(m.name)) retval += 20;
                //if (m.poisonous) retval += 1;
                // 吸血
                if (m.lifesteal) retval += m.Angr / 3 + 1;
                // 圣盾嘲讽
                if (m.divineshild && m.taunt) retval += 4;
                //if (m.taunt && m.handcard.card.name == CardDB.cardName.frog) owntaunt++;
                //if (m.handcard.card.isToken && m.Angr <= 2 && m.Hp <= 2) retval -= 5;
                //if (!penman.specialMinions.ContainsKey(m.name) && m.Angr <= 2 && m.Hp <= 2) retval -= 5;
                // if (p.ownMinions.Count > 2 && (m.handcard.card.name == CardDB.cardName.direwolfalpha || m.handcard.card.name == CardDB.cardName.flametonguetotem || m.handcard.card.name == CardDB.cardName.stormwindchampion || m.handcard.card.name == CardDB.cardName.raidleader)) retval += 10;
                // if (m.handcard.card.name == CardDB.cardName.bloodmagethalnos) retval += 10;
                // if (m.handcard.card.name == CardDB.cardName.nerubianegg)
                // {
                //     if (m.Angr >= 1) retval += 2;
                //     if ((!m.taunt && m.Angr == 0) && (m.divineshild || m.maxHp > 2)) retval -= 10;
                // }
                // if (m.Ready) readycount++;
                if (m.Hp <= 4 && (m.Angr > 2 || m.Hp > 3)) ownMinionsCount++;
                retval += m.synergy;
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
                switch (a.card.card.卡名)
                {
                    // 排序优先
                    case "凯瑞尔·罗姆":
                        if(i == 0) retval += 10;
                        break;
                    case "雷霆绽放":
                        usecoin+=2;
                        if (i == count - 1) retval -= 100;
                        break;
                    case "激活":
                    case "幸运币":
                        usecoin++;
                        // 最后一张卡
                        if (i == count - 1) retval -= 10;
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
                            retval -= 10;
                            if (p.ownHero.immune) retval-= 5;
                        }
                        break;
                    default:
                        retval -= 10;
                        break;
                }
            }
            // if (usecoin && p.mana >= 1) retval -= 20;
            // 尽量耗尽法力
            retval -= p.mana;
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

            if (ownMinionsCount - p.enemyMinions.Count >= 4 && bigMobsInHand >= 1)
            {
                retval += bigMobsInHand * 25;
            }

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


        // 敌方随从价值
        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            int retval = 5;
            retval += m.Hp * 2;
            if (!m.frozen && !(m.cantAttack && m.name != CardDB.cardName.argentwatchman))
            {
                retval += m.Angr * 2;
                if (m.windfury) retval += m.Angr * 2;
                if (m.Angr >= 4) retval += 10;
                if (m.Angr >= 7) retval += 50;
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
                // 解不掉基本 gg
                case "科卡尔驯犬者":
                case "巡游领队":
                case "螃蟹骑士":
                case "团队核心":
                case "前沿哨所":
                case "莫尔杉哨所":
                case "塔姆辛·罗姆":
                case "导师火心":
                case "伊纳拉·碎雷":
                case "暗影珠宝师汉纳尔":
                case "伦萨克大王":
                case "洛卡拉":
                    retval += 100;
                    break;
                // 我劝你最好解了
                case "甩笔侏儒":
                case "聒噪怪":
                case "精英牛头人酋长，金属之神":
                    retval += 50;
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
            if(m.divineshild && !target.isHero && target.Angr > 2){
                return -10;
            }
            // switch (m.handcard.card.卡名)
            // {
            //     case "螃蟹骑士":
            //         if(target.isHero) return -20;
            //         return 20;
            // }
            // 不要主动解亡语怪
            if(target.handcard.card.deathrattle){
                return 20;
            }
            return 0;
        }

        public override int GetSpecialCardComboPenalty(CardDB.Card card, Minion target, Playfield p)
        {
            // 初始惩罚值
            int pen = 0;      
            // 场上能打脸的随从数量
            int canAttackHeroCount = 0;
            foreach(Minion m in p.ownMinions)
            {
                if(m.Ready && !m.cantAttackHeroes && !m.frozen && !m.cantAttack ) {
                    canAttackHeroCount = canAttackHeroCount + 1;
                    break;
                }
            }      
            switch (card.卡名)
            {
                //-----------------------------超模补正-----------------------------------
                case "曼科里克":
                case "莫戈尔·莫戈尔格":
                    return -10;
                case "双盾优等生":
                case "纳鲁之锤":
                    return -20;
                case "凯瑞尔·罗姆":
                case "终极莫戈尔格":
                case "新生入学":
                    return -50;
                //-----------------------------武器-----------------------------------
                case "逝者之剑":
                    if(p.ownWeapon.Durability > 0 ){
                        return 20;
                    }
                    // 脸被冻住了就别挂刀了
                    if(p.ownHero.frozen || p.ownSecretsIDList.Count > 2) return 20;
                    return  -50;
                case "正义圣契":
                    pen = 5;
                    foreach (Minion m in p.enemyMinions){
                        pen -= m.Hp;
                    }
                    pen *= 10;
                    break;
                    return -60;     
                //-----------------------------针对卡-----------------------------------
                case "异教低阶牧师":
                    if(p.enemyHeroStartClass == TAG_CLASS.MAGE || p.enemyHeroStartClass == TAG_CLASS.PRIEST ) return -20;
                    return 0;
                case "食人魔巫术师":
                    if(p.enemyMinions.Count == 0 && p.ownMinions.Count < 4)
                        return -100;
                    // TODO 敌方的法术数量大于随从数量则优先
                    return -30;
                case "古神在上":
                    // 前期一律不出留着
                    if( p.enemyMaxMana < 4 ) return 100;
                    // TODO 对手没出硬币，一律不出
                    
                    // 随从少/对手没牌不用防
                    if(p.ownMinions.Count < 2 || p.tempanzEnemyCards < 4) return 20;
                    // 随从越多越需要优先打出
                    return -20 * (p.ownMinions.Count - 2) - (p.tempanzEnemyCards - 2) * 10;   
                

                //-----------------------------配合-----------------------------------
                case "十字路口大嘴巴":
                    return -10 * p.ownSecretsIDList.Count;
                case "夺日者间谍":
                    return p.ownSecretsIDList.Count > 0 ? -5 : 1;
                case "银色自大狂":
                    foreach (Handmanager.Handcard hc in p.owncards){
                        if(hc.card.卡名 == "威能祝福"){
                            return 30;
                        }
                    }
                    break;
                case "前沿哨所":
                    // 前两个回合优先下，赌对面解不掉
                    if( p.ownMaxMana <= 2 ) return -100;
                    if ( p.enemyHeroStartClass == TAG_CLASS.MAGE || p.enemyHeroStartClass == TAG_CLASS.PRIEST )
                        return -15;   
                    return -10;
                case "防护长袍":
                    if ( p.enemyHeroStartClass == TAG_CLASS.MAGE || p.enemyHeroStartClass == TAG_CLASS.PRIEST )
                        return -30;   
                    return 0;
                case "火炮长斯密瑟":
                    return -20 * p.ownSecretsIDList.Count;  
                case "北卫军指挥官":
                    return p.ownSecretsIDList.Count > 0 ? -20 : 1;
                case "复仇":
                    return -1;            

                //-----------------------------buff-----------------------------------
                case "威能祝福":
                // 需要对冲打脸奖励
                    if(target.own) return -60;
                    break;
                case "阿达尔之手":
                case "王者祝福":
                case "智慧圣契":
                    if (target.handcard.card.卡名 == "甩笔侏儒" && !target.silenced) return 1000;
                    if (target.own && target.windfury ) return -30;
                    if (target.own && (target.frozen || target.cantAttack) )  return 20; 
                    return -10;
                
                case "定罪（等级1）":
                    if(canAttackHeroCount > 0){
                        return 20;
                    }
                    return 100;
                case "定罪（等级2）":
                    if(canAttackHeroCount > 1){
                        return -30;
                    }else if(canAttackHeroCount > 0){
                        return 20;
                    }
                    return 100;
                case "定罪（等级3）":
                    if(canAttackHeroCount > 2){
                        return -100;
                    }else if(canAttackHeroCount == 2){
                        return -30;
                    }else if(canAttackHeroCount > 0){
                        return 20;
                    }
                    return 100;     
                //-----------------------------英雄技能-----------------------------------
                case "援军":
                    bool findMinion = false;
                    foreach(Handmanager.Handcard hc in p.owncards)
                    {
                        if(hc.card.type == CardDB.cardtype.MOB && hc.card.cost <= card.cost 
                        && hc.card.卡名 != "银色自大狂" && hc.card.卡名 != "银色保卫者") findMinion = true;
                    }
                    if(!findMinion) return -3;
                    return 1;
            }
            return pen;
        }           
    }
}