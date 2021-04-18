using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_875 : SimTemplate //* 逝者之剑 Sword of the Fallen
	{
		//[x]After your hero attacks,cast a <b>Secret</b> fromyour deck.
		//在你的英雄攻击后，从你的牌库中施放一个<b>奥秘</b>。
        CardDB.Card blaine = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_506);
		CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BAR_875);
		

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.equipWeapon(card, ownplay);
		}
	}
}