using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_YOP_035 : SimTemplate //* 月牙 Moonfang
	{
		//Can only take 1 damage at_a time.
		//每次只能受到1点伤害。
        // 失去圣盾失去一点生命
        public override void onMinionLosesDivineShield(Playfield p, Minion m, int num)
        {
            p.minionGetDamageOrHeal(m, 1, false);
            m.divineshild = true;
        }
		
	}
}
