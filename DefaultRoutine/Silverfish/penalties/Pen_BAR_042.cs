using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_BAR_042 : PenTemplate //goldshirefootman
	{

//    spott/
        public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
        {
            int maxCost = 0;
            foreach(KeyValuePair<CardDB.cardIDEnum, int>kvp in p.prozis.turnDeck )
			{
				// ID 转卡
				CardDB.cardIDEnum deckCard = kvp.Key;
				CardDB.Card deckSpell = CardDB.Instance.getCardDataFromID(deckCard);
				if(deckSpell.type == CardDB.cardtype.SPELL && deckSpell.cost > maxCost){
                    maxCost = deckSpell.cost;                    
                }
			}
			return -10 * maxCost;
        }

	}
}