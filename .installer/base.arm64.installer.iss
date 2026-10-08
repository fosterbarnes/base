#ifndef AppVersion
#define AppVersion "0.1.0"
#endif
[Setup]
AppId={{B5A5B85A-7E22-46ED-AE61-BASE00000003}
DefaultDirName={userappdata}\base
OutputDir=Output
OutputBaseFilename=base-arm64-installer
ArchitecturesAllowed=arm64
ArchitecturesInstallIn64BitMode=arm64
Uninstallable=yes
[Files]
Source: "..\publish\build\arm64\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
#include "base.installer.iss"
