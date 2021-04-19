using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DMF_119 : SimTemplate //* 邪恶低语 Wicked Whispers
	{
		//Discard your lowest Cost card. Give your minions +1/+1.
		//弃掉你手牌中法力值消耗最低的牌。使你的所有随从获得+1/+1。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int minCost = 10;
			int index = 0, i = 0;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				i++;
				if(hc.card.cost < minCost){
					index = i;
					minCost = hc.card.cost;
				}
			}
			if(index > 0){
				Helpfunctions.Instance.ErrorLog("邪恶低语将弃掉" + p.owncards[index].card.卡名);
				p.discardCards(index, ownplay);
			}
			p.allMinionOfASideGetBuffed(ownplay, 1, 1);
		}
	}
}
