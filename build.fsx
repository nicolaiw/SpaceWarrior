// include Fake lib
#r "packages/FAKE.3.30.3/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let buildDir = "./build/"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "BuildApp" (fun _ ->
    !! "src/**/*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "Default"

// start build
RunTargetOrDefault "Default"

