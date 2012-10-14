@echo OFF

if exist bin\Release (
	copy tools\* bin\Release
	cd bin\Release
	zip EditorHelper.zip *.dll *.bat *.js
)
