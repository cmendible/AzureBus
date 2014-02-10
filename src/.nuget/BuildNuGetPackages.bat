IF exist "package/" (echo "ok") ELSE (mkdir "package/")
FOR  %%f in ("*.nuspec") do CALL "NuGet.exe" pack "%CD%/%%f" -o "%CD%/package/"

pause		