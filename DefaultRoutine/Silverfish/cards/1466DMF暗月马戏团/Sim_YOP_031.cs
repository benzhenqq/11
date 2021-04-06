using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_YOP_031 : SimTemplate //* 螃蟹骑士 Crabrider
	{
		//<b>Rush</b><b>Windfury</b>
		//<b>突袭，风怒</b>
		// FIX 兄弟啊兄弟，对面的螃蟹很危险
		public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (!turnStartOfOwner && triggerEffectMinion.own == turnStartOfOwner)
            {
                if(p.enemyMaxMana >= 5){
                    p.minionGetBuffed(triggerEffectMinion, 4, 4);
                }else {
                    p.minionGetBuffed(triggerEffectMinion, 2, 2);
                }
            }
        }
		
	}
}
