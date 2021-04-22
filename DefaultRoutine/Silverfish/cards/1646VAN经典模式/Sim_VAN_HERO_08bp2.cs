﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_VAN_HERO_08bp2 : SimTemplate //* 二级火焰冲击
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getHeroPowerDamage(2) : p.getEnemyHeroPowerDamage(2);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}