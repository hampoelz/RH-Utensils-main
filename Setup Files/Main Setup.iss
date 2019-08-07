#define ProgrammVersion  "0.2"
#define ZipFileLink      "https://github.com/rh-utensils/main/releases/download/v" + ProgrammVersion + "/Main.zip"

[Setup]
AppId                    = RH Utensils_Main
AppName                  = RH Utensils Main
UninstallDisplayName     = RH Utensils Main
AppVersion               = {#ProgrammVersion}

AppPublisher             = RH Utensils
AppPublisherURL          = https://rh-utensils.hampoelz.net/
AppSupportURL            = https://github.com/rh-utensils/main/issues/
AppUpdatesURL            = https://github.com/rh-utensils/main/releases/

DisableWelcomePage       = no

DefaultDirName           = {userpf}\RH Utensils\Main
DisableDirPage           = true

DisableProgramGroupPage  = yes

PrivilegesRequired       = lowest
OutputBaseFilename       = Main Setup
WizardStyle              = modern
SetupIconFile            = Main Logo.ico
MinVersion               = 6.1

#include <idp.iss>

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "Desktop-Symbol erstellen"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "programsicon"; Description: "Startmenü-Symbol erstellen"; GroupDescription: "{cm:AdditionalIcons}"

[Dirs]
Name: "{userpf}\RH Utensils"
Name: "{app}"
Name: "{app}\Logs"

[Icons]
Name: "{userprograms}\RH Utensils\Main"; Filename: "{app}\RH Utensils.exe"; Tasks: programsicon
Name: "{userdesktop}\RH Utensils Main"; Filename: "{app}\RH Utensils.exe"; Tasks: desktopicon

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{tmp}\unzip.exe"; Parameters: "x ""{tmp}\Main.zip"" -y -o""{app}"""; Flags: runhidden; StatusMsg: "Entpacke Programm Dateien ..."
Filename: "{app}\RH Utensils.exe"; Flags: nowait postinstall skipifsilent unchecked; Description: "RH Utensils Main starten"

[Code]
function NetFrameworkIsMissing(): Boolean;
var
  bSuccess: Boolean;
  regVersion: Cardinal;
begin
  Result := True;

  bSuccess := RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', regVersion);
  if (True = bSuccess) and (regVersion >= 461808) then begin
    Result := False;
  end;
end;

procedure InitializeWizard();
begin
  if NetFrameworkIsMissing() then
  begin
    MsgBox('Das .NET Framework 4.7.2 wurde nicht auf deinem Computer gefunden, damit das Programm korrekt funktioniert musst du es manuell nach Installieren!', mbError, MB_OK);
  end;
  idpAddFile('https://raw.githubusercontent.com/rh-utensils/main/master/Setup Files/7za.exe', ExpandConstant('{tmp}\unzip.exe'));
  idpAddFile('{#ZipFileLink}', ExpandConstant('{tmp}\Main.zip'));

  idpDownloadAfter(wpReady);
end;