dotnet restore
dotnet pack -c Release System.Net.Http.HttpListener.UAP\project.json
dotnet pack -c Release System.Net.Http.HttpListener\project.json
pause