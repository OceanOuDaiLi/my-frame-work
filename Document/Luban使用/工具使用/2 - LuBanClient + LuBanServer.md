Luban.Client与Luban.Server独立部属，云生成模式


#一#: 部属 luban-server

Way1:
[
	基于 docker (强烈推荐以这种方式在服务器上部属luban-server，因为可以充分利用缓存机制大幅缩短生成时间)

	docker run -d --rm --name luban-server -p 8899:8899 focuscreativegames/luban-server:latest
]

Way2:
[
	基于 .net 6 runtime （可跨平台，不需要重新编译）

	自行安装 .net 6 runtime.
	从示例项目拷贝整个Luban.ClientServer目录（可跨平台，即使在linux、mac平台也不需要重新编译）
	在Luban.ClientServer目录下运行 dotnet Luban.Server.dll (提示：Win平台可以直接运行 Luban.Server.exe)
]


#二# 安装luban-client
Way1:
[
	基于 docker (只推荐与jenkins之类devops工具配合使用，因为docker容器启动会增加一定的延迟)

	docker run --rm -v $PWD/.cache.meta:/bin/.cache.meta focuscreativegames/luban-client <参数>

	提醒！ .cache.meta这个文件用于保存本地生成或者提交到远程的文件md5缓存，强烈推荐 添加-v $PWD/.cache.meta:/bin/.cache.meta 映射，不然每次重新计算所有涉及文件的md5,这可能在项目后期会造成多达几秒的延迟。

]

Way2:
[
	基于 .net 6 runtime （推荐win平台使用，可跨平台，不需要重新编译）

	自行安装 .net 6 runtime.
	从示例项目拷贝 Luban.Client（可跨平台，即使在linux、mac平台也不需要重新编译）
]