using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_008 : SimTemplate //* 正义感召 Righteous Cause
	{
		//<b>Sidequest:</b> Summon 5 minions.<b>Reward:</b> Give your minions +1/+1.
		//<b>支线任务：</b>召唤五个随从。<b>奖励：</b>使你的所有随从获得+1/+1。
		// FIX 对手的正义感召会在下回合让所有随从+1/+2
		public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (!turnStartOfOwner && triggerEffectMinion.own == turnStartOfOwner)
            {
				List<Minion> temp = p.enemyMinions;
				foreach (Minion m in temp)
				{
					p.minionGetBuffed(m, 1, 2);
				}
            }
        }
		
	}
}
