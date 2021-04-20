using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_SCH_248 : SimTemplate //* 甩笔侏儒 Pen Flinger
	{
		//<b>Battlecry:</b> Deal 1 damage. <b>Spellburst:</b> Return this to_your hand.
		//<b>战吼：</b>造成1点伤害。<b>法术迸发：</b>将该随从移回你的手牌。
		
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if(target!=null)
		    {
		       p.minionGetDamageOrHeal(target, 1);
			}		
		}
        
		public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool ownplay, Minion m)
        {
           if (m.own == ownplay && hc.card.type == CardDB.cardtype.SPELL) p.minionReturnToHand(m,m.own, 0);
        }
		
		
	}
}
