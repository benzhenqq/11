using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DMF_075 : SimTemplate //* 猜重量 Guess the Weight
	{
		//Draw a card. Guess if your next card costs more or less to draw it.
		//抽一张牌。猜中你下一张牌的法力值消耗更大或更小，则可抽取下一张牌。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			 p.drawACard(CardDB.cardName.unknown, ownplay);
		}
		
	}
}
