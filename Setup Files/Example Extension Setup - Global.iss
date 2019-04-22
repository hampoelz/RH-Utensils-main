#define ExtensionName           "ExampleExtension"
#define ExtensionVersion        "0.1"
#define ExtensionPublisher      "RH Utensils"
#define ExtensionPublisherURL   "https://rh-utensils.hampoelz.net/"
#define ExtensionSupportURL     "https://github.com/rh-utensils/main/issues/"
#define ExtensionUpdatesURL     "https://github.com/rh-utensils/main/releases/"
#define ZipFileLink             "https://github.com/rh-utensils/main/releases/download/" + ExtensionVersion + "/ExampleExtension.zip"

[Setup]
AppId                    = {#ExtensionPublisher}_{#ExtensionName}_Global
AppName                  = {#ExtensionName}
UninstallDisplayName     = {#ExtensionName} Global
AppVersion               = {#ExtensionVersion}

AppPublisher             = {#ExtensionPublisher}
AppPublisherURL          = {#ExtensionPublisherURL}
AppSupportURL            = {#ExtensionSupportURL}
AppUpdatesURL            = {#ExtensionUpdatesURL}

DisableWelcomePage       = no

DefaultDirName           = {pf}\RH Utensils\Extensions\{#ExtensionName}
DisableDirPage           = true

DisableProgramGroupPage  = yes

PrivilegesRequired       = admin
OutputBaseFilename       = {#ExtensionName} Setup - Global
WizardStyle              = modern
SetupIconFile            = Example Extension Logo.ico
MinVersion               = 6.1
ChangesAssociations      = yes

#include <idp.iss>

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "Desktop-Symbol erstellen"; GroupDescription: "{cm:AdditionalIcons}"
Name: "programsicon"; Description: "Startmenü-Symbol erstellen"; GroupDescription: "{cm:AdditionalIcons}"
Name: "fileaccessory"; Description: "Als Standartprogramm für unterstützte Dateitypen registrieren"; GroupDescription: "Andere"

[Dirs]
Name: "{app}"; Permissions: everyone-full
Name: "{app}\{#ExtensionVersion}"; Permissions: everyone-full

[Icons]
IconFilename: "{app}\{#ExtensionVersion}\Icons\logo.ico"; Name: "{commonprograms}\{#ExtensionPublisher}\{#ExtensionName}"; Filename: "{pf}\RH Utensils\Main\RH Utensils.exe"; Parameters: "-""{#ExtensionName}"""; Tasks: programsicon
IconFilename: "{app}\{#ExtensionVersion}\Icons\logo.ico"; Name: "{commondesktop}\{#ExtensionName}"; Filename: "{pf}\RH Utensils\Main\RH Utensils.exe"; Parameters: "-""{#ExtensionName}"""; Tasks: desktopicon

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{tmp}\unzip.exe"; Parameters: "x ""{tmp}\{#ExtensionName}.zip"" -y -o""{app}\{#ExtensionVersion}"""; Flags: runhidden; StatusMsg: "Entpacke Programm Dateien ..."
Filename: "{tmp}\MainSetup.exe"; Parameters: "/VERYSILENT"; Flags: skipifdoesntexist; StatusMsg: "Installiere RH Utensils Main ..."
Filename: "{pf}\RH Utensils\Main\RH Utensils.exe"; Parameters: "-""{#ExtensionName}"""; Flags: nowait postinstall skipifsilent; Description: "{#ExtensionName} starten"

[Registry]
Root: "HKCR"; Subkey: ".mp3";                                 ValueType: string; ValueData: "{#ExtensionName}";                             Flags: uninsdeletevalue;      Tasks: fileaccessory
Root: "HKCR"; Subkey: ".wav";                                 ValueType: string; ValueData: "{#ExtensionName}";                             Flags: uninsdeletevalue;      Tasks: fileaccessory
Root: "HKCR"; Subkey: ".mp4";                                 ValueType: string; ValueData: "{#ExtensionName}";                             Flags: uninsdeletevalue;      Tasks: fileaccessory
Root: "HKCR"; Subkey: ".mov";                                 ValueType: string; ValueData: "{#ExtensionName}";                             Flags: uninsdeletevalue;      Tasks: fileaccessory
Root: "HKCR"; Subkey: "{#ExtensionName}";                     ValueType: string; ValueData: "Program {#ExtensionName}";                     Flags: uninsdeletekey;        Tasks: fileaccessory
Root: "HKCR"; Subkey: "{#ExtensionName}\DefaultIcon";         ValueType: string; ValueData: "{app}\{#ExtensionVersion}\Icons\file.ico";   Tasks: fileaccessory
Root: "HKCR"; Subkey: "{#ExtensionName}\shell\open\command";  ValueType: string; ValueData: """{pf}\RH Utensils\Main\RH Utensils.exe"" -""{#ExtensionName}"" ""%1""";     Tasks: fileaccessory

[Code]
procedure InitializeWizard();
begin
  if not RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\RH Utensils_Main_Global_is1') then
  begin
    idpAddFile('https://raw.githubusercontent.com/rh-utensils/main/master/Setup Files/Output/Main Setup - Global.exe', ExpandConstant('{tmp}\MainSetup.exe'));  
  end;

  idpAddFile('https://raw.githubusercontent.com/rh-utensils/main/master/Setup Files/7za.exe', ExpandConstant('{tmp}\unzip.exe'));
  idpAddFile('{#ZipFileLink}', ExpandConstant('{tmp}\{#ExtensionName}.zip'));

  idpDownloadAfter(wpReady);
end;