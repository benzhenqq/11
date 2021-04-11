namespace HREngine.Bots
{
    using System;
    using System.Collections.Generic;
    public class Behavior封印骑 : Behavior
    {
        public override string BehaviorName() { return "封印骑"; }

        PenalityManager penman = PenalityManager.Instance;

        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value >= -2000000) return p.value;
            int retval = 0;
            retval -= p.evaluatePenality;
            retval += p.owncards.Count * 3;
            retval += p.ownQuest.questProgress * 10;

            retval += p.ownHero.Hp + p.ownHero.armor;
            retval += -(p.enemyHero.Hp + p.enemyHero.armor);

            retval += p.ownMaxMana * 15 - p.enemyMaxMana * 15;

            if (p.ownHeroPowerAllowedQuantity != p.enemyHeroPowerAllowedQuantity)
            {
                if (p.ownHeroPowerAllowedQuantity > p.enemyHeroPowerAllowedQuantity) retval += 1;
                else retval -= 4;
            }

            if (p.ownWeapon.Angr >= 1)
            {
                retval += p.ownWeapon.Angr * p.ownWeapon.Durability;
            }

            if (!p.enemyHero.frozen)
            {
                retval -= p.enemyWeapon.Durability * p.enemyWeapon.Angr;
            }
            else
            {
                if (p.enemyHeroName != HeroEnum.mage && p.enemyHeroName != HeroEnum.priest)
                {
                    retval += 11;
                }
            }

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
            retval += p.owncarddraw * 5;
            if (p.owncarddraw + 1 >= p.enemycarddraw) retval -= p.enemycarddraw * 7;
            else retval -= (p.owncarddraw + 1) * 7 + (p.enemycarddraw - p.owncarddraw - 1) * 12;

            bool useAbili = false;
            int usecoin = 0;
            foreach (Action a in p.playactions)
            {
                if (a.actionType == actionEnum.attackWithHero && p.enemyHero.Hp <= p.attackFaceHP) retval++;
                if (a.actionType == actionEnum.useHeroPower) useAbili = true;
                if (p.ownHeroName == HeroEnum.warrior && a.actionType == actionEnum.attackWithHero && useAbili) retval -= 1;
                //if (a.actionType == actionEnum.useHeroPower && a.card.card.name == CardDB.cardName.lesserheal && (!a.target.own)) retval -= 5;
                if (a.actionType != actionEnum.playcard) continue;
                if (a.card.card.name == CardDB.cardName.thecoin || a.card.card.name == CardDB.cardName.innervate) usecoin++;

                if(a.card.card.type == CardDB.cardtype.SPELL && a.card.card.name == CardDB.cardName.fireball && (a.target.isHero && !a.target.own)) retval += 2;
                if (a.card.card.type == CardDB.cardtype.SPELL && a.card.card.name == CardDB.cardName.roaringtorch && (a.target.isHero && !a.target.own)) retval += 2;
                if(a.card.card.Secret) retval += 5;
            }
            if (usecoin > 0)
            {
                if (useAbili && p.ownMaxMana <= 2) retval -= 40;
                retval -= 5 * p.manaTurnEnd;
                if (p.manaTurnEnd + usecoin > 10) retval -= 5 * usecoin;
            }
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
                                if (m.wounded) { wereTarget = true; break; }
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
                            if (p.ownHero.immune) retval -= 5;
                        }
                        break;
                    default:
                        retval -= 10;
                        break;
                }
            }
            //if (usecoin && p.mana >= 1) retval -= 20;

            foreach (Minion m in p.ownMinions)
            {
                retval += m.Hp * 1;
                retval += m.Angr * 2;
                retval += m.handcard.card.rarity;
                if (m.windfury) retval += m.Angr;
                if (m.taunt) retval += 1;
                if (!m.taunt && m.stealth && m.handcard.card.isSpecialMinion && !m.silenced) retval += 20;
                if (m.handcard.card.name == CardDB.cardName.silverhandrecruit && m.Angr == 1 && m.Hp == 1) retval -= 5;
                if (p.ownMinions.Count > 1 && (m.handcard.card.name == CardDB.cardName.direwolfalpha || m.handcard.card.name == CardDB.cardName.flametonguetotem || m.handcard.card.name == CardDB.cardName.stormwindchampion || m.handcard.card.name == CardDB.cardName.raidleader || m.handcard.card.name == CardDB.cardName.fallenhero)) retval += 10;
                if (m.handcard.card.name == CardDB.cardName.nerubianegg)
                {
                    if (m.Angr >= 1) retval += 2;
                    if ((!m.taunt && m.Angr == 0) && (m.divineshild || m.maxHp > 2)) retval -= 10;
                }
                retval += m.synergy;
            }

            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if (hc.card.type == CardDB.cardtype.MOB)
                {
                    retval += hc.addattack + hc.addHp + hc.elemPoweredUp;
                }
            }

            int tmp = 0;
            foreach (Minion m in p.enemyMinions)
            {
                tmp = this.getEnemyMinionValue(m, p);
            }
            if (p.enemyMinions.Count == 1) tmp /= 2;
            retval -= tmp;

            retval -= p.enemySecretCount;
            retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2
            retval -= p.lostWeaponDamage;
            if (p.ownMinions.Count == 0) retval -= 20;
            if (p.enemyMinions.Count >= 4) retval -= 20;
            if (p.enemyHero.Hp <= 0) retval = 10000;
            
            retval += p.anzOwnExtraAngrHp - p.anzEnemyExtraAngrHp / 2;
            //soulfire etc
            int deletecardsAtLast = 0;
            foreach (Action a in p.playactions)
            {
                if (a.actionType != actionEnum.playcard) continue;
                if (a.card.card.name == CardDB.cardName.soulfire || a.card.card.name == CardDB.cardName.doomguard || a.card.card.name == CardDB.cardName.queenofpain) deletecardsAtLast = 1;
                if (deletecardsAtLast == 1 && !(a.card.card.name == CardDB.cardName.soulfire || a.card.card.name == CardDB.cardName.doomguard || a.card.card.name == CardDB.cardName.queenofpain)) retval -= 20;
            }
            if (p.enemyHero.Hp >= 1 && p.guessingHeroHP <= 0)
            {
                if (p.turnCounter < 2) retval += p.owncarddraw * 500;
                retval -= 1000;
            }
            if (p.ownHero.Hp <= 0) retval -= 10000;

            p.value = retval;
            return retval;
        }

        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            int retval = 0;
            // 骑士随从加权
            if (p.enemyHeroName == HeroEnum.pala) retval += 10;
            if (p.enemyMinions.Count >= 4 || m.taunt || (m.handcard.card.targetPriority >= 1 && !m.silenced) || m.Angr >= 5)
            {
                retval += m.Hp;
                if (!m.frozen && !(m.cantAttack && m.name != CardDB.cardName.argentwatchman))
                {
                    retval += m.Angr * 2;
                    if (m.windfury) retval += 2 * m.Angr;
                }
                if (m.taunt) retval += 5;
                if (m.divineshild) retval += m.Angr;
                if (m.frozen) retval -= 1; // because its bad for enemy :D
                if (m.poisonous)
                {
                    retval += 4;
                    if (p.ownMinions.Count < p.enemyMinions.Count) retval += 10;
                }
                if (m.lifesteal) retval += m.Angr;
                if (m.handcard.card.isSpecialMinion) retval += m.handcard.card.rarity;
            }

            retval += m.synergy;
            if (m.handcard.card.targetPriority >= 1 && !m.silenced) retval += m.handcard.card.targetPriority;
            if (m.Angr >= 4) retval += 20;
            if (m.Angr >= 7) retval += 50;
            if (m.name == CardDB.cardName.nerubianegg && m.Angr <= 3 && !m.taunt) retval = 0;
            return retval;
        }

        public override int GetSpecialCardComboPenalty(CardDB.Card card, Minion target, Playfield p)
        {
            //在这里写你的惩罚，格式和penman一样
            // NMD实在打不过牧师，加个针对策略吧，对阵牧师别下太多怪（再上4个我就是狗）
            bool counterPriest = (p.enemyHeroStartClass == TAG_CLASS.PRIEST) && (p.ownMinions.Count > 2) && (p.ownMaxMana <= 8);
            // 场上能打脸的随从数量
            int canAttackHeroCount = 0;
            foreach(Minion m in p.ownMinions)
            {
                if(!m.cantAttackHeroes && !m.frozen && !m.cantAttack) {
                    canAttackHeroCount = canAttackHeroCount + 1;
                    break;
                }
            }
            // 特定卡牌惩罚值修改策略
            //-------------------------------------------封印骑----------------------------------------------                
            switch (card.name)
            {
                //神恩术 divinefavor
                case CardDB.cardName.divinefavor:
                    // 剩余卡牌不足，停止抽卡
                    if (p.prozis.ownDeckSize < 5) return 100;
                    // 抽牌越多优先度越高
                    return (p.tempanzEnemyCards - p.owncards.Count ) * (-3);
                // 战斗号角
                case CardDB.cardName.calltoarms:
                    // 判断牌库中2费以下随从数量
                    int count = 0;
                    foreach(KeyValuePair<CardDB.cardIDEnum, int>kvp in p.prozis.turnDeck )
                    {
                        CardDB.cardIDEnum deckCard = kvp.Key;
                        int cnt = kvp.Value;
                        switch (deckCard)
                        {
                            case CardDB.cardIDEnum.YOP_031:
                            case CardDB.cardIDEnum.GVG_058:
                            case CardDB.cardIDEnum.ICC_038:
                            case CardDB.cardIDEnum.FP1_017:
                            case CardDB.cardIDEnum.BAR_074:
                                count = count + cnt;
                                break;
                        }
                    }
                    if (count < 2) return 100;
                    if (count < 3) return 20;
                    
                    if(p.ownMinions.Count == 0) return -200;
                    // 防牧师的AOE
                    if(p.enemyHeroStartClass == TAG_CLASS.PRIEST) return 10;
                    
                    // 随从越少越推荐使用
                    if(p.ownMinions.Count < 2) return -150;
                    else if(p.ownMinions.Count < 3) return -100;
                    else if(p.ownMinions.Count == 3) return -20;
                    return  (p.ownMinions.Count - 4) * 20;
                // 逝者之剑
                case CardDB.cardName.swordofthefallen:
                    // 开局第二回合优先上随从
                    if(p.ownMaxMana <=2 ){
                        foreach(Handmanager.Handcard hc in p.owncards)
                        {
                            CardDB.Card m = hc.card;
                            if(m.cost == 2) return 0;
                        }
                    }
                    // 已经装备一把了就别挂了
                    if(p.ownWeapon.Durability > 0){
                        return 20;
                    }
                    // 脸被冻住了就别挂刀了
                    if(p.ownHero.frozen) return 10;
                    // 我就三个奥秘，都挂上了就别浪费法力水晶了
                    if(p.ownSecretsIDList.Count > 2) return 10;
                    return  -80;
                // 英勇圣印
                case CardDB.cardName.sealofchampions:
                    if (target == null) return 1000;
                    // 剩余卡牌不足，停止抽卡
                    // if (p.prozis.ownDeckSize < 5) return 10;
                    if (!target.own) return 1000;
                    // 冻结别 buff
                    if (target.own && target.frozen)
                    {
                        return 20;
                    }
                    // 尽量 buff 螃蟹
                    if (target.own && target.windfury )
                    {
                        return -100;
                    }
                    // 尽量别buff不会动的哨所
                    if (target.own && target.cantAttack)
                    {
                        return 20;
                    }
                    // 尽量别buff不会动的哨所
                    if (target.own && target.divineshild)
                    {
                        return 0;
                    }
                    if (p.ownMinions.Count >= 1) return -10;
                    return 10;
                // 王者祝福
                case CardDB.cardName.blessingofkings:
                    if (target == null) return 1000;
                    if (!target.own) return 1000;
                    // 尽量 buff 螃蟹
                    if (target.own && target.windfury)
                    {
                        return -120;
                    }
                    // 冻结别 buff
                    if (target.own && target.frozen)
                    {
                        return 20;
                    }
                    // 尽量别buff不会动的哨所
                    if (target.own && target.cantAttack && p.enemyHeroStartClass != TAG_CLASS.MAGE) 
                    {
                        return 20;
                    }
                    if (p.ownMinions.Count >= 1) return -10;
                    return 10;
                // 阿达尔之手
                case CardDB.cardName.handofadal:
                    if (target == null) return 1000;
                    if (!target.own) return 1000;
                    // 剩余卡牌不足，停止抽卡
                    if (p.prozis.ownDeckSize < 5) return 10;
                    // 尽量 buff 螃蟹
                    if (target.own && target.windfury && !target.cantAttackHeroes)
                    {
                        return -50;
                    }
                    // 冻结别 buff
                    if (target.own && target.frozen)
                    {
                        return 20;
                    }
                    // 尽量别buff不会动的哨所/法师除外
                    if (target.own && target.cantAttack && p.enemyHeroStartClass != TAG_CLASS.MAGE)
                    {
                        return 20;
                    }
                    if (p.ownMinions.Count >= 1) return -5;
                    return 10;
                // 古神在上
                case CardDB.cardName.ohmyyogg:
                    // 前期一律不出留着
                    if( p.enemyMaxMana < 4 ) return 100;
                    // 随从少/对手没牌不用防
                    if(p.ownMinions.Count < 2 || p.tempanzEnemyCards < 4) return 10;
                    // 随从越多越需要优先打出
                    return -20 * (p.ownMinions.Count - 2) - (p.tempanzEnemyCards - 2) * 10;
                // 尼鲁巴蛛
                case CardDB.cardName.nerubarweblord:
                    if(counterPriest) return 20;
                    // 对阵法师优先度提高，对阵骑士优先度降低
                    // if(p.enemyHeroStartClass == TAG_CLASS.MAGE) return -10;
                    // 前两个回合优先下，赌对面解不掉
                    if( p.ownMaxMana <= 2 && p.enemyHeroStartClass == TAG_CLASS.MAGE) return -80;
                    if(p.enemyHeroStartClass == TAG_CLASS.PALADIN) return 0;     
                    if ( p.enemyHeroStartClass == TAG_CLASS.MAGE)
                        return -15;           
                    return -6;
                // 智慧祝福
                case CardDB.cardName.blessingofwisdom:
                    if (target == null) return 1000;
                    // 剩余卡牌不足，停止抽卡
                    if (p.prozis.ownDeckSize < 5) return 10;
                    // 冻结别 buff
                    if (target.own && target.frozen)
                    {
                        return 20;
                    }
                    // 尽量别buff不会动的哨所
                    if (target.own && target.cantAttack)
                    {
                        return 100;
                    }
                    return -5;
                // 前沿哨所
                case CardDB.cardName.farwatchpost:
                    // 前两个回合优先下，赌对面解不掉
                    if( p.ownMaxMana <= 2 ) return -100;
                    if ( p.enemyHeroStartClass == TAG_CLASS.MAGE || p.enemyHeroStartClass == TAG_CLASS.PRIEST )
                        return -15;   
                    return -10;
                // 水晶学
                case CardDB.cardName.crystology:
                    // 判断牌库中1攻随从数量
                    int deckCountAtk1 = 0;
                    foreach(KeyValuePair<CardDB.cardIDEnum, int>kvp in p.prozis.turnDeck )
                    {
                        CardDB.cardIDEnum deckCard = kvp.Key;
                        int cnt = kvp.Value;
                        switch (deckCard)
                        {
                            case CardDB.cardIDEnum.YOP_031:
                            case CardDB.cardIDEnum.ICC_038:
                            case CardDB.cardIDEnum.FP1_017:
                                deckCountAtk1 = deckCountAtk1 + cnt;
                                break;
                        }
                    }
                    if (deckCountAtk1 < 1) return 100;
                    if (deckCountAtk1 < 2) return 20;
                    return (p.owncards.Count - 7) * 4;
                // 螃蟹骑士
                case CardDB.cardName.crabrider:
                    if(counterPriest) return 20;
                    // 手中有buff卡,并且费用足够,并且场上没有能动的风怒随从
                    bool foundBuff = false;
                    foreach(Handmanager.Handcard hc in p.owncards)
                    {
                        CardDB.Card m = hc.card;
                        switch(m.name){
                            case CardDB.cardName.blessingofkings: 
                            case CardDB.cardName.sealofchampions: 
                            case CardDB.cardName.handofadal:
                                if( p.mana >= (hc.card.cost + card.cost) ) foundBuff = true;
                                break;
                        }
                    }
                    foreach(Minion m in p.ownMinions)
                    {
                        if(m.windfury) {
                            foundBuff = false;
                            break;
                        }
                    }
                    
                    if(foundBuff) return -50;
                    return -4;
                // 护盾机器人
                case CardDB.cardName.shieldedminibot:
                    if(counterPriest) return 20;
                    return -5;
                // 正义保护者
                case CardDB.cardName.righteousprotector:
                    if(counterPriest) return 20;
                    return -5;
                // 救赎
                case CardDB.cardName.redemption:
                    return -5;
                // 永不屈服
                case CardDB.cardName.neversurrender:
                    return -5;      
                // 复仇
                case CardDB.cardName.avenge:
                    return -5;      
                // 定罪1
                case CardDB.cardName.convictionrank1:
                    if(canAttackHeroCount > 0){
                        return 1;
                    }
                    return 100;
                // 定罪2
                case CardDB.cardName.convictionrank2:
                    if(canAttackHeroCount > 1){
                        return -30;
                    }else if(canAttackHeroCount > 0){
                        return 1;
                    }
                    return 100;
                // 定罪3
                case CardDB.cardName.convictionrank3:
                    if(canAttackHeroCount > 2){
                        return -100;
                    }else if(canAttackHeroCount == 2){
                        return -30;
                    }else if(canAttackHeroCount > 0){
                        return 1;
                    }
                    return 100;
                // 责难
                case CardDB.cardName.rebuke:
                    // 前期一律不出留着
                    if( p.enemyMaxMana < 4 ) return 100;
                    // 随从少/对手没牌不用防
                    if(p.ownMinions.Count < 2 || p.tempanzEnemyCards < 4) return 10;
                    // 随从越多越需要优先打出
                    return -20 * (p.ownMinions.Count - 2) - (p.tempanzEnemyCards - 2) * 10;

                // 援军
                case CardDB.cardName.reinforce:
                    if(counterPriest) return 20;
                    return 0;
                // 强化援军/白银之手
                case CardDB.cardName.thesilverhand:
                    return -5;                
            }
            return 0;
        }
    }

}
