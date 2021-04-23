
using System.Collections.Generic;

namespace HREngine.Bots
{
    public abstract class Behavior
    {
        public virtual float getPlayfieldValue(Playfield p)
        {
            return 0;
        }

        public virtual int getEnemyMinionValue(Minion m, Playfield p)
        {
            return 0;
        }

        public virtual int getMyMinionValue(Minion m, Playfield p)
        {
            int retval = 5;
            retval += m.Hp * 2;
            if(!m.cantAttack || !m.Ready || !m.frozen){
                retval += m.Angr * 2;
            }else {
                retval += m.Angr / 2;
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
            retval += m.synergy;
            return retval;
        }

        public virtual string BehaviorName()
        {
            return "None";
        }

        public virtual int getPlayCardPenality(CardDB.Card card, Minion target, Playfield p)
        {
            return 0;
        }

        public virtual int getAttackWithHeroPenality(Minion target, Playfield p)
        {
            return 0;
        }

        public virtual int getAttackWithMininonPenality(Minion m, Playfield p, Minion target)
        {
            return 0;
        }

        public virtual int getSirFinleyPriority(List<Handmanager.Handcard> discoverCards)
        {
            return -1;
        }

        public virtual int getDiscoverPriority(Playfield playfield, Handmanager.Handcard handcard)
        {
            return 0;
        }
		public virtual int GetSpecialCardComboPenalty(CardDB.Card card, Minion target, Playfield p)
        {
            return 0;
        }

    }

}