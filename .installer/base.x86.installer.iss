#ifndef AppVersion
#define AppVersion "0.1.0"
#endif
[Setup]
AppId={{B5A5B85A-7E22-46ED-AE61-BASE00000001}
DefaultDirName={userappdata}\base
OutputDir=Output
OutputBaseFilename=base-x86-installer
ArchitecturesAllowed=x86compatible
ArchitecturesInstallIn64BitMode=x86compatible
Uninstallable=yes
[Files]
Source: "..\publish\build\x86\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
#include "base.installer.iss"
