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
    }
}