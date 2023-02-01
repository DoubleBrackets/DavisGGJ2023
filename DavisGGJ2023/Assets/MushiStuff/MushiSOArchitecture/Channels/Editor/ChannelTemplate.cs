#region

using System.Text;
using MushiCore.Editor;
using UnityEditor;

#endregion

public class ChannelTemplate
{
    private static string path = "Assets/MushiStuff/MushiSOArchitecture/Channels/Editor";

    [MenuItem("Assets/Create/MushiStuff/MushiSOArchitecture/Event Channel Script", false, 0)]
    public static void CreateEventChannelScript()
    {
        AssetTemplateUtility.CreateNamedAssetFromTemplate(
            $"{path}/EventChannelScriptTemplate.txt",
            "EventChannelSO.cs",
            "#FILENAME#",
            "#RAWFILENAME#",
            "EventChannelSO"
        );
    }

    [MenuItem("Assets/Create/MushiStuff/MushiSOArchitecture/Func Channel Script", false, 0)]
    public static void CreateFuncChannelScript()
    {
        AssetTemplateUtility.CreateNamedAssetFromTemplate(
            $"{path}/FuncChannelScriptTemplate.txt",
            "ChannelSO.cs",
            "#FILENAME#",
            "#RAWFILENAME#",
            "FuncChannelSO"
        );
    }

    private const int eventArgs = 8;
    private const int funcArgs = 8;
    private static string genericChannelsPath = "Assets/MushiStuff/MushiSOArchitecture/Channels";

    [MenuItem("Assets/Create/MushiStuff/MushiSOArchitecture/Generate Base Generic Channels Script", false, 0)]
    public static void CreateGenericChannelScript()
    {
        var processors = new AssetTemplateUtility.TemplateStringProcessor[] { new GenericChannelsGenerator() };
        AssetTemplateUtility.CreateAssetFromTemplate(
            $"{path}/GenericChannelsScriptTemplate.txt",
            $"{genericChannelsPath}/GenericChannelsSO.cs",
            templateProcessors: processors
        );
    }

    private class GenericChannelsGenerator : AssetTemplateUtility.TemplateStringProcessor
    {
        public StringBuilder ProcessString(StringBuilder input)
        {
            string raw = input.ToString();
            string eventBodyTag = "#EVENT_BODY";
            string funcBodyTag = "#FUNC_BODY";
            var eventBody = new StringBuilder(raw.Split(eventBodyTag)[1]);
            var funcBody = new StringBuilder(raw.Split(funcBodyTag)[1]);

            StringBuilder buildGenerics = new StringBuilder();

            void AddLine(string str)
            {
                buildGenerics.Append(str + "\n");
            }

            // Usings
            AddLine("using System;");
            AddLine("using UnityEngine;");

            var genericParams = new StringBuilder();
            var raiseParams = new StringBuilder();
            var invokeParams = new StringBuilder();

            // Events
            void CreateGenericEvent(string generic, string raise, string invoke)
            {
                var body = new StringBuilder();
                body.Append(eventBody);
                body.Replace("#PARAMS#", generic);
                body.Replace("#RAISE_PARAMS#", raise);
                body.Replace("#INVOKE_PARAMS#", invoke);
                AddLine(body.ToString());
            }

            CreateGenericEvent("", "", "");

            genericParams.Append("<");

            for (int i = 1; i <= eventArgs; i++)
            {
                genericParams.Append($"T{i},");
                raiseParams.Append($"T{i} arg{i},");
                invokeParams.Append($"arg{i},");

                CreateGenericEvent(
                    genericParams.ToString().TrimEnd(',') + ">",
                    raiseParams.ToString().TrimEnd(','),
                    invokeParams.ToString().TrimEnd(','));
            }

            // Funcs
            genericParams.Clear();
            raiseParams.Clear();
            invokeParams.Clear();

            void CreateGenericFunc(string generic, string raise, string invoke)
            {
                var body = new StringBuilder();
                body.Append(funcBody);
                body.Replace("#PARAMS#", generic);
                body.Replace("#RAISE_PARAMS#", raise);
                body.Replace("#INVOKE_PARAMS#", invoke);
                AddLine(body.ToString());
            }

            CreateGenericFunc("", "", "");

            for (int i = 1; i <= eventArgs; i++)
            {
                genericParams.Append($"T{i},");
                raiseParams.Append($"T{i} t{i},");
                invokeParams.Append($"t{i},");

                CreateGenericFunc(
                    genericParams.ToString(),
                    raiseParams.ToString().TrimEnd(','),
                    invokeParams.ToString().TrimEnd(','));
            }

            input.Clear();
            input.Append(buildGenerics);
            return buildGenerics;
        }
    }
}