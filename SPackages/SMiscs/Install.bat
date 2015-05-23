@echo off
echo Linking Utils... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SMiscs\Utils\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Utils\"

echo Creating Resource folder...

mkdir "H:\GitHub\SAssemblies\SPackages\SMiscs\Resources"

echo Linking Dll's... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SMiscs\Resources\DLL\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\DLL\"

echo Linking Translations... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SMiscs\Resources\TRANSLATIONS\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\TRANSLATIONS\"

echo Creating Sprites folder...

mkdir "H:\GitHub\SAssemblies\SMiscs\SPackages\Resources\SPRITES"

echo Linking Sprites... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SMiscs\Resources\SPRITES\AutoLevler\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\SPRITES\AutoLevler\"
mklink /J "H:\GitHub\SAssemblies\SPackages\SMiscs\Resources\SPRITES\EloDisplayer\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\SPRITES\EloDisplayer\"
mklink /J "H:\GitHub\SAssemblies\SPackages\SMiscs\Resources\SPRITES\SmartPing\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\SPRITES\SmartPing\"

echo Linking Miscs... 

mklink /J "H:\GitHub\SAssemblies\SPackages\SMiscs\Miscs\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Miscs\"

echo Finished 
Pause