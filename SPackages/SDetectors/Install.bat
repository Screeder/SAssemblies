@echo off
echo Linking Utils... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SDetectors\Utils\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Utils\"

echo Creating Resource folder...

mkdir "H:\GitHub\SAssemblies\SPackages\SDetectors\Resources"

echo Linking Dll's... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SDetectors\Resources\DLL\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\DLL\"

echo Linking Translations... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SDetectors\Resources\TRANSLATIONS\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\TRANSLATIONS\"

echo Linking Miscs... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SDetectors\Detectors\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Detectors\"

echo Finished 
Pause