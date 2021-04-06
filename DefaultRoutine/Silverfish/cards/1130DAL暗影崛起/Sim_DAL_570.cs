using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DAL_570 : SimTemplate //* 永不屈服 Never Surrender!
	{
		//<b>Secret:</b> When your opponent casts a spell, give your minions +2_Health.
		// TODO <b>奥秘：</b>当你的对手施放一个法术时，使你的所有随从获得+2生命值。
		public override void onSecretPlay(Playfield p, bool ownplay, int number)
        {
            List<Minion> temp = ownplay ? p.ownMinions : p.enemyMinions;
			foreach (Minion m in temp)
			{
				p.minionGetBuffed(m, 0, 2);
			}   
        }

	}
}
