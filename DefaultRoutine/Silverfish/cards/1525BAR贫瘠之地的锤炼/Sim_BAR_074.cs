using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_074 : SimTemplate //* 前沿哨所 Far Watch Post
	{
		//[x]Can't attack. After youropponent draws a card, it___costs (1) more <i>(up to 10)</i>.__
		//无法攻击。在你的对手抽一张牌后，使其法力值消耗增加（1）点<i>（最高不超过10点）</i>。
		// FIX 敌方的哨所自带每回合+2/+2/可以攻击，就问你怕不怕
		public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (!turnStartOfOwner && triggerEffectMinion.own == turnStartOfOwner)
            {
                p.minionGetBuffed(triggerEffectMinion, 2, 2);
				p.minionLoseCantAttack(triggerEffectMinion);
            }
        }
		
	}
}
