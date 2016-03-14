set /p version=Enter Package Version: 

IF exist ".nuget/package/" (echo "ok") ELSE (mkdir ".nuget/package/")
FOR /R %%f in (*.nuspec) do CALL ".nuget/NuGet.exe" pack "%%f" -o "%CD%/.nuget/package/" -Version %version%

pause