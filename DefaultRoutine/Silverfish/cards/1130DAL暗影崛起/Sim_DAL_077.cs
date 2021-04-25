using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DAL_077 : SimTemplate //* 毒鳍鱼人
    {
        // 战吼：使一个友方鱼人获得剧毒。

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            
            if ( target != null && own.own)
            {
                target.poisonous = true;
                if (own.own && target.name == CardDB.cardName.firemancerflurgl)
                {
                    foreach (Minion m in p.enemyMinions)
                    {
                        p.evaluatePenality -= m.Hp * 2;
                        p.minionGetDestroyed(m);
                    }
                }
            }
        }
    }
}