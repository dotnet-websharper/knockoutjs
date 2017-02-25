#I "../packages/NuGet.Core/lib/net40-client"
#r "NuGet.Core"
#r "../packages/IntelliFactory.Core/lib/net45/IntelliFactory.Core.dll"
#r "../packages/IntelliFactory.Build/lib/net45/IntelliFactory.Build.dll"

open System
open System.IO
open IntelliFactory.Build

#load "utility.fsx"

try Directory.Delete(Utility.loc ["packages"], true) with _ -> ()
let ok =
    Utility.nuget "install Zafir -pre -o packages -excludeVersion -nocache"
    && Utility.nuget "install Zafir.TypeScript -pre -o packages -excludeVersion -nocache"
    && Utility.nuget "install Zafir.TypeScript.Lib -pre -o packages -excludeVersion -nocache"
    && Utility.nuget "install IntelliFactory.Build -pre -o packages -excludeVersion -nocache"

let bt = BuildTool().PackageId("Zafir.Knockout").VersionFrom("Zafir")
File.WriteAllText(__SOURCE_DIRECTORY__ + "/version.txt", PackageVersion.Full.Find(bt).ToString())

printfn "configure: %b" ok
if not ok then exit 1 else exit 0
