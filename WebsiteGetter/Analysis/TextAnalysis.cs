using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WebsiteGetter.Analysis
{
    class TextAnalysis
    {
        public static string getTitle(string website, string str = "")
        {
            string res = "";

            Regex reg1;
            string tmp;

            switch (website)
            {
                case "www.fengrubei.net":
                    reg1 = new Regex("论文题目</th><tdclass=\"col-2-3\">([^<]*)</td>");

                    str = str.Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");
                    if (!reg1.IsMatch(str)) break;
                    tmp = reg1.Match(str).Groups[1].ToString();
                    res += tmp;
                    break;
                default:
                    reg1 = new Regex("<title>([^<]*)</title>");
                    str = str.Trim();
                    if (reg1.IsMatch(str))
                    {
                        res = reg1.Match(str).Groups[1].ToString();
                    }
                    else
                    {
                        res = website;
                    }
                    break;
            }

            res = res.Replace('\\', '_');
            res = res.Replace('/', '_');
            res = res.Replace('*', '_');
            res = res.Replace(':', '_');
            res = res.Replace('<', '_');
            res = res.Replace('>', '_');
            res = res.Replace('"', '_');

            if (res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Length > 2)
                return res;
            else return "";
        }

        private static Dictionary<string, List<string>> contentFormatWithRegexGroup(string str, RegexItem[] regex)
        {
            Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();
            res.Add("all", new List<string> { str });

            List<RegexItem> g = new List<RegexItem>(regex);
            while (g.Count > 0)
            {
                bool find = false;
                foreach (var v in res)
                {
                    var regitem = g.Find(item => item.sourcename == v.Key);
                    if (regitem != null
                        &&
                        !string.IsNullOrWhiteSpace(regitem.sourcename)
                        &&
                        !string.IsNullOrWhiteSpace(regitem.name)
                        &&
                        !string.IsNullOrWhiteSpace(regitem.regex)
                        )
                    {
                        find = true;
                        var reg = regitem.regex;
                        Regex r = new Regex(reg, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        foreach (var value in v.Value)
                        {
                            var match = r.Matches(value);
                            if (match.Count > 0)
                            {
                                for (int i = 0; i < match.Count; i++)
                                {
                                    var newkey = regitem.name;
                                    var newval = match[i].Groups[1].Value;
                                    if (!res.ContainsKey(newkey)) res[newkey] = new List<string>();
                                    res[newkey].Add(newval);
                                }

                            }
                        }


                        g.Remove(regitem);
                        break;
                    }
                }
                if (!find) break;
            }

            return res;
        }


        public static Dictionary<string,List<string>> contentFormat(string url, string str, List<RegexGroup> regexGroup = null)
        {
            Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();
            res.Add("all", new List<string> { str });
            if (regexGroup != null && regexGroup.Count>0)
            {
                foreach (var regexg in regexGroup)
                {
                    if (url==regexg.target)
                    {
                        res = contentFormatWithRegexGroup(str, regexg.regex.ToArray());
                        break;
                    }
                }
            }

            //switch (website)
            //{
            //    case "wecool.socialmedia.cn":
            //        reg1 = new Regex(">(.*?)</h2>");
            //        reg2 = new Regex("<p>(.*)</p>");
            //        reg3 = new Regex("<p>(.*?)</p>");
            //        reg4 = new Regex("(<img.*?>)");
            //        string title = "";
            //        if (reg1.IsMatch(str))
            //        {
            //            tmp = reg1.Match(str).Groups[1].ToString();
            //            res += tmp;
            //            tmp = reg2.Match(str).Groups[0].ToString().Replace("<br/>", "");
            //            m = reg3.Matches(tmp);
            //            for (int i = 0; i < m.Count; i++)
            //            {
            //                tmp2 = m[i].Groups[1].ToString();
            //                if (!reg4.IsMatch(tmp2))
            //                    res += tmp2 + "\r\n";
            //            }
            //        }
            //        if (res.Length > 10) res = title + "\r\n" + res;
            //        break;
            //    case "www.6188.com":
            //        reg1 = new Regex("<title>(.*?)</title>");
            //        if (reg1.IsMatch(str))
            //        {
            //            res = reg1.Match(str).Groups[1].ToString();
            //        }
            //        break;
            //    case "10.200.21.61:7001":
            //        reg1 = new Regex("<td width=\"30%\">(.*)&nbsp;</td>");
            //        reg2 = new Regex("<td width=\"20%\">(.*)&nbsp;</td>");
            //        reg3 = new Regex("出生日期：</th>[^<]*<td>(.*)&nbsp;</td>");
            //        if (reg1.IsMatch(str))
            //        {
            //            str = str.Trim();
            //            tmp = reg1.Match(str).Groups[1].ToString();
            //            res += tmp;
            //            res += ",";
            //            tmp = reg2.Match(str).Groups[1].ToString();
            //            res += tmp;
            //            res += ",";
            //            tmp = reg3.Match(str).Groups[1].ToString();
            //            res += tmp;
            //        }
            //        break;
            //    case "www.fengrubei.net":
            //        reg1 = new Regex("项目作品全称/论文题目</th><tdclass=\"col-2-3\">([^<]*)</td>");

            //        str = str.Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");
            //        if (!reg1.IsMatch(str)) break;
            //        tmp = reg1.Match(str).Groups[1].ToString();
            //        res += tmp;


            //        res += "\r\n\r\n作者：\r\n";
            //        reg2 = new Regex("第一作者姓名</th><tdclass=\"col-2-3\">([^<]*)</td><trclass=\"even\"><thclass=\"col-1-6\">第一作者学号</th><tdclass=\"col-2-3\">([^<]*)</td>");
            //        string uname = reg2.Match(str).Groups[1].ToString();
            //        string uid = reg2.Match(str).Groups[2].ToString();
            //        res += uid + " " + uname + "\r\n";
            //        reg2 = new Regex("第二作者姓名</th><tdclass=\"col-2-3\">([^<]*)</td><trclass=\"even\"><thclass=\"col-1-6\">第二作者学号</th><tdclass=\"col-2-3\">([^<]*)</td>");
            //        if (reg2.IsMatch(str))
            //        {
            //            uname = reg2.Match(str).Groups[1].ToString();
            //            uid = reg2.Match(str).Groups[2].ToString();
            //            res += uid + " " + uname + "\r\n";
            //        }
            //        reg2 = new Regex("第三作者姓名</th><tdclass=\"col-2-3\">([^<]*)</td><trclass=\"even\"><thclass=\"col-1-6\">第三作者学号</th><tdclass=\"col-2-3\">([^<]*)</td>");
            //        if (reg2.IsMatch(str))
            //        {
            //            uname = reg2.Match(str).Groups[1].ToString();
            //            uid = reg2.Match(str).Groups[2].ToString();
            //            res += uid + " " + uname + "\r\n";
            //        }

            //        res += "\r\n\r\n项目简介：\r\n";
            //        reg2 = new Regex("项目简介或论文摘要</th><tdclass=\"col-2-3\">([^<]*)</td><trclass=\"odd\"><thclass=\"col-1-6\">项目特色与创新点</th><tdclass=\"col-2-3\">([^<]*)?</td>");
            //        tmp = reg2.Match(str).Groups[1].ToString();
            //        res += tmp;
            //        res += "\r\n项目特色：\r\n";
            //        tmp = reg2.Match(str).Groups[2].ToString();
            //        res += tmp;
            //        break;
            //    default:
            //        reg1 = new Regex("<div class=\\\"zm-editable-content clearfix\\\">(.*?)</div>", RegexOptions.Singleline);
            //        //reg1 = new Regex("(?s)<div class=\"title\"><h1>(.*)<div class=\"ad\"><script language=\"JavaScript\" src=\"http://www.itmuu.com/template/ad/cpc-960-read-1.js\"");
            //        if (reg1.IsMatch(str))
            //        {
            //            res = reg1.Match(str).Groups[1].ToString();
            //        }
            //        else
            //        {
            //            res = str;
            //        }
            //        break;
            //}
            //if (res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Length > 2)
            //if(!string.IsNullOrWhiteSpace(res))   return res;
           // else return "";
            return res;
        }

        
    }
}
