#### Daily Schedule ####
1.上午 LeetCode / Solution   
2.午休 Games 10x <=> 20x系列
3.下午 框架开发，工具开发。业务开发跟进。

Res :
1.资源。			 										
2.启动流程重构															
3.多线程下载
4.资源管理器重构。


#### Schedule #### 2023.3.10
1.Terasurware 插件优化														 [自动导入ScriptableObject有问题。暂时迁移到备份文件夹]
2.MiNiZip 接入																 [Delete. 使用7Zip/LZMA 压缩 & 解压缩]
3.启动流程重构
4.多线程下载。
4.资源管理器重构
	-- 稳定的引用计数工具。
	-- 基于多线程的资源包加载。
6.HCLR - DHE接入、热重载接入												

#### Schedule #### 2023.3.13
1.Down -> [jenkins build] 首包 删除非跟包文件及文件夹
2.Down -> ETTask Example 初版ETTask使用。
3.热更资源
	-- Down -> 增量资源记录配置文件生成。
	-- Down -> 客户端，回退任意热更版本支持。
	
	【优先】
	-- ToDo 热更版本相差1时，下载zip解压即可。(Zip策略：筛选出和上个版本的差异文件并压缩。)
	-- ToDo 热更跨多个版本时，单个文件对比下载(CDN 最新版本缓存完整Total_Pkg,通过update-list文件进行完整哈希校验，并下载)。	
	-- ToDo cdn生成下载的配置校验文件
	
	-- ToDo 可配置的每个Zip包的大小。(通用)
	

#### Schedule #### 2023.3.14
热更资源
	【优先】

	-- ToDo 热更版本相差1时，下载zip解压即可。						(Zip策略：筛选出和上个版本的差异文件并压缩。)
	-- ToDo 热更跨多个版本时，单个文件对比下载。					(CDN 最新版本缓存完整Total_Pkg,通过update-list文件进行完整哈希校验，并下载)。	

	-- Down -> PrepearedInitBuildInformation 迁移到 SetupStrategy.	
	-- ToDo cdn生成下载的配置校验文件
	
	 
#### Schedule #### 2023.3.15
热更资源
	【优先】

	-- Down -> 热更版本相差1时，下载zip解压即可。					(Zip策略：筛选出和上个版本的差异文件并压缩。)
	-- Down -> 热更跨多个版本时，单个文件对比下载。					(CDN 最新版本缓存完整Total_Pkg,通过update-list文件进行完整哈希校验，并下载)。	
	-- Down -> PrepearedInitBuildInformation 迁移到 SetupStrategy.	
	-- Down -> cdn生成下载的配置校验文件
	
	
	
#### 启动流程重构 #### 2023.3.16
1.首包启动
2.热更包启动

本地不存在 version.ini
	-- Server VersionCode=1.0.0.0
	 -- ZipFile=Empty
	  -- 首包使用整包。使用StreamingAsset加载。资源加载器做处理(一般是给IOS用)

本地不存在 version.ini	  
	-- Server VersionCode>1.0.0.0
	 -- ZipFile!=Empty

#### 启动流程重构 #### 2023.3.17
1.首包启动
2.热更包启动
3.Launch添加启动UI预制，打包到一个AssetBundle。并设置为跟包

 *  流程简述
 * 
 * 
 *  1. 资源解压检测
 *      1.1.解压检测不通过。执行解压
 * 
 *  2.  资源版本号检测
 *       2.1.版本号检测不通过.
 *        2.1.1.资源下载.
 *         2.1.2 顺延版本下载或跨版本加载检测.
 *          2.1.3 顺延版本下载，直接下载Zip，并解压到PersistentDataPath.
 *           2.1.4 跨版本下载，按文件下载到PersistentDataPath.
 *           
 *       2.2 CLR启动
 *      
 *  3.  切换到登录或开启场景     
 

#### 资源异步多线程加载 #### 2023.3.18
1.Resources.cs
2.AssetBundleLoader.cs

#### 2023.3.19-2023.3.25 #### 
1.资源加载支持多线程。
2.TMP引入并整理。
3.Todo: 
   -- 【AssetBundle工具收尾】	自动分zip功能。version.ini添加多zip支持。
   -- 【启动流程收尾】 			zip多线程下载。zip多线程解压。

4.初步了解KCP。技术预研 & 接入。


#### 初识KCP ####  2023.3.26
#### 渲染框架重整 ####  2023.3.26
"com.unity.render-pipelines.universal": "10.8.1",
#### 渲染框架重整 ####  2023.3.28
  -- Shader FrameWork -> Shader GUI 10%
  
  
#### Add ToDoList #### 2023.4.23
#### 添加整包StreamingAsset加载支持 ####
	 -- 打包工具添加一键整包支持。
	 -- 资源加载脚本添加StreamingAsset加载策略：
		-- Way1: 判断持久化路径是否有加载资源。
				 若有，则从持久化路径加载。
				 若无，则从StreamingAsset路径加载。
		-- Way2: 待定。
		
#### 添加AdvanFPS工具 & 重构体型工具 & 接入体型测试资源 ####
#### 启动流程重构 -> Launch 应该包含热更资源下载 ####