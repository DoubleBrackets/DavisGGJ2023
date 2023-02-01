#region

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

#endregion

namespace MushiCore.Editor
{
    public class AssetTemplateUtility
    {
        /// <summary>
        /// Create a new user-named script using a template text file
        /// </summary>
        /// <param name="templatePath">path to template file</param>
        /// <param name="defaultPath">default path for created asset (might change)</param>
        /// <param name="nameSymbol">pattern to replace with the script name (includes any specified postfix/prefix)</param>
        /// <param name="rawNameSymbol">pattern to replace with the script name (excludes postfix/prefix)</param>
        /// <param name="appendPostfix">postfix to append to end of name (if user provided name does not contain)</param>
        /// <param name="appendPrefix">prefix to append to end of name (if user provided name does not contain)</param>
        /// <param name="templateProcessors">custom template string processors</param>
        /// <param name="onAssetCreated">Callback when the asset has been created</param>
        public static void CreateNamedAssetFromTemplate(
            string templatePath,
            string defaultPath,
            string nameSymbol = "",
            string rawNameSymbol = "",
            string appendPostfix = "",
            string appendPrefix = "",
            TemplateStringProcessor[] templateProcessors = null,
            Action<string> onAssetCreated = null,
            string creationIcon = "cs Script Icon")
        {
            // Set up end file naming action 
            var endAction = ScriptableObject.CreateInstance<CreateScriptTemplateAction>();
            endAction.nameSymbol = nameSymbol;
            endAction.rawNameSymbol = rawNameSymbol;
            endAction.onAssetCreated = onAssetCreated;

            // Set up processors
            endAction.SetNameProcessors(
                new AddPostfix(appendPostfix, true),
                new AddPrefix(appendPrefix)
            );

            endAction.SetTemplateProcessors(templateProcessors);

            // Begin name editing (this also creates the new file)
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endAction,
                defaultPath,
                (Texture2D)EditorGUIUtility.IconContent(creationIcon).image,
                templatePath);
        }

        /// <summary>
        /// Create a script from a template without user naming
        /// </summary>
        /// <param name="templatePath">Path of template file</param>
        /// <param name="generatePath">Path of where the file should be created</param>
        /// <param name="templateProcessors">Any string processors to apply to the template</param>
        public static void CreateAssetFromTemplate(
            string templatePath,
            string generatePath,
            TemplateStringProcessor[] templateProcessors = null)
        {
            string fullTemplatePath = Path.GetFullPath(templatePath);
            var finalContent = new StringBuilder(File.ReadAllText(fullTemplatePath));

            foreach (var processor in templateProcessors)
            {
                processor.ProcessString(finalContent);
            }

            AssetCreateUtility.WriteTextAsset(generatePath, finalContent.ToString());
        }

        #region String Processors

        public interface TemplateStringProcessor
        {
            public StringBuilder ProcessString(StringBuilder input);
        }

        public class SimpleReplacement : TemplateStringProcessor
        {
            private string toReplacePattern;
            private string replacement;

            public SimpleReplacement(string toReplacePattern, string replacement)
            {
                this.toReplacePattern = toReplacePattern;
                this.replacement = replacement;
            }

            public StringBuilder ProcessString(StringBuilder input)
            {
                if (toReplacePattern == "") return input;
                return input.Replace(toReplacePattern, replacement);
            }
        }

        public class AddPrefix : TemplateStringProcessor
        {
            private string prefix;

            public AddPrefix(string prefix)
            {
                this.prefix = prefix;
            }

            public StringBuilder ProcessString(StringBuilder input)
            {
                if (prefix == "") return input;
                return input.Insert(0, prefix);
            }
        }

        public class AddPostfix : TemplateStringProcessor
        {
            private string postfix;
            private bool applyBeforeExtension;

            public AddPostfix(string postfix, bool applyBeforeExtension = true)
            {
                this.postfix = postfix;
                this.applyBeforeExtension = applyBeforeExtension;
            }

            public StringBuilder ProcessString(StringBuilder input)
            {
                if (postfix == "") return input;
                if (applyBeforeExtension)
                {
                    string extension = Path.GetExtension(input.ToString());
                    string str = Path.GetFileNameWithoutExtension(input.ToString());
                    if (!str.EndsWith(postfix))
                    {
                        str += postfix;
                        input.Clear();
                        input.Append(str);
                        input.Append(extension);
                    }
                }
                else
                {
                    input.Append(postfix);
                }

                return input;
            }
        }

        #endregion

        private class CreateScriptTemplateAction : EndNameEditAction
        {
            public string nameSymbol;
            public string rawNameSymbol;
            
            /// <summary>
            /// Called when the asset is created, returning the asset path
            /// </summary>
            public Action<string> onAssetCreated;

            private TemplateStringProcessor[] nameProcessors;
            private TemplateStringProcessor[] templateProcessors;

            public void SetTemplateProcessors(params TemplateStringProcessor[] processors)
            {
                templateProcessors = processors;
            }

            public void SetNameProcessors(params TemplateStringProcessor[] processors)
            {
                nameProcessors = processors;
            }

            /// <summary>
            /// Called when the file name editing finishes
            /// </summary>
            /// <param name="instanceId"></param>
            /// <param name="pathName"></param>
            /// <param name="resourceFile"></param>
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                // Name formatting
                string rawFileName = Path.GetFileName(pathName).Trim();
                var processedFileName = new StringBuilder(rawFileName);

                if (nameProcessors != null)
                {
                    foreach (var nameProcessor in nameProcessors)
                    {
                        nameProcessor.ProcessString(processedFileName);
                    }
                }

                // Update path name
                pathName = pathName.Replace(rawFileName, processedFileName.ToString());

                // Start processing actual content
                // Start by reading from template file
                var finalContent = new StringBuilder(File.ReadAllText(resourceFile));

                // Swap out replacement symbol for formatted script name
                var swapNamePattern = new SimpleReplacement(
                    nameSymbol,
                    Path.GetFileNameWithoutExtension(processedFileName.ToString()));

                var swapRawName = new SimpleReplacement(
                    rawNameSymbol,
                    Path.GetFileNameWithoutExtension(rawFileName)
                );

                swapNamePattern.ProcessString(finalContent);
                swapRawName.ProcessString(finalContent);

                if (templateProcessors != null)
                {
                    foreach (var templateProcessor in templateProcessors)
                    {
                        templateProcessor.ProcessString(finalContent);
                    }
                }

                // Write processed text into the file
                AssetCreateUtility.WriteTextAsset(pathName, finalContent.ToString());
                onAssetCreated?.Invoke(pathName);
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                base.Cancelled(instanceId, pathName, resourceFile);
            }
        }
    }
}