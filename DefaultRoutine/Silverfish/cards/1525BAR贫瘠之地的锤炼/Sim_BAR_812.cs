using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_812 : SimTemplate //* 绿洲盟军 Oasis Ally
	{
		// 当一个友方随从受到攻击时，召唤一个3/6的水元素
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BT_072);//水元素

        public override void onSecretPlay(Playfield p, bool ownplay, int number)
        {
            int place = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, place, ownplay);
        }		
	}
}
