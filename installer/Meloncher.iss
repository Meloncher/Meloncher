#define public Dependency_NoExampleSetup
#include "src\CodeDependencies.iss"

#define MyAppName "Meloncher"
#define MyAppVersion "1.0"
#define MyAppPublisher "MelonHell"          
#define MyAppCopyright "Copyright © MelonHell"
#define MyAppURL "https://github.com/MelonHell/Meloncher/"
#define MyAppExeName "Meloncher.exe"

[Setup]
AppId={{A7C89DFE-4387-4C63-B005-A10E0D5CC7C4}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=commandline
OutputBaseFilename=MeloncherSetup
SetupIconFile=SetupIcon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
WizardImageFile="WizardImage.bmp"
;WizardSmallImageFile="WizardSmallImage.bmp"
SourceDir="src"
OutputDir={#SourcePath}\out

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "X:\Projects\C#\Meloncher\src\MeloncherAvalonia\bin\Release\net5.0\win-x86\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "X:\Projects\C#\Meloncher\src\MeloncherAvalonia\bin\Release\net5.0\win-x86\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "WizardHeaderImage.bmp"; Flags: dontcopy

Source: "netcorecheck\netcorecheck.exe"; Flags: dontcopy noencryption
Source: "netcorecheck\netcorecheck_x64.exe"; Flags: dontcopy noencryption

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
procedure InitializeWizard;
var
  BitmapImage: TBitmapImage;
begin
  Dependency_InitializeWizard;


  ExtractTemporaryFile('WizardHeaderImage.bmp');
  BitmapImage := TBitmapImage.Create(WizardForm);
  BitmapImage.Parent := WizardForm.MainPanel;
  BitmapImage.Width := WizardForm.MainPanel.Width;
  BitmapImage.Height := WizardForm.MainPanel.Height;
  { Needed for WizardStyle=modern in Inno Setup 6. Must be removed in Inno Setup 5. }
  BitmapImage.Anchors := [akLeft, akTop, akRight, akBottom];
  BitmapImage.Stretch := True;
  BitmapImage.AutoSize := False;
  BitmapImage.Bitmap.LoadFromFile(ExpandConstant('{tmp}\WizardHeaderImage.bmp'));
  
  WizardForm.WizardSmallBitmapImage.Visible := False;
  WizardForm.PageDescriptionLabel.Visible := False;
  WizardForm.PageNameLabel.Visible := False;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  Result := Dependency_PrepareToInstall(NeedsRestart);
end;

function NeedRestart: Boolean;
begin
  Result := Dependency_NeedRestart;
end;

function UpdateReadyMemo(const Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
begin
  Result := Dependency_UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo);
end;

function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet50;

  Result := True;
end;
