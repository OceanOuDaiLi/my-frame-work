@echo off
 
set UNITY_LOG_PATH=%cd%\unity_log22.txt
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2021.2.7f1c1\Editor\2021.2.7f1c1\Editor\Unity.exe"
set PROJECT_PATH="C:\Users\dell\Documents\GitHub\kjframe-work\OFrameWork\KJFrameWork"
echo lunch unity.exe ,please wait a moment...
 
%UNITY_PATH% -quit -batchmode -logFile %UNITY_LOG_PATH% -projectPath %PROJECT_PATH% -executeMechod AssetBundlesMaker.BuildApplicationUnCrypt 
echo "Build apk done"
pause