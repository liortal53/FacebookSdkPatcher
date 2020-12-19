using System;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace Patcher
{
    class Program
    {
        private const string MODIFIED_CLASS_NAME = @"Facebook.Unity.Mobile.Android.AndroidFacebookGameObject";

        private static readonly string[] REMOVED_METHODS = new[]
        {
            "OnAwake",
            "OnEnable",
            "OnDisable"
        };

        static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Usage: FacebookUnityPatcher.exe <full path to Facebook.Unity.dll>");

                return;
            }

            var assemblyPath = args[0];

            if (!File.Exists(assemblyPath))
            {
                Console.WriteLine($"Assembly doesn't exist: {assemblyPath}");

                return;
            }

            // Split into path/filename since we want to create a new name for saving
            var folder = Path.GetDirectoryName(assemblyPath);
            var fileName = Path.GetFileNameWithoutExtension(assemblyPath);

            // Get an AssemblyDefinition to work with.
            var asm = AssemblyDefinition.ReadAssembly(assemblyPath);

            // Get the TypeDefinition of the clas we want to work with.
            var type = asm.MainModule.Types.Where(typeDefinition => typeDefinition.FullName == MODIFIED_CLASS_NAME).FirstOrDefault();

            if (type == null)
            {
                Console.WriteLine($"Type {MODIFIED_CLASS_NAME} was not found!");

                return;
            }

            // Get all the methods to remove from the TypeDefinition.
            var methodDefitionsToRemove = type.Methods.Where(methodDefinition => REMOVED_METHODS.Contains(methodDefinition.Name)).ToArray();

            // Iterate and remove methods
            foreach (var md in methodDefitionsToRemove)
            {
                type.Methods.Remove(md);
            }

            // Save the modified AssemblyDefinition to disk.
            asm.Write(Path.Combine(folder, $"{fileName}.Modified.dll"));
        }
    }
}