# TFS Build Notifier

TFS 2015+ on premise (Build 2.0) notifier/watcher

Currently in alpha state

Sample API Url: 

http://<host>/<Collection>/<project>/_apis/build/builds?definitions=865&$top=1&api-version=2.0&statusFilter=completed

Sample command line:

TFSBuildNotifier.exe "http://<host>/<Collection>/<project>/_apis/build/builds?definitions=865&$top=1&api-version=2.0&statusFilter=completed" "
