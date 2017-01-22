using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteGetter
{
    /// <summary>
    /// 用来根据特定设置的字母表来枚举字符串的类
    /// </summary>
    class WordsIncrement
    {
        public bool useNumberChars = false;
        public bool useSmallLatinChars = false;
        public bool useBigLatinChars = false;
        public bool useSymbolChars = false;
        private bool useUserChars = false;

        private char[] NumberChars = new char[]
        {
            '0','1','2','3','4','5','6','7','8','9'
        };
        private char[] SmallLatinChars = new char[]
        {
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
        };
        private char[] BigLatinChars = new char[]
        {
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };
        private char[] SymbolChars = new char[]
        {
            '!','@','#','$','%','^','&','*','(',')'
        };
        private char[] UserChars;
        private char[] chars;
        private string nowStr;

        /// <summary>
        /// 初始化，使用前必须初始化
        /// </summary>
        public void initChar()
        {
            int length = 0;
            if (useNumberChars) length += NumberChars.Length;
            if (useSmallLatinChars) length += SmallLatinChars.Length;
            if (useBigLatinChars) length += BigLatinChars.Length;
            if (useSymbolChars) length += SymbolChars.Length;
            if (useUserChars) length += UserChars.Length;
            chars = new char[length];
            int n = 0;
            if (useNumberChars)
            {
                for (int i = 0; i < NumberChars.Length; i++)
                {
                    chars[n] = NumberChars[i];
                    n++;
                }
            }
            if (useSmallLatinChars)
            {
                for (int i = 0; i < SmallLatinChars.Length; i++)
                {
                    chars[n] = SmallLatinChars[i];
                    n++;
                }
            }
            if (useBigLatinChars)
            {
                for (int i = 0; i < BigLatinChars.Length; i++)
                {
                    chars[n] = BigLatinChars[i];
                    n++;
                }
            }
            if (useSymbolChars)
            {
                for (int i = 0; i < SymbolChars.Length; i++)
                {
                    chars[n] = SymbolChars[i];
                    n++;
                }
            }
            if (useUserChars)
            {
                for (int i = 0; i < UserChars.Length; i++)
                {
                    chars[n] = UserChars[i];
                    n++;
                }
            }
            nowStr = "";
        }

        /// <summary>
        /// 默认为只有阿拉伯数字
        /// </summary>
        public WordsIncrement()
        {
            useNumberChars = true;
            initChar();
        }

        /// <summary>
        /// 根据特定字符表来构造枚举词典
        /// </summary>
        /// <param name="userChars"></param>
        public WordsIncrement(char[] userChars)
        {
            UserChars = userChars;
            useUserChars = true;
            initChar();
        }

        /// <summary>
        /// 查询枚举序列，获得紧邻的下一个字符
        /// </summary>
        /// <param name="c1"></param>
        /// <returns></returns>
        private char getNextChar(char c1)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (c1 == chars[i])
                {
                    if (i == chars.Length - 1) return chars[0];
                    else return chars[i + 1];
                }
            }
            return chars[0];
        }

        /// <summary>
        /// 修改字符串中特定位上的字符
        /// </summary>
        /// <param name="str">待修改字符串</param>
        /// <param name="index">位数，最左为0</param>
        /// <param name="a">要修改成的字符</param>
        /// <returns></returns>
        private string changeCharInStr(string str, int index, char a)
        {
            string newstr = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (i == index) newstr += a;
                else newstr += str[i];
            }
            return newstr;
        }

        /// <summary>
        /// 获取枚举的下一个字符串
        /// </summary>
        /// <param name="str">当前字符串，留空则表示用该对象之前记录的字符串</param>
        /// <returns></returns>
        public string getNext(string str=null)
        {
            if (str != null)
            {
                nowStr = str;
            }
            if (nowStr.Length <= 0)
            {
                nowStr = chars[0].ToString();
            }
            else
            {
                bool addone = false;
                char a;
                for (int i = nowStr.Length - 1; i >= 0; i--)
                {
                    a = nowStr[i];
                    if (i == nowStr.Length - 1)
                    {
                        a = getNextChar(a);
                        if (a == chars[0])
                        {
                            addone = true;
                        }
                    }
                    else if (addone)
                    {
                        a = getNextChar(a);
                        if (a != chars[0])
                        {
                            addone = false;
                        }
                    }
                    nowStr = changeCharInStr(nowStr, i, a);
                }
                if (addone)
                {
                    nowStr = chars[0].ToString() + nowStr;
                }
            }
            return nowStr;
        }

        /// <summary>
        /// 设置对象存储的当前字符串
        /// </summary>
        /// <param name="str"></param>
        public void setNowStr(string str)
        {
            nowStr = str;
        }
    }
}
