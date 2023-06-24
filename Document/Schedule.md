#### Schedule Pre-Research #### 2023.March.
1.ET FrameWork Research.
2.GPU/CPU hardware Infomation Research.
3.MyFrameWork Develop.
4.Base Shadeing Research.
5.GamePlay research.


Res :
【功能框架支持】
	1.资源构建
		-- 打包工具可视化支持。
			-- 可视化预览 AB 依赖。
			-- 可视化构建。
				-- 可配置的渠道信息，构建信息。
				-- 可选的加密类型，压缩类型。
				-- 首包可视化拆包功能。
				-- 可视化分包支持。				(Todo.主要应用于大型场景有动态加卸载需求时.)
		-- 打包工具功能支持。
			-- 打包管线开发。
			-- 资源三种压缩方式对比，及应用。
		-- Jenkins支持。
			-- Aot打包
			-- 跨平台首包。
			-- 跨平台热更包。
		-- 热更构建。
			-- 支持版本回退下载。
			-- 支持跨多版本，基于对比文件下载。
			-- 支持跨一个版本，基于zip包文件下载。
			
		-- 收尾
			-- Todo: 可视化分包支持。用于大场景有动态加卸载资源包时。
			-- Todo: 7zip-》mobile端 LZ4/LZMA 解压缩验证

	2.启动流程重构

	3.修改将资源异步协程加载修改为多线程加载
			
	4.基于KCP的网络框架构建。

 【渲染支持】