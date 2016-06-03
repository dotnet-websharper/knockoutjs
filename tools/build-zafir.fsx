#r "../packages/Zafir.TypeScript/tools/net40/WebSharper.Core.dll"
#r "../packages/Zafir.TypeScript/tools/net40/WebSharper.TypeScript.dll"
//#r "C:/dev/websharper.typescript/build/Release/WebSharper.TypeScript.dll"
#I "../packages/NuGet.Core/lib/net40-client"
#r "NuGet.Core"
#r "../packages/IntelliFactory.Core/lib/net45/IntelliFactory.Core.dll"
#r "../packages/IntelliFactory.Build/lib/net45/IntelliFactory.Build.dll"
#load "utility.fsx"

open System
open System.IO
module C = WebSharper.TypeScript.Compiler
module U = Utility

open IntelliFactory.Build

let bt = BuildTool().PackageId("Zafir.Knockout").VersionFrom("Zafir")

let asmVersion =
    let v = PackageVersion.Full.Find(bt)
    sprintf "%i.%i.0.0" v.Major v.Minor

let dts = U.loc ["typings/knockout.d.ts"]
let lib = U.loc ["packages/Zafir.TypeScript.Lib/lib/net40/WebSharper.TypeScript.Lib.dll"]
let snk = U.loc [Environment.GetEnvironmentVariable("INTELLIFACTORY"); "keys/IntelliFactory.snk"]

let fsCore =
    U.loc [
        Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        "Reference Assemblies/Microsoft/FSharp/.NETFramework/v4.0/4.3.0.0/FSharp.Core.dll"
    ]

let opts =
    {
        C.Options.Create("WebSharper.Knockout", [dts]) with
            AssemblyVersion = Some (Version asmVersion)
            Renaming = C.Renaming.RemovePrefix ""
            References = [C.ReferenceAssembly.File lib; C.ReferenceAssembly.File fsCore]
            StrongNameKeyFile = Some snk
            Verbosity = C.Level.Verbose
            EmbeddedResources =
                [
                    C.EmbeddedResource.FromFile("js/knockout-min.js")
                ]
            WebSharperResources =
                [
                    C.WebSharperResource.Create("Knockout", "knockout-min.js")
                ]
    }

let result =
    C.Compile opts

for msg in result.Messages do
    printfn "%O" msg

let tlibVerson = bt.NuGetResolver.FindLatestVersion("Zafir.TypeScript.Lib", true).Value.ToString()

match result.CompiledAssembly with
| None -> ()
| Some asm ->
    let out = U.loc ["build/WebSharper.Knockout.dll"]
    let dir = DirectoryInfo(Path.GetDirectoryName(out))
    if not dir.Exists then
        dir.Create()
    printfn "Writing %s" out
    File.WriteAllBytes(out, asm.GetBytes())

    bt.Solution [
        bt.NuGet.CreatePackage()
            .Configure(fun c ->
                { c with
                    Authors = ["IntelliFactory"]
                    Title = Some "Zafir.Knockout 3.1.0"
                    LicenseUrl = Some "http://websharper.com/licensing"
                    ProjectUrl = Some "http://websharper.com"
                    Description = "Zafir bindings for Knockout (3.1.0)"
                    RequiresLicenseAcceptance = true })
            .AddDependency("Zafir.TypeScript.Lib", tlibVerson, forceFoundVersion = true)
            .AddFile("build/WebSharper.Knockout.dll", "lib/net40/WebSharper.Knockout.dll")
            .AddFile("README.md", "docs/README.md")
    ]
    |> bt.Dispatch
