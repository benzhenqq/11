using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_093 : SimTemplate //* Argent Watchman
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int cnt = 3;
			string str = "我寻思出战斗号角应该能出: ";
			// 遍历卡组
			foreach(KeyValuePair<CardDB.cardIDEnum, int>kvp in p.prozis.turnDeck )
			{
				// ID 转卡
				CardDB.cardIDEnum deckCard = kvp.Key;
				CardDB.Card deckMinion = CardDB.Instance.getCardDataFromID(deckCard);
				if( deckMinion.cost <= 2 ){
					int pos = p.ownMinions.Count ;
					p.callKid(deckMinion, pos, ownplay);
					str += deckMinion.卡名 + " ";
					cnt--;
					if(cnt <= 0)break;
				}
			}
			Helpfunctions.Instance.ErrorLog(str);
		}
    }
}