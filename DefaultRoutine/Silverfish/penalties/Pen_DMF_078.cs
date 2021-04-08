using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_DMF_163 : PenTemplate //* 狂欢小丑
	{

//    spott/
        public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
        {
            foreach (Handmanager.Handcard hc in p.owncards){
                // 手里有高费卡
                if( hc.card.cost >= m.handcard.card.cost ){
                    // 存在足够的费用打出更高费的卡来腐蚀自己
                   if(p.ownMaxMana >= m.handcard.card.cost){
                       return 100;
                   }else {
                       return 20;
                   }
                }
            }
            return 0;
        }

	}
}