using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BT_301 : SimTemplate //* 夜影主母 Nightshade Matron
	{
		//<b>Rush</b><b>Battlecry:</b> Discard your highest Cost card.
		//<b>突袭，战吼：</b>弃掉你的法力值消耗最高的牌。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int maxCost = 0;
			int index = 0, i = 0;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				i++;
				if(hc.card.cost > maxCost){
					index = i;
					maxCost = hc.card.cost;
				}
			}
			if(index > 0){
				Helpfunctions.Instance.ErrorLog("夜影主母将弃掉" + p.owncards[index].card.卡名);
				p.discardCards(index, own.own);
			}
			p.discardCards(1, own.own);
		}

	}
}
