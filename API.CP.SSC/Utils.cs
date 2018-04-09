using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace API.CP.SSC
{
    public class Utils
    {
        public static string 单双(char code)
        {
            switch (code)
            {
                case '0':
                case '2':
                case '4':
                case '6':
                case '8':
                    return 定位胆.双;
                case '1':
                case '3':
                case '5':
                case '7':
                case '9':
                    return 定位胆.单;

                default:
                    return "-";
            }
        }

        public static string 大小(char code)
        {
            switch (code)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                    return 定位胆.小;
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return 定位胆.大;
                default:
                    return "-";
            }
        }

        public static string 前二形态(string code)
        {
            try
            {
                string temp = code.Substring(0, 2);
                if (temp[0] == temp[1])
                {
                    return 二星形态.对子;
                }
                else
                {
                    if (Regex.Match(temp, @"(0(?=1|9)|1(?=2|0)|2(?=3|1)|3(?=4|2)|4(?=5|3)|5(?=6|4)|6(?=7|5)|7(?=8|6)|8(?=9|7)|9(?=0|8)){1}\d").Success)
                    {
                        return 二星形态.连号;
                    }
                }

                return "";
            }
            catch (System.Exception)
            {
                return "";
            }
        }

        public static string 后二形态(string code)
        {
            try
            {
                string temp = code.Substring(3, 2);
                if (temp[0] == temp[1])
                {
                    return 二星形态.对子;
                }
                else
                {
                    if (Regex.Match(temp, @"(0(?=1|9)|1(?=2|0)|2(?=3|1)|3(?=4|2)|4(?=5|3)|5(?=6|4)|6(?=7|5)|7(?=8|6)|8(?=9|7)|9(?=0|8)){1}\d").Success)
                    {
                        return 二星形态.连号;
                    }
                }

                return "";
            }
            catch (System.Exception)
            {
                return "";
            }
        }

        public static string 前三形态(string code)
        {
            try
            {
                string temp = code.Substring(0, 3);
                int len = Regex.Replace(temp, "(?s)(.)(?=.*\\1)", "").Length;
                if (len == 1)
                {
                    return 三星形态.豹子;
                }
                else if (len == 2)
                {
                    return 三星形态.组三;
                }
                else if (len == 3)
                {
                    return 三星形态.组六;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static string 中三形态(string code)
        {
            try
            {
                string temp = code.Substring(1, 3);
                int len = Regex.Replace(temp, "(?s)(.)(?=.*\\1)", "").Length;
                if (len == 1)
                {
                    return 三星形态.豹子;
                }
                else if (len == 2)
                {
                    return 三星形态.组三;
                }
                else if (len == 3)
                {
                    return 三星形态.组六;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static string 后三形态(string code)
        {
            try
            {
                string temp = code.Substring(2, 3);
                int len = Regex.Replace(temp, "(?s)(.)(?=.*\\1)", "").Length;
                if (len == 1)
                {
                    return 三星形态.豹子;
                }
                else if (len == 2)
                {
                    return 三星形态.组三;
                }
                else if (len == 3)
                {
                    return 三星形态.组六;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static string 五星形态(string code)
        {
            try
            {
                string temp = Regex.Replace(code, "(?s)(.)(?=.*\\1)", "");
                switch (temp.Length)
                {
                    case 1:
                        // 1个五重号
                        return API.CP.SSC.五星形态.大豹子;
                    case 2:
                        // 组5：1个四重号 + 1个单号
                        // 组10：1个三重号 + 1个两重号
                        foreach (char item in temp)
                        {
                            switch (code.Count(c => c == item))
                            {
                                case 1:
                                case 4:
                                    return API.CP.SSC.五星形态.组5;
                                default:
                                    return API.CP.SSC.五星形态.组10;
                            }
                        }

                        return "";
                    case 3:
                        // 组30：2个两重号 + 1个单号
                        // 组20：1个三重号 + 2个单号
                        int n3 = 0;
                        foreach (char item in temp)
                        {
                            if (code.Count(c => c == item) == 3)
                            {
                                n3 = 1;
                                break;
                            }
                        }

                        if (n3 == 1)
                            return API.CP.SSC.五星形态.组20;
                        else
                            return API.CP.SSC.五星形态.组30;

                    case 4:
                        // 组60：1个两重号 + 3个单号
                        return API.CP.SSC.五星形态.组60;
                    case 5:
                        // 组120：5个单号
                        return API.CP.SSC.五星形态.组120;
                    default:
                        return "";
                }
            }
            catch
            {
                return "";
            }
        }

        //public static void 更新开奖清单(string sn, string code, DataSet aDS)
        //{
        //    DictSetUtil aXTCS = ExtXTCSUtil.PickExtXTCS2(aDS);
        //    if (string.IsNullOrEmpty(sn) || sn.Length != 12)    // 20150709-033
        //        return;

        //    try
        //    {
        //        aXTCS.SetValue(sn, code);
        //        if (code.Replace("-", "").Trim().Length != 5)
        //        {
        //            aXTCS.SetValue(sn, "开奖结果", "-");
        //            aXTCS.SetValue(sn, "前三形态", "-");
        //            aXTCS.SetValue(sn, "中三形态", "-");
        //            aXTCS.SetValue(sn, "后三形态", "-");
        //            aXTCS.SetValue(sn, "五星形态", "-");
        //            aXTCS.SetValue(sn, "万", "-");
        //            aXTCS.SetValue(sn, "千", "-");
        //            aXTCS.SetValue(sn, "百", "-");
        //            aXTCS.SetValue(sn, "十", "-");
        //            aXTCS.SetValue(sn, "个", "-");
        //        }
        //        else
        //        {
        //            aXTCS.SetValue(sn, "开奖结果", code);
        //            aXTCS.SetValue(sn, "前三形态", CodeUtil.前三形态(code));
        //            aXTCS.SetValue(sn, "中三形态", CodeUtil.中三形态(code));
        //            aXTCS.SetValue(sn, "后三形态", CodeUtil.后三形态(code));
        //            aXTCS.SetValue(sn, "五星形态", CodeUtil.五星形态(code));
        //            if (code.Length == 5)
        //            {
        //                aXTCS.SetValue(sn, "万", CodeUtil.大小(code[0]) + CodeUtil.单双(code[0]));
        //                aXTCS.SetValue(sn, "千", CodeUtil.大小(code[1]) + CodeUtil.单双(code[1]));
        //                aXTCS.SetValue(sn, "百", CodeUtil.大小(code[2]) + CodeUtil.单双(code[2]));
        //                aXTCS.SetValue(sn, "十", CodeUtil.大小(code[3]) + CodeUtil.单双(code[3]));
        //                aXTCS.SetValue(sn, "个", CodeUtil.大小(code[4]) + CodeUtil.单双(code[4]));
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
    }
}
