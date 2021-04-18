using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_880t : SimTemplate //* 定罪（等级2） Conviction (Rank 2)
	{
		//[x]Give two random friendlyminions +3 Attack.<i>(Upgrades when youhave 10 Mana.)</i>
		// 随机我哪会写啊，优先加给不会攻击的吧
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			if(ownplay){
				int cnt = 2;
				String str = "定罪buff将加给";
				// 优先加给不会攻击的吧
				foreach(Minion m in p.ownMinions)
				{
					if(m.cantAttackHeroes || m.cantAttack || !m.Ready){
						cnt = cnt - 1;
						p.minionGetBuffed(m, 3, 0);
						str += m.handcard.card.卡名 + ",";
						if(cnt <= 0) break;
					}
				}
				foreach(Minion m in p.ownMinions)
				{
					if(cnt <= 0) break;
					cnt = cnt - 1;
					str += m.handcard.card.卡名;
					p.minionGetBuffed(m, 3, 0);
				}
				// Helpfunctions.Instance.ErrorLog(str); 
			}
		}
		
	}
}
