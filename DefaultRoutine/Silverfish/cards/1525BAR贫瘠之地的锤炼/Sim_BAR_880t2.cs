using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_880t2 : SimTemplate //* 定罪（等级3） Conviction (Rank 3)
	{
		//Give three random friendly minions +3_Attack.
		//随机使三个友方随从获得+3攻击力。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			if(ownplay){
				int cnt = 3;
				foreach(Minion m in p.ownMinions)
				{
					cnt = cnt - 1;
					p.minionGetBuffed(m, 3, 0);
					if(cnt <= 0) break;
				}
			}
		}
		
	}
}
