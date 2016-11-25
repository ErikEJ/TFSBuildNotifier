Sample API Url:

http://<host>/<Collection>/<project>/_apis/build/builds?definitions=865&$top=1&api-version=2.0&statusFilter=completed

Sample command line:

TFSBuildNotifier.exe "http://<host>/<Collection>/<project>/_apis/build/builds?definitions=865&$top=1&api-version=2.0&statusFilter=completed" "http://<host>/<Collection>/<project>/_apis/build/builds?definitions=837&$top=1&api-version=2.0&statusFilter=completed"
