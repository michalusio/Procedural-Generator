using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UnityBuilderAction
{
    public static class BuildScript
    {
        private static readonly string Eol = Environment.NewLine;

        private static readonly string[] Secrets =
            {"androidKeystorePass", "androidKeyaliasName", "androidKeyaliasPass"};

        public static void Build()
        {
            // Gather values from args
            Dictionary<string, string> options = GetValidatedOptions();

            // Set version for this build
            PlayerSettings.bundleVersion = options["buildVersion"];
            PlayerSettings.macOS.buildNumber = options["buildVersion"];
            PlayerSettings.Android.bundleVersionCode = int.Parse(options["androidVersionCode"]);

            // Apply build target
            var buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), options["buildTarget"]);
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    {
                        EditorUserBuildSettings.buildAppBundle = options["customBuildPath"].EndsWith(".aab");
                        if (options.TryGetValue("androidKeystoreName", out string keystoreName) &&
                            !string.IsNullOrEmpty(keystoreName))
                            PlayerSettings.Android.keystoreName = keystoreName;
                        if (options.TryGetValue("androidKeystorePass", out string keystorePass) &&
                            !string.IsNullOrEmpty(keystorePass))
                            PlayerSettings.Android.keystorePass = keystorePass;
                        if (options.TryGetValue("androidKeyaliasName", out string keyaliasName) &&
                            !string.IsNullOrEmpty(keyaliasName))
                            PlayerSettings.Android.keyaliasName = keyaliasName;
                        if (options.TryGetValue("androidKeyaliasPass", out string keyaliasPass) &&
                            !string.IsNullOrEmpty(keyaliasPass))
                            PlayerSettings.Android.keyaliasPass = keyaliasPass;
                        break;
                    }
                case BuildTarget.StandaloneOSX:
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
                    break;
            }

            BuildPackageInternal(true);
        }

        private static Dictionary<string, string> GetValidatedOptions()
        {
            ParseCommandLineArguments(out Dictionary<string, string> validatedOptions);

            if (!validatedOptions.TryGetValue("projectPath", out string _))
            {
                Console.WriteLine("Missing argument -projectPath");
                EditorApplication.Exit(110);
            }

            if (!validatedOptions.TryGetValue("buildTarget", out string buildTarget))
            {
                Console.WriteLine("Missing argument -buildTarget");
                EditorApplication.Exit(120);
            }

            if (!Enum.IsDefined(typeof(BuildTarget), buildTarget ?? string.Empty))
            {
                EditorApplication.Exit(121);
            }

            if (!validatedOptions.TryGetValue("customBuildPath", out string _))
            {
                Console.WriteLine("Missing argument -customBuildPath");
                EditorApplication.Exit(130);
            }

            const string defaultCustomBuildName = "TestBuild";
            if (!validatedOptions.TryGetValue("customBuildName", out string customBuildName))
            {
                Console.WriteLine($"Missing argument -customBuildName, defaulting to {defaultCustomBuildName}.");
                validatedOptions.Add("customBuildName", defaultCustomBuildName);
            }
            else if (customBuildName == "")
            {
                Console.WriteLine($"Invalid argument -customBuildName, defaulting to {defaultCustomBuildName}.");
                validatedOptions.Add("customBuildName", defaultCustomBuildName);
            }

            return validatedOptions;
        }

        private static void ParseCommandLineArguments(out Dictionary<string, string> providedArguments)
        {
            providedArguments = new Dictionary<string, string>();
            string[] args = Environment.GetCommandLineArgs();

            Console.WriteLine(
                $"{Eol}" +
                $"###########################{Eol}" +
                $"#    Parsing settings     #{Eol}" +
                $"###########################{Eol}" +
                $"{Eol}"
            );

            // Extract flags with optional values
            for (int current = 0, next = 1; current < args.Length; current++, next++)
            {
                // Parse flag
                bool isFlag = args[current].StartsWith("-");
                if (!isFlag) continue;
                string flag = args[current].TrimStart('-');

                // Parse optional value
                bool flagHasValue = next < args.Length && !args[next].StartsWith("-");
                string value = flagHasValue ? args[next].TrimStart('-') : "";
                bool secret = Secrets.Contains(flag);
                string displayValue = secret ? "*HIDDEN*" : "\"" + value + "\"";

                // Assign
                Console.WriteLine($"Found flag \"{flag}\" with value {displayValue}.");
                providedArguments.Add(flag, value);
            }
        }

        [MenuItem("File/Build Package", priority = 220)]
        public static void BuildPackage()
        {
            BuildPackageInternal(false);
        }

        private static void BuildPackageInternal(bool exit)
        {
            try
            {
                var packageDir = Path.Combine(Environment.CurrentDirectory, "build");
                if (!Directory.Exists(packageDir)) Directory.CreateDirectory(packageDir);
                AssetDatabase.ExportPackage(
                    "Packages/pl.lochalhost.procedural-generator",
                    Path.Combine(packageDir, "ProceduralGenerator.unitypackage"),
                    ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
                );
                if (exit) ExitWithResult(BuildResult.Succeeded);
            }
            catch (Exception e)
            {
                if (exit) ExitWithResult(BuildResult.Failed);
                else Debug.LogException(e);
            }
        }

        private static void ExitWithResult(BuildResult result)
        {
            switch (result)
            {
                case BuildResult.Succeeded:
                    Console.WriteLine("Build succeeded!");
                    EditorApplication.Exit(0);
                    break;
                case BuildResult.Failed:
                    Console.WriteLine("Build failed!");
                    EditorApplication.Exit(101);
                    break;
                case BuildResult.Cancelled:
                    Console.WriteLine("Build cancelled!");
                    EditorApplication.Exit(102);
                    break;
                case BuildResult.Unknown:
                default:
                    Console.WriteLine("Build result is unknown!");
                    EditorApplication.Exit(103);
                    break;
            }
        }
    }
}