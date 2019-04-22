#define ProgrammVersion  "0.1"
#define ZipFileLink      "https://github.com/rh-utensils/main/releases/download/v" + ProgrammVersion + "/Main.zip"

[Setup]
AppId                    = RH Utensils_Main_Global
AppName                  = RH Utensils Main
UninstallDisplayName     = RH Utensils Main Global
AppVersion               = {#ProgrammVersion}

AppPublisher             = RH Utensils
AppPublisherURL          = https://rh-utensils.hampoelz.net/
AppSupportURL            = https://github.com/rh-utensils/main/issues/
AppUpdatesURL            = https://github.com/rh-utensils/main/releases/

DisableWelcomePage       = no

DefaultDirName           = {pf}\RH Utensils\Main
DisableDirPage           = true

DisableProgramGroupPage  = yes

PrivilegesRequired       = admin
OutputBaseFilename       = Main Setup - Global
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
Name: "{pf}\RH Utensils"; Permissions: everyone-full
Name: "{pf}\RH Utensils\Main"; Permissions: everyone-full

[Icons]
Name: "{commonprograms}\RH Utensils\Main"; Filename: "{app}\RH Utensils.exe"; Tasks: programsicon
Name: "{commondesktop}\RH Utensils Main"; Filename: "{app}\RH Utensils.exe"; Tasks: desktopicon

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{tmp}\unzip.exe"; Parameters: "x ""{tmp}\Main.zip"" -y -o""{app}"""; Flags: runhidden; StatusMsg: "Entpacke Programm Dateien ..."
Filename: "{app}\RH Utensils.exe"; Flags: nowait postinstall skipifsilent unchecked; Description: "RH Utensils Main starten"

[CustomMessages]
InstallingDotNetFramework=.NET Framework 4.7.2 wird installiert. Dies könnte ein paar Minuten dauern...
DotNetFrameworkFailedToLaunch=Fehler beim Starten des .NET Framework-Installationsprogramms mit Fehler "%1". Bitte behebe den Fehler und führen Sie das Installationsprogramm erneut aus.
DotNetFrameworkFailed1602=Die .NET Framework-Installation wurde abgebrochen. Diese Installation kann fortgesetzt werden. Beachte jedoch, dass diese Anwendung möglicherweise nicht ausgeführt wird, wenn die .NET Framework-Installation erfolgreich abgeschlossen wurde.
DotNetFrameworkFailed1603=Bei der Installation von .NET Framework ist ein schwerwiegender Fehler aufgetreten. Bitte behebe den Fehler und führe das Installationsprogramm erneut aus.
DotNetFrameworkFailed5100=Dein Computer erfüllt nicht die Anforderungen von .NET Framework. Bitte konsultieren Sie die Dokumentation.
DotNetFrameworkFailedOther=Das .NET Framework-Installationsprogramm wurde mit einem unerwarteten Statuscode "%1" beendet. Überprüfe alle anderen vom Installationsprogramm angezeigten Meldungen, um festzustellen, ob die Installation erfolgreich abgeschlossen wurde, und brechen Sie die Installation ab, und beheben Sie das Problem, falls dies nicht der Fall ist.

[Code]
var
  requiresRestart: boolean;

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
    idpAddFile('http://go.microsoft.com/fwlink/?LinkId=863262', ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
  end;

  idpAddFile('https://raw.githubusercontent.com/rh-utensils/main/master/Setup Files/7za.exe', ExpandConstant('{tmp}\unzip.exe'));
  idpAddFile('{#ZipFileLink}', ExpandConstant('{tmp}\Main.zip'));

  idpDownloadAfter(wpReady);
end;

function InstallFramework(): String;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := CustomMessage('InstallingDotNetFramework');
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    if not Exec(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'), '/passive /norestart /showrmui /showfinalerror', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      Result := FmtMessage(CustomMessage('DotNetFrameworkFailedToLaunch'), [SysErrorMessage(resultCode)]);
    end
    else
    begin
      case resultCode of
        0: begin
        end;
        1602 : begin
          MsgBox(CustomMessage('DotNetFrameworkFailed1602'), mbInformation, MB_OK);
        end;
        1603: begin
          Result := CustomMessage('DotNetFrameworkFailed1603');
        end;
        1641: begin
          requiresRestart := True;
        end;
        3010: begin
          requiresRestart := True;
        end;
        5100: begin
          Result := CustomMessage('DotNetFrameworkFailed5100');
        end;
        else begin
          MsgBox(FmtMessage(CustomMessage('DotNetFrameworkFailedOther'), [IntToStr(resultCode)]), mbError, MB_OK);
        end;
      end;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;
    
    DeleteFile(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  if NetFrameworkIsMissing() then
  begin
    Result := InstallFramework();
  end;
end;

function NeedRestart(): Boolean;
begin
  Result := requiresRestart;
end;
