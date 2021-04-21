using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_CORE_EX1_198 : SimTemplate //* 娜塔莉·塞林 Natalie Seline
	{
		//<b>Battlecry:</b> Destroy a minion and gain its Health.
		//<b>战吼：</b>消灭一个随从并获得其生命值。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				own.Hp = target.Hp;
				p.minionGetDestroyed(target);
			}
		}
	}
}
