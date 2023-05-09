
cd /d %~dp0

rd /s /Q Packages
mklink /J Packages "%~dp0../EngineFrameWork/Packages"

	
cd "Assets"
rd /s /Q Launch
mklink /J Launch "%~dp0../EngineFrameWork/Assets/Launch"

cd "Assets"
rd /s /Q Plugins
mklink /J Plugins "%~dp0../EngineFrameWork/Assets/Plugins"

cd "Assets"
rd /s /Q Scenes
mklink /J Scenes "%~dp0../EngineFrameWork/Assets/Scenes"

cd "Scripts/Engine/LockStep"


rd /s /Q Common
mklink /J Common "%~dp0../EngineFrameWork/Assets/Scripts/Engine/LockStep/Common"

rd /s /Q Debug
mklink /J Debug "%~dp0../EngineFrameWork/Assets/Scripts/Engine/LockStep/Debug"



REM cd "Assets"
REM rd /s /Q Debug
REM mklink /J Debug "%~dp0../EngineFrameWork/Assets/Scripts/Engine/LockStep/Debug/"


REM cd ".."
REM cd "ProjectSettings"

REM del /f /Q TagManager.asset
REM mklink TagManager.asset "%~dp0../EngineFrameWork/ProjectSettings/TagManager.asset"

REM del /f /Q NavMeshAreas.asset
REM mklink NavMeshAreas.asset "%~dp0../EngineFrameWork/ProjectSettings/NavMeshAreas.asset"

REM del /f /Q DynamicsManager.asset
REM mklink DynamicsManager.asset "%~dp0../EngineFrameWork/ProjectSettings/DynamicsManager.asset"

REM del /f /Q EditorBuildSettings.asset
REM mklink EditorBuildSettings.asset "%~dp0../EngineFrameWork/ProjectSettings/EditorBuildSettings.asset"

REM rd /s /Q ProjectSettings
REM mklink /J ProjectSettings "%~dp0../EngineFrameWork/ProjectSettings"

echo "链接完成，按任意键退出...."

pause