using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_SCH_610 : PenTemplate //动物保镖
	{
		public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
		{
            int cnt = 2;
			// 遍历卡组
			foreach(KeyValuePair<CardDB.cardIDEnum, int>kvp in p.prozis.turnDeck )
			{
				// ID 转卡
				CardDB.cardIDEnum deckCard = kvp.Key;
				CardDB.Card deckMinion = CardDB.Instance.getCardDataFromID(deckCard);
				// 5费以下野兽
				if(deckMinion.race == 20 && deckMinion.cost <= 5){
					cnt--;
					if(cnt <= 0) return -40;
				}
			}
            if(cnt == 1) return -10;
            return 100;
		}
	}
}