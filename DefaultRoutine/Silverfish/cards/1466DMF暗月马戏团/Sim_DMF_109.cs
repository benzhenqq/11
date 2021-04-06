using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DMF_109 : SimTemplate // 暗月先知赛格
    {
        // 战吼：抽1张牌。（在本局对战中，每触发一个友方奥秘都会升级！）

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                for (int i = 1; i <= p.secretsTriggeredSinceRecalc; i++)
                {
                    p.drawACard(CardDB.cardName.unknown, own.own);
                }
            }
        }

    }
}
