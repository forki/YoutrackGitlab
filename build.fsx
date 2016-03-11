// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing.NUnit3

RestorePackages()

// Properties
let buildDir = "./build/"
let testDir  = "./test/"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "BuildApp" (fun _ ->
   !! "*.sln"
     |> MSBuildRelease buildDir "Build"
     |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (buildDir + "/*.Tests.dll")
      |> NUnit3 (fun (p:NUnit3Params) ->
          {p with
             ShadowCopy = true})
)

Target "Default" (fun _ ->
    ()
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "Test"
  ==> "Default"

// start build
RunTargetOrDefault "Default"