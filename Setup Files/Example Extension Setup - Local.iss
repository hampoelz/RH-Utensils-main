#define ExtensionName           "ExampleExtension"
#define ExtensionVersion        "0.1"
#define ExtensionPublisher      "RH Utensils"
#define ExtensionPublisherURL   "https://rh-utensils.hampoelz.net/"
#define ExtensionSupportURL     "https://github.com/rh-utensils/main/issues/"
#define ExtensionUpdatesURL     "https://github.com/rh-utensils/main/releases/"
#define ZipFileLink             "https://github.com/rh-utensils/main/releases/download/" + ExtensionVersion + "/ExampleExtension.zip"

[Setup]
AppId                    = {#ExtensionPublisher}_{#ExtensionName}_Local
AppName                  = {#ExtensionName}
UninstallDisplayName     = {#ExtensionName} Local
AppVersion               = {#ExtensionVersion}

AppPublisher             = {#ExtensionPublisher}
AppPublisherURL          = {#ExtensionPublisherURL}
AppSupportURL            = {#ExtensionSupportURL}
AppUpdatesURL            = {#ExtensionUpdatesURL}

DisableWelcomePage       = no

DefaultDirName           = {userpf}\RH Utensils\Extensions\{#ExtensionName}
DisableDirPage           = true

DisableProgramGroupPage  = yes

PrivilegesRequired       = lowest
OutputBaseFilename       = {#ExtensionName} Setup - Local
WizardStyle              = modern
SetupIconFile            = Example Extension Logo.ico
MinVersion               = 6.1

#include <idp.iss>

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "Desktop-Symbol erstellen"; GroupDescription: "{cm:AdditionalIcons}"
Name: "programsicon"; Description: "Startmenü-Symbol erstellen"; GroupDescription: "{cm:AdditionalIcons}"

[Dirs]
Name: "{app}"
Name: "{app}\{#ExtensionVersion}"

[Icons]
IconFilename: "{app}\{#ExtensionVersion}\Icons\logo.ico"; Name: "{userprograms}\{#ExtensionPublisher} (für {username})\{#ExtensionName}"; Filename: "{userpf}\RH Utensils\Main\RH Utensils.exe"; Parameters: "-""{#ExtensionName}"""; Tasks: programsicon
IconFilename: "{app}\{#ExtensionVersion}\Icons\logo.ico"; Name: "{userdesktop}\{#ExtensionName}"; Filename: "{userpf}\RH Utensils\Main\RH Utensils.exe"; Parameters: "-""{#ExtensionName}"""; Tasks: desktopicon 

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{tmp}\unzip.exe"; Parameters: "x ""{tmp}\{#ExtensionName}.zip"" -y -o""{app}\{#ExtensionVersion}"""; Flags: runhidden; StatusMsg: "Entpacke Programm Dateien ..."
Filename: "{tmp}\MainSetup.exe"; Parameters: "/VERYSILENT"; Flags: skipifdoesntexist; StatusMsg: "Installiere RH Utensils Main ..."
Filename: "{userpf}\RH Utensils\Main\RH Utensils.exe"; Parameters: "-""{#ExtensionName}"""; Flags: nowait postinstall skipifsilent; Description: "{#ExtensionName} starten"

[Code]
procedure InitializeWizard();
begin
  if not RegKeyExists(HKCU, 'Software\Microsoft\Windows\CurrentVersion\Uninstall\RH Utensils_Main_Local_is1') then
  begin
    idpAddFile('https://raw.githubusercontent.com/rh-utensils/main/master/Setup Files/Output/Main Setup - Local.exe', ExpandConstant('{tmp}\MainSetup.exe'));  
  end;

  idpAddFile('https://raw.githubusercontent.com/rh-utensils/main/master/Setup Files/7za.exe', ExpandConstant('{tmp}\unzip.exe'));
  idpAddFile('{#ZipFileLink}', ExpandConstant('{tmp}\{#ExtensionName}.zip'));

  idpDownloadAfter(wpReady);
end;