using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DMF_237 : SimTemplate //* 狂欢报幕员 Carnival Barker
	{
		//Whenever you summon a 1-Health minion, give it +1/+2.
		//每当你召唤一个生命值为1的随从，便使其获得+1/+2。
		// FIX 敌方的狂欢报幕员自带每回合+2/+4
		public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (!turnStartOfOwner && triggerEffectMinion.own == turnStartOfOwner)
            {
                p.minionGetBuffed(triggerEffectMinion, 2, 4);
            }
        }
		
	}
}
