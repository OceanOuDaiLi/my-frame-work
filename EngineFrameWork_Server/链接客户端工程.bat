
cd /d %~dp0

rd /s /Q Packages
mklink /J Packages "%~dp0../EngineFrameWork/Packages"

	
rd /s /Q ProjectSettings
mklink /J ProjectSettings "%~dp0../EngineFrameWork/ProjectSettings"


cd "Assets"
rd /s /Q Launch
mklink /J Launch "%~dp0../EngineFrameWork/Assets/Launch"


cd "Assets"
rd /s /Q Scenes
mklink /J Scenes "%~dp0../EngineFrameWork/Assets/Scenes"


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

echo "链接完成，按任意键退出...."

pause