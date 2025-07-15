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

        /// <summary>
        /// 클래스랑 네임스페이스 scope 닫기
        /// </summary>
        private static void SetBottom(StringBuilder sb)
        {
            sb.AppendLine("\t}");
            sb.AppendLine("}");
        }
    }
}
#endif
