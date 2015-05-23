@echo off
echo Linking Utils... 

mklink /J "H:\GitHub\SAssemblies\SPackages\STrackers\Utils\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Utils\"

echo Creating Resource folder...

mkdir "H:\GitHub\SAssemblies\SPackages\STrackers\Resources"

echo Linking Dll's... 

mklink /J "H:\GitHub\SAssemblies\SPackages\STrackers\Resources\DLL\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\DLL\"

echo Linking Translations... 

mklink /J "H:\GitHub\SAssemblies\SPackages\STrackers\Resources\TRANSLATIONS\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\TRANSLATIONS\"

echo Creating Sprites folder...

mkdir "H:\GitHub\SAssemblies\SPackages\STrackers\Resources\SPRITES"

echo Linking Sprites... 

mklink /J "H:\GitHub\SAssemblies\SPackages\STrackers\Resources\SPRITES\Ui\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Resources\SPRITES\Ui\"

echo Linking Miscs... 

mklink /J "H:\GitHub\SAssemblies\SPackages\STrackers\Trackers\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Trackers\"

echo Linking Spectator... 

mklink /J "H:\GitHub\SAssemblies\SPackages\STrackers\Spectator\" "H:\Visual Studio 2012\Projects\SAwareness\SAwareness\Spectator\"

echo Finished 
Pause