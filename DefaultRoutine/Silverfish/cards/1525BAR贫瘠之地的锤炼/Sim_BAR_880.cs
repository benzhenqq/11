using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_880 : SimTemplate //* 定罪（等级1） Conviction (Rank 1)
	{
		//[x]Give a random friendlyminion +3 Attack.<i>(Upgrades when youhave 5 Mana.)</i>
		//随机使一个友方随从获得+3攻击力。<i>（当你有5点法力值时升级。）</i>
		// 随机我哪会写啊，从左到右依次获得就行了
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			if(ownplay){
				int cnt = 1;
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
