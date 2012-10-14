@echo OFF
set PATH=%PATH%;%WINDIR%\Microsoft.NET\Framework\v4.0.30319

RegAsm.exe /codebase EditorHelper.dll
::pause
