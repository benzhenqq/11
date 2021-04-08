using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_YOP_035 : SimTemplate //* 月牙 Moonfang
	{
		//Can only take 1 damage at_a time.
		//每次只能受到1点伤害。
		public override void onMinionGotDmgTrigger(Playfield p, Minion m, int anzOwnMinionsGotDmg, int anzEnemyMinionsGotDmg, int anzOwnHeroGotDmg, int anzEnemyHeroGotDmg)
        {
            if (m.anzGotDmg > 0)
            {
                int tmp = m.anzGotDmg;
                m.anzGotDmg = 0;
                p.minionGetDamageOrHeal(m, tmp - 1, false);
            }
        }
		
	}
}
