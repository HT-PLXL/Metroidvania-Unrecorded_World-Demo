using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ExportScriptsToTxt
{
    [MenuItem("Tools/导出 Scripts 文件夹代码")]
    static void ExportAllScriptsInFolder()
    {
        // ---------- 设置要导出的文件夹 ----------
        string targetFolder = "Assets/Scripts"; // 可根据项目实际修改

        // 检查文件夹是否存在
        string fullPath = Path.Combine(Application.dataPath, "Scripts"); // Assets/Scripts 的绝对路径
        if (!Directory.Exists(fullPath))
        {
            EditorUtility.DisplayDialog("错误", $"文件夹不存在：{fullPath}\n请检查路径或修改代码中的 targetFolder", "确定");
            return;
        }

        // 让用户选择保存位置
        string savePath = EditorUtility.SaveFilePanel(
            "保存汇总文件",
            "",
            "ScriptsAllCode",
            "txt"
        );
        if (string.IsNullOrEmpty(savePath))
        {
            // 用户取消
            return;
        }

        // 获取所有 .cs 文件（包含子文件夹）
        string[] csFiles = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories);

        if (csFiles.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", $"在 {targetFolder} 中没有找到任何 .cs 文件", "确定");
            return;
        }

        // 开始拼接内容
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"// 汇总路径：{targetFolder}");
        sb.AppendLine($"// 文件总数：{csFiles.Length}");
        sb.AppendLine("// ========================================");
        sb.AppendLine();

        foreach (string file in csFiles)
        {
            // 转换为相对路径，方便阅读
            string relativePath = "Assets/Scripts" + file.Substring(fullPath.Length).Replace("\\", "/");
            sb.AppendLine($"// ===== 文件: {relativePath} =====");
            sb.AppendLine(File.ReadAllText(file));
            sb.AppendLine(); // 文件之间用空行隔开
        }

        // 写入文件
        File.WriteAllText(savePath, sb.ToString(), Encoding.UTF8);

        // 刷新资源数据库（如果保存在工程内，确保 Unity 能识别）
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("完成", $"已成功导出 {csFiles.Length} 个脚本文件到：\n{savePath}", "好的");
    }
}