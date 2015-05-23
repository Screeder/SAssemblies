@echo off
echo Linking Utils... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SWards\Utils\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Utils\"

echo Creating Resource folder...

mkdir "H:\GitHub\SAssemblies\SPackages\SWards\Resources"

echo Linking Dll's... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SWards\Resources\DLL\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\DLL\"

echo Linking Translations... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SWards\Resources\TRANSLATIONS\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\TRANSLATIONS\"

echo Linking Miscs... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SWards\Wards\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Wards\"

echo Finished 
Pause