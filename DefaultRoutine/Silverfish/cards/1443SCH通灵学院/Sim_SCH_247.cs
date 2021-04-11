using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_SCH_247 : SimTemplate //* 新生入学 First Day of School
	{
		//Add 2 random 1-Cost minions to your hand.
		//随机将两张法力值消耗为（1）的随从牌置入你的手牌。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)        
        {
			p.drawACard(CardDB.cardIDEnum.EX1_009, ownplay, true);		
			p.drawACard(CardDB.cardIDEnum.EX1_009, ownplay, true);					
        }		
	}
}
