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
    /// BuilderGenerated.cs 파일의 체이닝 메소드 자동 생성 에디터 클래스
    /// </summary>
    internal static class BuilderGenerator 
    {
        private const string OUTPUT_PATH = "Assets/2.Scripts/Runtime/Objects/BehaviourTree/Base/Builder/BuilderGenerated.cs";

        [MenuItem("TKTool/BehaviourTree/Generate C# builder code")]
        private static void Generate()
        {
            //Node 파생 Concreate 타입 수집
            var nodeTypes = AppDomain.CurrentDomain.GetAssemblies() //현재 .Net의 열려있는 Assembly의 Domain 접근
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(Node).IsAssignableFrom(t))
                .OrderBy(t => t.Name)
                .ToArray();

            //코드 빌드
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

            //파일저장 및 리프레쉬
            Directory.CreateDirectory(Path.GetDirectoryName(OUTPUT_PATH)!);
            File.WriteAllText(OUTPUT_PATH, sb.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
            Debug.Log("@BehaviourTree 체이닝 메서드 생성 완료");
        }

        /// <summary>
        /// 네임스페이스랑 class BTGenerator { 까지
        /// </summary>
        private static void SetHeader(StringBuilder sb)
        {
            sb.AppendLine("/*");
            sb.AppendLine(" [Builder Generated]");
            sb.AppendLine(" 유니티 에디터 툴로 클래스들 생성, 직접 수정 금지");
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
        /// 클래스랑 네임스페이스 scope 닫기
        /// </summary>
        private static void SetBottom(StringBuilder sb)
        {
            sb.AppendLine("\t}");
            sb.AppendLine("}");
        }

        /// <summary>BehaviourTree 파라미터를 첫 인자로 받는 가장 파라미터 많은 ctor</summary>
        private static ConstructorInfo? SelectBestConstructor(Type t)
        {
            return t.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Where((c) =>
                    c.GetParameters().Length > 0 &&
                    c.GetParameters()[0].ParameterType == typeof(BT))
                .OrderByDescending((c) => c.GetParameters().Length)
                .FirstOrDefault();
        }

        /// <summary>System.Single → float 등 C# 키워드 변환</summary>
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
