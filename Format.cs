using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace MFAsys
{
    static class Format
    {

        public static Dictionary<int, string> Base64Code = new Dictionary<int, string>() {
        {   0  ,"A"}, {   1  ,"B"}, {   2  ,"C"}, {   3  ,"D"}, {   4  ,"E"}, {   5  ,"F"}, {   6  ,"G"}, {   7  ,"H"}, {   8  ,"I"}, {   9  ,"J"},
        {   10  ,"K"}, {   11  ,"L"}, {   12  ,"M"}, {   13  ,"N"}, {   14  ,"O"}, {   15  ,"P"}, {   16  ,"Q"}, {   17  ,"R"}, {   18  ,"S"}, {   19  ,"T"},
        {   20  ,"U"}, {   21  ,"V"}, {   22  ,"W"}, {   23  ,"X"}, {   24  ,"Y"}, {   25  ,"Z"}, {   26  ,"a"}, {   27  ,"b"}, {   28  ,"c"}, {   29  ,"d"},
        {   30  ,"e"}, {   31  ,"f"}, {   32  ,"g"}, {   33  ,"h"}, {   34  ,"i"}, {   35  ,"j"}, {   36  ,"k"}, {   37  ,"l"}, {   38  ,"m"}, {   39  ,"n"},
        {   40  ,"o"}, {   41  ,"p"}, {   42  ,"q"}, {   43  ,"r"}, {   44  ,"s"}, {   45  ,"t"}, {   46  ,"u"}, {   47  ,"v"}, {   48  ,"w"}, {   49  ,"x"},
        {   50  ,"y"}, {   51  ,"z"}, {   52  ,"0"}, {   53  ,"1"}, {   54  ,"2"}, {   55  ,"3"}, {   56  ,"4"}, {   57  ,"5"}, {   58  ,"6"}, {   59  ,"7"},
        {   60  ,"8"}, {   61  ,"9"}, {   62  ,"+"}, {   63  ,"/"}, };

        public static Dictionary<string, int> _Base64Code
        {
            get
            {
                return Enumerable.Range(0, Base64Code.Count()).ToDictionary(i => Base64Code[i], i => i);
            }
        }

        public static string Float10ToString64(float float10)
        {
            long bit10_long = (long)(float10 * 10000000000);//要将约10位十进制转化为约40位二进制
            string bit2_string = Convert.ToString(bit10_long, 2);//将原10进制字符串（约12位）转化为2进制，大约在40位
            //Console.WriteLine("bit2_string:" + bit2_string);
            int len = bit2_string.Length;//新字符串长度
            //Console.WriteLine("len:" + len.ToString());
            int left_len = len;//字符串剩余长度
            int index;
            string kong = "";
            while (left_len > 6)
            {
                string sixbit = bit2_string.Substring(left_len - 6);//提取末尾6位(参数表示特定位置)
                //Console.WriteLine("sixbit:" + sixbit);
                bit2_string = bit2_string.Substring(0, left_len - 6);//保留其余位（第一个参数表示特定位置，第二个参数表示截取的长度）
                index = Convert.ToInt32(sixbit, 2);
                //Console.WriteLine("index:" + index);
                kong = Base64Code[index] + kong;//旧字符串从右向左裁剪数字。因此新字符串中原始的kong应在右侧，新生成的数位应放在在左侧
                left_len = left_len - 6;
            }
            index = Convert.ToInt32(bit2_string, 2);
            kong = Base64Code[index] + kong;
            return kong;
        }

        public static float String64ToFloat10(string string64)
        {
            long long10 = 0;
            int power = string64.Length - 1;
            for (int i = 0; i <= power; i++)
            {
                long10 += _Base64Code[string64[power - i].ToString()] * Convert.ToInt64(Math.Pow(64, i));
            }
            float float10 = (float)(long10) / 10000000000.0f;
            return float10;
        }

        public static string FloatsToVarcharMax(float[] float10)
        {
            int len = float10.Length;
            string varchar64 = Float10ToString64(float10[0]); ;
            int i = 1;
            while (i < len)
            {
                string string_that = Float10ToString64(float10[i]);
                varchar64 = varchar64 + " " + string_that;
                i++;
            }
            return varchar64;
        }

        public static float[] VarcharMaxToFloats(string varchar64)
        {
            float[] float10s = new float[1024];
            string[] item = Regex.Split(varchar64, "\\s+", RegexOptions.IgnoreCase);
            for (int i = 0; i < item.Length; i++)
                float10s[i] = String64ToFloat10(item[i]);
            return float10s;
        }

        public static float StringToFloat(string string10)
        {
            float float10 = float.Parse(string10);
            return float10;
        }

        public static string FloatsToVarcharMax_new(float[] float10)
        {
            int len = float10.Length;
            string varchar64 = float10[0].ToString();
            int i = 1;
            while (i < len)
            {
                string string_that = float10[i].ToString();
                varchar64 = varchar64 + " " + string_that;
                i++;
            }
            return varchar64;
        }

        public static float[] VarcharMaxToFloats_new(string varchar64)
        {
            float[] float10s = new float[1024];
            string[] item = Regex.Split(varchar64, "\\s+", RegexOptions.IgnoreCase);
            for (int i = 0; i < item.Length; i++)
                float10s[i] = StringToFloat(item[i]);
            return float10s;
        }

        public static void Test()
        {
            float[] a = {
                0.005846504f,
                0.02717116f,
                0.13016f,
                0.0005517777f,
                0f,
                0.0002282686f,
                0.009999999999f
            };


            /*string varchar64 = FloatsToVarcharMax(a);
            Console.WriteLine("varchar64：" + varchar64);

            float[] ax = VarcharMaxToFloats(varchar64);
            Console.WriteLine("ax：");
            for (int i = 0; i < ax.Length; i++)
                Console.WriteLine(ax[i]);*/


            string varchar64_new = FloatsToVarcharMax_new(a);
            Console.WriteLine("varchar64_new：" + varchar64_new);

            float[] ax_new = VarcharMaxToFloats_new(varchar64_new);
            Console.WriteLine("ax_new：");
            for (int i = 0; i < ax_new.Length; i++)
                Console.WriteLine(ax_new[i]);
        }


    }
}
