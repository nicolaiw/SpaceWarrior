@echo off
cls
echo start building SpaceWarrior ...
echo this may take a while ...
echo please wait ...
".nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
"packages\FAKE\tools\Fake.exe" build.fsx
pause