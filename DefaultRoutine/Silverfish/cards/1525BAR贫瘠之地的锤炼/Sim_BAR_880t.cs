using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_880t : SimTemplate //* 定罪（等级2） Conviction (Rank 2)
	{
		//[x]Give two random friendlyminions +3 Attack.<i>(Upgrades when youhave 10 Mana.)</i>
		//随机使两个友方随从获得+3攻击力。<i>（当你有10点法力值时升级。）</i>
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			if(ownplay){
				int cnt = 2;
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
