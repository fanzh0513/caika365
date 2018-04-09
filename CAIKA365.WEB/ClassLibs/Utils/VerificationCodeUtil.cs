using System;
using System.Drawing;

namespace CAIKA365.WEB.ClassLibs
{
    public class VerificationCodeUtil
    {
        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="length">随机码个数</param>
        /// <returns></returns>
        public static string CreateCode(int length)
        {
            int rand;
            char code;
            string randomcode = String.Empty;

            //生成一定长度的验证码
            System.Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                rand = random.Next();
                if (rand % 3 == 0)
                {
                    code = (char)('A' + (char)(rand % 26));
                }
                else
                {
                    code = (char)('0' + (char)(rand % 10));
                }

                randomcode += code.ToString();
            }

            return randomcode;
        }

        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="randomcode">随机码</param>
        public static Bitmap CreateImage(string randomcode)
        {
            Bitmap map = new Bitmap(140, 44);
            Graphics graph = Graphics.FromImage(map);
            Random rand = new Random();

            // 重置图片（边框）
            graph.Clear(Color.AliceBlue);
            graph.DrawRectangle(new Pen(Color.LightGray, 0), 0, 0, map.Width - 1, map.Height - 1);

            //背景噪点生成
            Pen blackPen = new Pen(Color.LightGray, 0);
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(0, map.Width);
                int y = rand.Next(0, map.Height);
                graph.DrawRectangle(blackPen, x, y, 1, 1);
            }

            //验证码旋转，防止机器识别
            char[] chars = randomcode.ToCharArray();//拆散字符串成单字符数组

            // 文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            // 颜色、字体
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
            for (int i = 0; i < chars.Length; i++)
            {
                int findex = rand.Next(5);
                int cindex = rand.Next(7);

                // 字体样式(参数2为字体大小)
                Font f = new System.Drawing.Font(font[findex], 28, System.Drawing.FontStyle.Bold);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                Point dot = new Point(24, 24);

                int randAngle = 45; // 随机转动角度
                float angle = rand.Next(-randAngle, randAngle);

                // 移动光标到指定位置
                graph.TranslateTransform(dot.X, dot.Y);
                graph.RotateTransform(angle);
                graph.DrawString(chars[i].ToString(), f, b, 2, 2, format);

                graph.RotateTransform(-angle);// 转回去
                graph.TranslateTransform(-2, -dot.Y);// 移动光标到指定位置，每个字符紧凑显示，避免被软件识别
            }

            return map;
        }
    }
}