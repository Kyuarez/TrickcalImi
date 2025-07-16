#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Reflection;

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

            foreach (var type in nodeTypes)
            {
                if(type.Name == nameof(Builder))
                {
                    continue;
                }

                if(type.IsDefined(typeof(ExcludeFromBuilderAttribute), inherit: false))
                {
                    continue;
                }

                var constructor = SelectBestConstructor(type);
                
                if(constructor == null)
                {
                    continue;
                }

                SetMethodByConstructor(sb, type, constructor);
            }

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

        private static void SetMethodByConstructor(StringBuilder sb, Type type, ConstructorInfo info)
        {
            var ps = info.GetParameters()
                .Where((p) => p.ParameterType != typeof(BT))
                .ToArray();

            string paramList = string.Join(", ", ps.Select((p) => $"{GetFriendlyTypeName(p.ParameterType)} {p.Name}"));
            sb.AppendLine($"\t\tpublic Builder {type.Name}({paramList})");
            sb.AppendLine("\t\t{");

            sb.AppendLine("\t\t\treturn this;");
            sb.AppendLine("\t\t}");
            sb.AppendLine();
        }

        /// <summary>
        /// Ŭ������ ���ӽ����̽� scope �ݱ�
        /// </summary>
        private static void SetBottom(StringBuilder sb)
        {
            sb.AppendLine("\t}");
            sb.AppendLine("}");
        }

        /// <summary>BehaviourTree �Ķ���͸� ù ���ڷ� �޴� ���� �Ķ���� ���� ctor</summary>
        private static ConstructorInfo? SelectBestConstructor(Type t)
        {
            return t.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Where((c) =>
                    c.GetParameters().Length > 0 &&
                    c.GetParameters()[0].ParameterType == typeof(BT))
                .OrderByDescending((c) => c.GetParameters().Length)
                .FirstOrDefault();
        }

        /// <summary>System.Single �� float �� C# Ű���� ��ȯ</summary>
        private static string GetFriendlyTypeName(Type t)
        {
            return t switch
            {
                { } when t == typeof(int) => "int",
                { } when t == typeof(float) => "float",
                { } when t == typeof(string) => "string",
                { } when t == typeof(bool) => "bool",
                _ => t.Name
            };
        }
    }
}
#endif
