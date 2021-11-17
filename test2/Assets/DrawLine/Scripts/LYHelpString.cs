
using Newtonsoft.Json;
using System.IO;

public class LYHelpString
{
    //获取最后一个.(不一定是. 可以是其他字符串) 前面或后面的字符串 true代表前面
    public static string StripExt(string prefabPath, string lastFenGeFu = ".", bool isFirstStr = true)
    {
        int idx = prefabPath.LastIndexOf(lastFenGeFu, System.StringComparison.Ordinal);
        if (idx < 0)
        {
            return prefabPath;
        }
        return isFirstStr ? prefabPath.Substring(0, idx) : prefabPath.Substring(idx + lastFenGeFu.Length, prefabPath.Length - idx - lastFenGeFu.Length);
    }

    //string 转成json字符串
    public static string ConvertJsonString(string str)
    {
        JsonSerializer jsonSerializer = new JsonSerializer();
        TextReader reader = new StringReader(str);
        JsonTextReader reader2 = new JsonTextReader(reader);
        object obj = jsonSerializer.Deserialize(reader2);
        if (obj != null)
        {
            StringWriter stringWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            jsonSerializer.Serialize(jsonWriter, obj);
            return stringWriter.ToString();
        }
        return str;
    }
}
