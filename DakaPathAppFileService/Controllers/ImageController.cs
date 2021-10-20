using Chloe;
using DakaPathAppFileService.Domains;
using DakaPathAppFileService.ExtendMethod;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Newtonsoft.Json;
using NLog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DakaPathAppFileService.Controllers
{
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPut, Route("upload"), Produces("application/json")]
        public async Task<object> Upload([SwaggerParameter("图片文件")] IFormFile image
            , [SwaggerParameter("Sign")] string sign
            , [SwaggerParameter("TimeStamp")] long? t)
        {
            dynamic json = new ExpandoObject();
            json.data = null;
            if (sign.IsBlank() || !t.HasValue)
            {
                json.code = 1;
                json.msg = $"参数异常";
                return json;
            }
            if (t.Value <= 0)
            {
                json.code = 1;
                json.msg = $"接口请求超时";
                return json;
            }
            var dis = Math.Abs(DateTime.Now.Subtract(t.Value.TimeStampToDateTime().Value).TotalSeconds);
            if (dis > 5)
            {
                json.code = 1;
                json.msg = $"接口请求超时";
                return json;
            }
            var _sign = $"INT.{t}.LONG".MD5Encrypt32().ToUpper();
            if (sign != _sign)
            {
                json.code = 1;
                json.msg = $"签名验证失败";
                return json;
            }
            try
            {
                MinioClient mc = new MinioClient(App.MinIOAPIEndPoint, App.MinIOAccessKey, App.MinIOSecertKey);
                var fileSaveName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = $"http{(App.MinIOUseSsl == true ? "s" : "")}://{App.MinIOEndPoint}/{App.MinIOImageBucket}/{fileSaveName}";
                await mc.PutObjectAsync(App.MinIOImageBucket, fileSaveName, image.OpenReadStream(), image.Length);
                DakaImage dakaImage = new DakaImage
                {
                    BucketName = App.MinIOImageBucket,
                    SaveName = fileSaveName,
                    FileName = image.FileName,
                    FilePath = filePath,
                    FileSize = image.Length,
                    FileExtension = Path.GetExtension(image.FileName),
                    CteateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                using (DbContext my = App.sqlite) 
                {
                    my.Insert(dakaImage);
                }
                dynamic file = new ExpandoObject();
                file.Id = dakaImage.Id;
                file.FilePath = filePath;
                json.code = 0;
                json.msg = $"上传成功";
                json.data = file;
                return json;
            }
            catch(Exception e)
            {
                logger.Error($"{e}");
                json.code = 1;
                json.msg = $"{e}";
                json.data = null;
                return json;
            }
        }

        [SwaggerOperation(
            Summary = "图片下载",
            Description = "图片下载接口，可以证据特定参数，实现图片的缩放、旋转。缩放时如果同时传了比例和高宽度参数，优先根据比例参数。",
            Tags = new[] { "图片相关接口" }
        )]
        [HttpGet, Route("download")]
        public async Task<IActionResult> Download([SwaggerParameter("图片ID")] int? id
            , [SwaggerParameter("是否预览")] bool? preview
            , [SwaggerParameter("是否缩放")] bool? resize
            , [SwaggerParameter("是否旋转")] bool? rotate
            , [SwaggerParameter("旋转方向：0(或非1值).逆时针；1.顺时针")] int? rotate_d
            , [SwaggerParameter("按比例缩放")] double? resize_p
            , [SwaggerParameter("按高度缩放")] int? resize_h
            , [SwaggerParameter("按宽度缩放")] int? resize_w
            , [SwaggerParameter("旋转度数")] int? rotate_degree
            , [SwaggerParameter("Sign")] string sign
            , [SwaggerParameter("TimeStamp")] long? t)
        {
            dynamic json = new ExpandoObject();
            json.data = null;
            if (sign.IsBlank() || !t.HasValue)
            {
                json.code = 1;
                json.msg = $"参数异常";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
            if (t.Value <= 0)
            {
                json.code = 1;
                json.msg = $"接口请求超时";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
            var dis = Math.Abs(DateTime.Now.Subtract(t.Value.TimeStampToDateTime().Value).TotalSeconds);
            if (dis > 5)
            {
                json.code = 1;
                json.msg = $"接口请求超时";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
            var _sign = $"INT.{t}.LONG".MD5Encrypt32().ToUpper();
            if (sign != _sign)
            {
                json.code = 1;
                json.msg = $"签名验证失败";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
            DakaImage dakaImage = null;
            using (DbContext my = App.sqlite)
            {
                dakaImage = my.QueryByKey<DakaImage>(id);
            }
            if (dakaImage == null)
            {
                json.code = 1;
                json.msg = $"未找到相关图片信息";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
            if (dakaImage.IsDelete == true)
            {
                json.code = 1;
                json.msg = $"图片已被删除";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
            try
            {
                MinioClient mc = new MinioClient(App.MinIOAPIEndPoint, App.MinIOAccessKey, App.MinIOSecertKey);
                string filepath = Path.Combine(App.SystemTempPath, dakaImage.SaveName);
                string new_iamge_filepath = Path.Combine(App.SystemTempPath, $"{Guid.NewGuid()}{dakaImage.FileExtension}");
                string dirpath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
                await mc.GetObjectAsync(App.MinIOImageBucket, dakaImage.SaveName, (x =>
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        x.CopyTo(fs);
                        fs.Flush();
                        fs.Dispose();
                    }
                    x.Dispose();
                }));
                using (Image image = Image.Load(filepath)) 
                {
                    if (image != null)
                    {
                        bool changed = false;
                        if (resize == true)
                        {
                            if (resize_p != null)
                            {
                                if (resize_p.Value > 2)
                                {
                                    resize_p = 2d;
                                }
                                image.Mutate(x =>
                                {
                                    x.Resize((int)(image.Width * resize_p.Value), (int)(image.Height * resize_p.Value));
                                });
                                changed = true;
                            }
                            else
                            {
                                if (resize_h.HasValue)
                                {
                                    image.Mutate(x =>
                                    {
                                        x.Resize(image.Width, resize_h.Value);
                                    });
                                    changed = true;
                                }
                                if (resize_w.HasValue)
                                {
                                    image.Mutate(x =>
                                    {
                                        x.Resize(resize_w.Value, image.Height);
                                    });
                                    changed = true;
                                }
                            }
                        }
                        if (rotate == true)
                        {
                            if (rotate_d == 1)
                            {
                                image.Mutate(x =>
                                {
                                    x.Rotate(rotate_degree.Value);
                                });
                                changed = true;
                            }
                            else
                            {
                                image.Mutate(x =>
                                {
                                    x.Rotate(360f - rotate_degree.Value);
                                });
                                changed = true;
                            }
                        }
                        if (changed)
                        {
                            image.Save(new_iamge_filepath);
                            filepath = new_iamge_filepath;
                        }
                    }
                }
                if (preview == true)
                {
                    return File(System.IO.File.ReadAllBytes(filepath), "image/jpeg");
                }
                return File(System.IO.File.ReadAllBytes(filepath), "application/octet-stream", dakaImage.SaveName);
            }
            catch (Exception e)
            {
                logger.Error($"{e}");
                json.code = 1;
                json.msg = $"{e}";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
        }

        [HttpDelete, Route("delete"), Produces("application/json")]
        public async Task<object> Delete([SwaggerParameter("图片ID")] int? id
            , [SwaggerParameter("Sign")] string sign
            , [SwaggerParameter("TimeStamp")] long? t)
        {
            dynamic json = new ExpandoObject();
            json.data = null;
            if (sign.IsBlank() || !t.HasValue)
            {
                json.code = 1;
                json.msg = $"参数异常";
                return json;
            }
            if (t.Value <= 0)
            {
                json.code = 1;
                json.msg = $"接口请求超时";
                return json;
            }
            var dis = Math.Abs(DateTime.Now.Subtract(t.Value.TimeStampToDateTime().Value).TotalSeconds);
            if (dis > 5)
            {
                json.code = 1;
                json.msg = $"接口请求超时";
                return json;
            }
            var _sign = $"INT.{t}.LONG".MD5Encrypt32().ToUpper();
            if (sign != _sign)
            {
                json.code = 1;
                json.msg = $"签名验证失败";
                return json;
            }
            DakaImage dakaImage = null;
            using (DbContext my = App.sqlite)
            {
                dakaImage = my.QueryByKey<DakaImage>(id);
            }
            if (dakaImage == null)
            {
                json.code = 1;
                json.msg = $"未找到相关图片信息";
                return json;
            }
            if (dakaImage.IsDelete == true)
            {
                json.code = 1;
                json.msg = $"图片已被删除";
                return json;
            }
            try
            {
                MinioClient mc = new MinioClient(App.MinIOAPIEndPoint, App.MinIOAccessKey, App.MinIOSecertKey);
                await mc.RemoveObjectAsync(App.MinIOImageBucket, dakaImage.SaveName);
                using (DbContext my = App.sqlite)
                {
                    dakaImage.IsDelete = true;
                    my.UpdateOnly<DakaImage>(dakaImage, a => new { a.IsDelete });
                }
                json.code = 0;
                json.msg = $"删除成功";
                return json;
            }
            catch (Exception e)
            {
                logger.Error($"{e}");
                json.code = 1;
                json.msg = $"{e}";
                return json;
            }
        }
    }
}
