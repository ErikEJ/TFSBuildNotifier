# TFS Build Notifier

TFS 2015+ on premise (Build 2.0) notifier/watcher - Currently in alpha state

Sample command line:

´´´´
TFSBuildNotifier.exe "http://host/Collection/project/_apis/build/builds?definitions=865&$top=1&api-version=2.0&statusFilter=completed"
´´´´

(You can have multiple Urls on the command line, separated by spaces)

## Latest version of the application
You can download a .zip file with the [latest build of the master branch from AppVeyor](https://ci.appveyor.com/api/projects/ErikEJ/TFSBuildNotifier/artifacts/TfsBuildNotifier.zip?branch=master)
