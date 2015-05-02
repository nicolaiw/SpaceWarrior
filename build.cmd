@echo off
cls
".nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
"packages\FAKE.3.30.3\tools\Fake.exe" build.fsx
pause