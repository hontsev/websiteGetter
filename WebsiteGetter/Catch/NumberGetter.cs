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
        public static string nextId(string nowid)
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
    }
}
