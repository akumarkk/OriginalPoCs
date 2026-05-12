using System;
using System.IO;

namespace MyTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "begin")
            {
                ExecuteBegin();
            }
            else if (args.Length > 0 && args[0].ToLower() == "end")
            {
                ExecuteEnd();
            }
            else
            {
                Console.WriteLine("Usage: mytool [begin|end]");
            }
        }

        static void ExecuteBegin()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string injectionDir = Path.Combine(localAppData, @"Microsoft\MSBuild\Current\Microsoft.Common.targets\ImportBefore");
            
            Directory.CreateDirectory(injectionDir);

            string targetsPath = Path.Combine(injectionDir, "MyTool.Global.targets");
            string targetsContent = 
@"<Project>
  <Target Name='MyCustomGlobalInjection' BeforeTargets='BeforeBuild'>
    <Message Importance='high' Text='--- [MyTool GLOBAL] Injecting Custom Logic into $(ProjectName) ---' />
  </Target>
</Project>";
            File.WriteAllText(targetsPath, targetsContent);

            Console.WriteLine($"✅ MyTool global injection enabled. Targets file created at: {targetsPath}");
        }

        static void ExecuteEnd()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string targetsPath = Path.Combine(localAppData, @"Microsoft\MSBuild\Current\Microsoft.Common.targets\ImportBefore\MyTool.Global.targets");

            if (File.Exists(targetsPath))
            {
                File.Delete(targetsPath);
                Console.WriteLine("✅ MyTool global injection removed.");
            }
        }
    }
}