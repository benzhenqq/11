using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DMF_082t : SimTemplate //* 暗月雕像 Darkmoon Statue
	{
		//<b>Corrupted</b>Your other minions have +1 Attack.
		//<b>已腐蚀</b>你的其他随从获得+1攻击力。
		public override void onAuraStarts(Playfield p, Minion own)
		{
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                p.minionGetBuffed(m, 1, 0);
            }            
		}

        public override void onAuraEnds(Playfield p, Minion own)
        {
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                p.minionGetBuffed(m, -1, 0);
            }
        }
		
		
	}
}
