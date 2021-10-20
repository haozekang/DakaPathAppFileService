# DakaPathAppFileService
MinIO + ImageSharp + Swagger实现自搭对象存储和图片处理
- 整个项目使用`MinIO`做分布式对象存储系统，并使用Asp.Core封装使用接口对外开放上传、删除、下载等权限
- 项目使用`Swagger`作为接口测试工具，仅在Debug模式下可访问Swagger界面并调试接口，Release发布模式下无法访问Swagger和任何界面
- 使用`ImageSharp`做为图像处理工具，可以脱离System.Drawing，方便快捷的对图像进行简易处理
- 使用`SQLite`进行记录存储，方便接口系统迁移
- 使用`Chloe`作为数据库ORM框架
- 系统配置使用了通用的`Yaml`格式
- 使用了`Quartz`作为系统定时任务执行框架，根据配置文件可以自行修改`Cron表达式`实现定时清理缓存文件
