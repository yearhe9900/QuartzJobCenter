# 基于 .NET 5和 .QuartzNet 3.x 的可视化页面
## 前言
以前在华住的时候，就遇到一个很烦人的问题，很多定时任务，用的是一个很老的winform程序，进行可视化管理，虽说效果不错，但面对一百多个调度任务的时候，维护起来，也是力不从心。于是就有了使用QuartzNet做一个可视化的管理页面的想法。去年一年太忙，通勤来回4个多小时，没太多时间做，最近换了工作，有了充足时间，刚好 .NET 5发布，于是便用 .NET 5实现，顺便练练手。初期没太多头绪，于是参考了[https://github.com/zhaopeiym/quartzui](https://github.com/zhaopeiym/quartzui)它的方法。
## 内容
- 基于 .NET 5
- 基于 .QuartzNet 3.x 的可视化页面
- 使用Layui
- 内置数据库持久化
- 使用Dapper作为ORM
- 支持API和GRPC定时调度
- 执行器与语言无关
- docker支持
## 未来要实现
- 邮件通知
- 常驻任务（如RabbitMQ客户端，支持手动开启关闭）
## 效果图
![1609811469.jpg](https://upload-images.jianshu.io/upload_images/17755401-c9877e1dadf60935.jpg?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![1609811519(1).jpg](https://upload-images.jianshu.io/upload_images/17755401-36f50b620258f6c9.jpg?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![1609811541(1).jpg](https://upload-images.jianshu.io/upload_images/17755401-490b97f11a874cc9.jpg?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

## 源码地址
[https://github.com/yearhe9900/QuartzJobCenter](https://github.com/yearhe9900/QuartzJobCenter)
## 注意
目前代码比较粗糙，且很多功能未实现，仅供学习参考，如用于生产环境，请谨慎选择。