namespace HREngine.Bots
{
    using System;
    using System.IO;
    // 虽然叫打印工具，但是其实是用来存放自定义的静态变量的
    public class printUtils
    {
        public static bool printPentity = false;

        public static bool printNextMove = false;

        public static int enfaceReward = 0;


        // type 取值 0 （打印下一步）， 1（打印自定义惩罚）
        public static void printDebuggerInfo(CardDB.Card card, string content, int pen, int type){
            if(pen == 0) return;
            if( printPentity && type == 1 ){
                Helpfunctions.Instance.ErrorLog(card.卡名 + content + pen+"");
            }
        }

        public static void printNowVal(){
            Playfield p = new Playfield(); 
            // 输出当前场面价值判定
            String enemyVal = "[敌方场面] ";
            String myVal    = "[我方场面] ";
            foreach (Minion m in p.enemyMinions)
            {
                enemyVal += m.handcard.card.卡名 + "(" + m.Angr + "/" + m.Hp + ") 威胁: " + Ai.Instance.botBase.getEnemyMinionValue(m, p) + "; ";
                //hasTank = hasTank || m.taunt;
            }
            foreach (Minion m in p.ownMinions)
            {
                myVal += m.handcard.card.卡名 + "(" + m.Angr + "/" + m.Hp +  ") 价值: " + Ai.Instance.botBase.getMyMinionValue(m, p) + "; ";
                //hasTank = hasTank || m.taunt;
            }
            Helpfunctions.Instance.ErrorLog(enemyVal);
            Helpfunctions.Instance.ErrorLog(myVal);
        }
    }
}