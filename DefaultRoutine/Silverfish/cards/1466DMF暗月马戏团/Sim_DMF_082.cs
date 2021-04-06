using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DMF_082 : SimTemplate //* 暗月雕像 Darkmoon Statue
	{
		//Your other minions have +1 Attack. <b>Corrupt:</b> This gains +4 Attack.
		//你的其他随从获得+1攻击力。<b>腐蚀：</b>该随从获得+4攻击力。
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
