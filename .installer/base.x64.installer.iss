#ifndef AppVersion
#define AppVersion "0.1.0"
#endif
[Setup]
AppId={{B5A5B85A-7E22-46ED-AE61-BASE00000002}
DefaultDirName={userappdata}\base
OutputDir=Output
OutputBaseFilename=base-x64-installer
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
Uninstallable=yes
[Files]
Source: "..\publish\build\x64\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
#include "base.installer.iss"
