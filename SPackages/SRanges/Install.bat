@echo off
echo Linking Utils... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SRanges\Utils\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Utils\"

echo Creating Resource folder...

mkdir "H:\GitHub\SAssemblies\SPackages\SRanges\Resources"

echo Linking Dll's... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SRanges\Resources\DLL\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\DLL\"

echo Linking Translations... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SRanges\Resources\TRANSLATIONS\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\TRANSLATIONS\"

echo Linking Miscs... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SRanges\Ranges\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Ranges\"

echo Finished 
Pause