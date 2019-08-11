#define ExtensionName           "ExampleExtension"
#define ExtensionVersion        "0.2.5"
#define ExtensionPublisher      "RH Utensils"
#define ExtensionPublisherURL   "https://rh-utensils.hampoelz.net/"
#define ExtensionSupportURL     "https://github.com/rh-utensils/main/issues/"
#define ExtensionUpdatesURL     "https://github.com/rh-utensils/main/releases/"
#define ZipFileLink             "https://github.com/rh-utensils/main/releases/download/v" + ExtensionVersion + "/ExampleExtension.zip"

[Setup]
AppId                    = {#ExtensionPublisher}_{#ExtensionName}
AppName                  = {#ExtensionName}
UninstallDisplayName     = {#ExtensionName}
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
OutputBaseFilename       = {#ExtensionName} Setup
WizardStyle              = modern
SetupIconFile            = {#ExtensionName} Logo.ico
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
Name: "{app}"
Name: "{app}\Logs"
Name: "{app}\{#ExtensionVersion}"
Name: "{userpf}\RH Utensils\SetUserFTA"

[Icons]
IconFilename: "{app}\{#ExtensionVersion}\Icons\logo.ico"; Name: "{userprograms}\{#ExtensionPublisher}\{#ExtensionName}"; Filename: "{userpf}\RH Utensils\Main\RH Utensils.exe"; Parameters: "-""{#ExtensionName}"""; Tasks: programsicon
IconFilename: "{app}\{#ExtensionVersion}\Icons\logo.ico"; Name: "{userdesktop}\{#ExtensionName}"; Filename: "{userpf}\RH Utensils\Main\RH Utensils.exe"; Parameters: "-""{#ExtensionName}"""; Tasks: desktopicon 

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{tmp}\unzip.exe";                                           Parameters: "x ""{tmp}\{#ExtensionName}.zip"" -y -o""{app}\{#ExtensionVersion}"""; Flags: runhidden;                       StatusMsg: "Entpacke Programm Dateien ..."
Filename: "{tmp}\unzip.exe";                                           Parameters: "x ""{tmp}\SetUserFTA.zip"" -y -o""{userpf}\RH Utensils""";            Flags: runhidden;                       StatusMsg: "Entpacke UserFTA ..."
Filename: "{tmp}\MainSetup.exe";                                       Parameters: "/VERYSILENT";                                                         Flags: skipifdoesntexist;               StatusMsg: "Installiere RH Utensils Main ..."
Filename: "{userpf}\RH Utensils\SetUserFTA\SetUserFTA.exe";            Parameters: ".mp3 {#ExtensionName}";                                               Flags: waituntilidle;                   Tasks: fileaccessory
Filename: "{userpf}\RH Utensils\SetUserFTA\SetUserFTA.exe";            Parameters: ".wav {#ExtensionName}";                                               Flags: waituntilidle;                   Tasks: fileaccessory
Filename: "{userpf}\RH Utensils\Main\RH Utensils.exe";                 Parameters: "-""{#ExtensionName}""";                                               Flags: nowait postinstall skipifsilent; Description: "{#ExtensionName} Starten";

[Registry]
Root: "HKCU"; Subkey: "Software\Classes\.txt";                                 ValueType: string; ValueData: "{#ExtensionName}";                             Flags: uninsdeletevalue;       Tasks: fileaccessory
Root: "HKCU"; Subkey: "Software\Classes\.test";                                ValueType: string; ValueData: "{#ExtensionName}";                             Flags: uninsdeletevalue;       Tasks: fileaccessory
Root: "HKCU"; Subkey: "Software\Classes\{#ExtensionName}";                     ValueType: string; ValueData: "Musik-Datei";                                  Flags: uninsdeletekey;         Tasks: fileaccessory
Root: "HKCU"; Subkey: "Software\Classes\{#ExtensionName}\DefaultIcon";         ValueType: string; ValueData: "{app}\{#ExtensionVersion}\Icons\file.ico";                                    Tasks: fileaccessory
Root: "HKCU"; Subkey: "Software\Classes\{#ExtensionName}\shell\open\command";  ValueType: string; ValueData: """{userpf}\RH Utensils\Main\RH Utensils.exe"" -""{#ExtensionName}"" ""%1""";  Tasks: fileaccessory

[Code]
procedure InitializeWizard();
begin
  if not RegKeyExists(HKCU, 'Software\Microsoft\Windows\CurrentVersion\Uninstall\RH Utensils_Main_is1') then
  begin
    idpAddFile('https://raw.githubusercontent.com/rh-utensils/main/master/Setup Files/Output/Main Setup.exe', ExpandConstant('{tmp}\MainSetup.exe'));  
  end;

  idpAddFile('https://raw.githubusercontent.com/rh-utensils/main/master/Setup Files/7za.exe', ExpandConstant('{tmp}\unzip.exe'));
  idpAddFile('http://kolbi.cz/SetUserFTA.zip', ExpandConstant('{tmp}\SetUserFTA.zip'));
  idpAddFile('{#ZipFileLink}', ExpandConstant('{tmp}\{#ExtensionName}.zip'));

  idpDownloadAfter(wpReady);
end;