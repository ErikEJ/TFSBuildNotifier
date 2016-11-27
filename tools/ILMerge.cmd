echo Running ilmerge...
rem .\ilmerge /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /out:..\TfsBuildNotifier.exe ..\src\TfsBuildNotifier\bin\release\TfsBuildNotifier.exe ..\src\TfsBuildNotifier\bin\release\Hardcodet.Wpf.TaskbarNotification.dll

.\ilmerge /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /out:..\TfsBuildNotifier.exe ..\src\TfsBuildNotifier\bin\release\TfsBuildNotifier.exe ..\src\TfsBuildNotifier\bin\release\Newtonsoft.Json.dll ..\src\TfsBuildNotifier\bin\release\Hardcodet.Wpf.TaskbarNotification.dll
