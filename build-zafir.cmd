@ECHO OFF
setlocal
set PATH=%PATH%;%ProgramFiles(x86)%\Microsoft SDKs\F#\4.0\Framework\v4.0
set PATH=%PATH%;%ProgramFiles%\Microsoft SDKs\F#\4.0\Framework\v4.0

if exist "%ProgramFiles(x86)%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsi.exe" (
   xcopy /y /q "%ProgramFiles(x86)%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsi.exe" tools
   xcopy /y /q "%ProgramFiles(x86)%\Microsoft SDKs\F#\4.0\Framework\v4.0\FSharp.Compiler.Interactive.Settings.dll" tools
)

if exist "%ProgramFiles%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsi.exe" (
   xcopy /y /q "%ProgramFiles%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsi.exe" tools
   xcopy /y /q "%ProgramFiles%\Microsoft SDKs\F#\4.0\Framework\v4.0\FSharp.Compiler.Interactive.Settings.dll" tools
)

tools\fsi.exe --exec tools/configure-zafir.fsx
tools\fsi.exe --exec tools/build-zafir.fsx
