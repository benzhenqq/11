using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_SCH_609 : PenTemplate //优胜劣汰
	{
		public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
		{
            // 手牌中有小丑
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if(hc.card.name == CardDB.cardName.carnivalclown){
                    return -60;
                }
            }
            // TO
		    return -5;
		}
	}
}