#r "../packages/WebSharper.TypeScript/tools/net40/IntelliFactory.WebSharper.Core.dll"
#r "../packages/WebSharper/tools/net40/IntelliFactory.WebSharper.JQuery.dll"
#r "../packages/WebSharper.TypeScript/tools/net40/IntelliFactory.WebSharper.TypeScript.dll"
//#r "C:/dev/websharper.typescript/build/Release/IntelliFactory.WebSharper.TypeScript.dll"
#load "utility.fsx"

open System
open System.IO
module C = IntelliFactory.WebSharper.TypeScript.Compiler
module U = Utility
type JQuery = IntelliFactory.WebSharper.JQuery.Resources.JQuery

let dts = U.loc ["typings/knockout.d.ts"]
let lib = U.loc ["packages/WebSharper.TypeScript.Lib/lib/net40/IntelliFactory.WebSharper.TypeScript.Lib.dll"]
let snk = U.loc [Environment.GetEnvironmentVariable("INTELLIFACTORY"); "keys/IntelliFactory.snk"]

let fsCore =
    U.loc [
        Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        "Reference Assemblies/Microsoft/FSharp/.NETFramework/v4.0/4.3.0.0/FSharp.Core.dll"
    ]

let opts =
    {
        C.Options.Create("IntelliFactory.WebSharper.Knockout", [dts]) with
            AssemblyVersion = Some (Version "3.0.0.0")
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

match result.CompiledAssembly with
| None -> ()
| Some asm ->
    let out = U.loc ["build/IntelliFactory.WebSharper.Knockout.dll"]
    let dir = DirectoryInfo(Path.GetDirectoryName(out))
    if not dir.Exists then
        dir.Create()
    printfn "Writing %s" out
    File.WriteAllBytes(out, asm.GetBytes())

let (|I|_|) (x: string) =
    match x with
    | null | "" -> None
    | n ->
        match Int32.TryParse(n) with
        | true, r -> Some r
        | _ -> None

let version =
    match Environment.GetEnvironmentVariable("BUILD_NUMBER") with
    | I k -> Version(3, 0, k, 0).ToString() + "-alpha"
    | _ -> Version(3, 0, 0, 0).ToString() + "-alpha"

let ok =
    match Environment.GetEnvironmentVariable("NuGetPackageOutputPath") with
    | null | "" ->
        U.nuget (sprintf "pack -out build/ -version %s Knockout.nuspec" version)
    | path ->
        U.nuget (sprintf "pack -out %s -version %s Knockout.nuspec" path version)

printfn "pack: %b" ok
if not ok then exit 1 else exit 0
