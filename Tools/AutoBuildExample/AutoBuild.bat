@echo off&setlocal EnableDelayedExpansion

goto notice
===============================================================================================================================================
:: 重要
:: 1.每次构建需进行版本号设置和版本类型确认
:: 2.首次构建需要进行工程路径设置(不能有空格)
:: 需知
:: 3.最后一步构建apk需要在环境变量里配置 JAVA_HOME  如 D:\Android Studio\jre\
:: 4.Unity里需要在 Edit/Preferences/External Tools/Android 下将JDK SDK NDK 勾选框去掉，把地址手动填进去，否则构建可能会失败（应该是Unity bug）
:: 5.要使用svn更新的话，需要先安装 svn command line client tools

[示例配置]
set UnityEditorPath=C:\Program Files\Unity\Hub\Editor\2019.4.23f1c1\Editor
set UnityProjectDir=E:\WorkSpace\Unity\GIT\Build_Arwen
set AndroidProjectDir=E:\WorkSpace\Android\ArwenAndroid

===============================================================================================================================================
:notice

REM ========= NOTICE! 版本号设置 ================
REM ---------------------------------------------
set Version=190001
REM set VerType=-debug
set VerType=-release
REM ---------------------------------------------
REM ---------------------------------------------
REM ---------------------------------------------
REM ********* NOTICE! 工程路径设置 **************
set UnityEditorPath=C:\Program Files\Unity\Hub\Editor\2019.4.23f1c1\Editor
set UnityProjectDir=E:\WorkSpace\Unity\GIT\Build_Arwen
set AndroidProjectDir=E:\WorkSpace\Android\ArwenAndroid
REM ---------------------------------------------
REM =============================================



REM ======== 版本Code和BanbenName设置 ========
set verCode=versionCode %Version%
set verName=versionName '%Version:~0,1%.%Version:~1,1%.%Version:~2,1%.%Version:~3,3%'

echo ==== %VerType%
echo ==== %Version%
echo ==== %verCode%
echo ==== %verName%

REM =========== 文件路径设置 ==========
set PATH=%PATH%;C:\Windows\SysWOW64;%UnityEditorPath%
set UnityOutPut=%UnityProjectDir%\Output
set ApkOutPut=%AndroidProjectDir%\launcher\build\outputs\apk\release

REM goto build
REM goto export
REM goto end

REM android工程更新
echo ======== S.1 Git Update =============== %date% %time%
git -C %AndroidProjectDir% reset --hard
git -C %AndroidProjectDir% clean -dfx unityLibrary
git -C %AndroidProjectDir% checkout unityLibrary\.
git -C %AndroidProjectDir% pull

REM goto end

REM Unity输出目录处理（清空&新建）
echo ======== S.2 Clear Output ============= %date% %time%
rd /s /Q %UnityOutPut%
md %UnityOutPut%

REM ================================ 版本类型 ============================================================
echo ======== S.3 PrepareUnity ============= %date% %time%
Unity.exe -batchmode -projectPath %UnityProjectDir% -executeMethod AutoBuildTool.Prepare -quit %VerType%
rem -debug 测试版本 不填则默认release版本
rem -i18n_en 英文版本 不填则默认中文版本
REM ======================================================================================================

REM 执行一键更新表格数据
echo ======== S.4 Excel to Json ============ %date% %time%
Unity.exe -batchmode -projectPath %UnityProjectDir% -executeMethod SpreadsheetTool.Convert2Json -quit

echo ======== S.5 Excel Script ============= %date% %time%
Unity.exe -batchmode -projectPath %UnityProjectDir% -executeMethod SpreadsheetTool.CreateScript -quit


REM 执行AssetBundle构建
echo ======== S.6 Build AssetBundle ======== %date% %time%
Unity.exe -batchmode -projectPath %UnityProjectDir% -executeMethod MenuTool.BuildAssetBundle -quit


REM 执行Export
echo ======== S.7 Export Project =========== %date% %time%
Unity.exe -batchmode -projectPath %UnityProjectDir% -executeMethod AutoBuildTool.Export -quit -output %UnityOutPut%

:export
REM goto find

REM 合并工程文件 Libs
echo ======== S.8 Merge & Update =============== %date% %time%
rd /s /q %AndroidProjectDir%\unityLibrary\libs\
xcopy %UnityOutPut%\unityLibrary\libs\* %AndroidProjectDir%\unityLibrary\libs\ /s/e/y

REM 合并工程文件 Src
REM echo ======== S.9 Merge src ================  %date% %time%
rd /s /q %AndroidProjectDir%\unityLibrary\src\
xcopy %UnityOutPut%\unityLibrary\src\* %AndroidProjectDir%\unityLibrary\src\ /s/e/y

REM goto build
:find

REM goto update

REM 源noCompress查找
for /f "delims=" %%a in ('findstr .* %UnityProjectDir%\Output\launcher\build.gradle') do (
 
  echo output====%%a
 
  echo %%a|findstr noCompress >nul &&(
 
  set "opt=%%a"
 
  echo find=====)
)

:update

echo ==== start update gradle===

REM goto build

for /f "delims=" %%b in ('findstr .* %AndroidProjectDir%\launcher\build.gradle') do (

 set "str=%%b"

 rem update versionCode
 echo %%b|findstr versionCode >nul && (
 set "str=%verCode%"
 )

 rem update versionName
 echo %%b|findstr versionName >nul &&(
  echo %%b|findstr defaultConfig >nul ||(
  set "str=%verName%"
  )
 )

 rem update apptOptions
 echo %%b|findstr noCompress >nul &&(
 set "str=!opt!"
 )

echo new===!str!

echo !str! >>%AndroidProjectDir%\launcher\tmp.txt
)

echo ===rename===

del /q /a /f .* %AndroidProjectDir%\launcher\build.gradle

ren %AndroidProjectDir%\launcher\tmp.txt build.gradle

:build

goto skip

::符号表暂不上传，需手动备份.

REM 上传符号表
rem 上传符号表至后台 TODO： 正式版记得更换版本号
echo ======== Step.9 Start UploadSymble ========  %date% %time%


for %%i in (Output\symbols) do (
    java -jar Tools\Bugly\buglyqq-upload-symbol.jar -appid 5c7cba49e0 -appkey d371b585-df69-49c8-af53-89b520502ca2 -bundleid com.jmgo.arwen -version %Version% -inputSymbol  %%i -platform Android 
)
if exist buglybin (
    rd /q/s buglybin
    del cp_buglyQqUploadSymbolLib.jar
    del cp_buglySymbolAndroid.jar
) 

:skip


REM =================储存apk相关数据===============================
REM 记录文件夹名
set fileName=Arwen_%Version%_%date:~5,2%%date:~8,2%_%time:~0,2%%time:~3,2%%VerType%
echo ======== Step.10 Save Publish Files ======== %fileName%
cd %UnityProjectDir%
REM 1.创建对应版本存储文件夹
md %UnityProjectDir%\ApkCache\%fileName%
REM 2.复制存储打包依赖
xcopy %UnityOutPut%\symbols\* ApkCache\%fileName%\symbols\ /s/e/y

REM 3.复制存储符号表
xcopy %UnityProjectDir%\BuildCache\* ApkCache\%fileName%\BuildCache\ /s/e/y


REM goto end

REM 构建APK
echo ======== Step.11 Start Build Apk ======== %date% %time%
cd %AndroidProjectDir%

.\gradlew assembleRelease
REM .\gradlew installRelease

:end


echo ======= Finished !! ========== %date% %time%

PAUSE
exit 0