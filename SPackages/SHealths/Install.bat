@echo off
echo Linking Utils... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SHealths\Utils\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Utils\"

echo Creating Resource folder...

mkdir "H:\GitHub\SAssemblies\SPackages\SHealths\Resources"

echo Linking Dll's... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SHealths\Resources\DLL\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\DLL\"

echo Linking Translations... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SHealths\Resources\TRANSLATIONS\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\TRANSLATIONS\"

echo Linking Miscs... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SHealths\Healths\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Healths\"

echo Finished 
Pause