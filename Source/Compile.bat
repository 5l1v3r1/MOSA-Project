f exist %SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319 set MSBUILDPATH=%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319
if exist "%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin" set MSBUILDPATH="%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"
if exist "%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64" set MSBUILDPATH="%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64"

set MSBUILD=%MSBUILDPATH%\msbuild.exe

..\Tools\nuget\nuget.exe restore MOSA.sln

%MSBUILD% /nologo /m /p:BuildInParallel=true /p:Configuration=Release /p:Platform="Any CPU" MOSA.sln
