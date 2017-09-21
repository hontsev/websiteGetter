using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteGetter.Catch
{
    class NumberGetter
    {

        /// <summary>
        /// 给字符串前添加0补齐位数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string add0(string str, int len)
        {
            string res = "";
            for (int i = 0; i < len - str.Length; i++)
            {
                res += "0";
            }
            res += str;
            return res;
        }


        /// <summary>
        /// 获取下一个北航学号（本科生）
        /// </summary>
        /// <param name="nowid"></param>
        /// <returns></returns>
        public static string nextBUAA(string nowid)
        {
            string next = "";
            int temp = Int32.Parse(nowid);
            if (nowid.Length == 8)
            {
                if (temp % 1000 > 300)
                {
                    if (temp / 10000 % 100 > 29)               //加一学年
                    {
                        temp = (temp / 1000000 + 1) * 1000000 + 11001;
                    }
                    else                            //加一个系
                    {
                        temp = (temp / 10000 + 1) * 10000 + 1001;
                    }
                }
                else                //加一个号
                {
                    temp++;
                }
            }
            else
            {
                temp++;
            }
            next = temp.ToString();
            return next;
        }

        public static string nextUCAS(string nowid)
        {
            string next = "";
            if (nowid.Length < 15) return nowid;

            int year = int.Parse(nowid.Substring(0, 4));
            int type = int.Parse(nowid.Substring(4, 1));
            int org = int.Parse(nowid.Substring(5, 5));
            int major = int.Parse(nowid.Substring(10, 2));
            int num = int.Parse(nowid.Substring(12, 3));

            if ( num <= 98) num++;
            else if (major <= 98) { major++;num = 1; }
            else if (org <= 80198) { org++; major = 1;num = 1; }
            else if (type <= 3) { type++;org = 80001;major = 1;num = 1; }
            else { year++;type = 1;org = 80001;major = 1;num = 1; }

            next = year.ToString() + type.ToString() + org.ToString() + major.ToString().PadLeft(2,'0') + num.ToString().PadLeft(3,'0');
            return next;
        }
    }
}
