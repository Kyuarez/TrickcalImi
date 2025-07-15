#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;

namespace TK.BT
{
    /// <summary>
    /// BuilderGenerated.cs ������ ü�̴� �޼ҵ� �ڵ� ���� ������ Ŭ����
    /// </summary>
    internal static class BuilderGenerator 
    {
        private const string OUTPUT_PATH = "Assets/2.Scripts/Runtime/Objects/BehaviourTree/Base/Builder/BuilderGenerated.cs";

        [MenuItem("TKTool/BehaviourTree/Generate C# builder code")]
        private static void Generate()
        {
            //Node �Ļ� Concreate Ÿ�� ����
            var nodeTypes = AppDomain.CurrentDomain.GetAssemblies() //���� .Net�� �����ִ� Assembly�� Domain ����
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(Node).IsAssignableFrom(t))
                .OrderBy(t => t.Name)
                .ToArray();

            //�ڵ� ����
            var sb = new StringBuilder();
            SetHeader(sb);



            SetBottom(sb);

            //�������� �� ��������
            Directory.CreateDirectory(Path.GetDirectoryName(OUTPUT_PATH)!);
            File.WriteAllText(OUTPUT_PATH, sb.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
            Debug.Log("@BehaviourTree ü�̴� �޼��� ���� �Ϸ�");
        }

        /// <summary>
        /// ���ӽ����̽��� class BTGenerator { ����
        /// </summary>
        private static void SetHeader(StringBuilder sb)
        {
            sb.AppendLine("/*");
            sb.AppendLine(" [Builder Generated]");
            sb.AppendLine(" ����Ƽ ������ ���� Ŭ������ ����, ���� ���� ����");
            sb.AppendLine("*/");
            sb.AppendLine();
            sb.AppendLine("namespace TK.BT");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic partial class Builder");
            sb.AppendLine("\t{");
        }

        /// <summary>
        /// Ŭ������ ���ӽ����̽� scope �ݱ�
        /// </summary>
        private static void SetBottom(StringBuilder sb)
        {
            sb.AppendLine("\t}");
            sb.AppendLine("}");
        }
    }
}
#endif
