using Chloe;
using DakaPathAppFileService.Domains;
using DakaPathAppFileService.ExtendMethod;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Newtonsoft.Json;
using NLog;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DakaPathAppFileService.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPut, Route("upload"), Produces("application/json")]
        public async Task<object> Upload([SwaggerParameter("文件")] IFormFile file
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
                var fileSaveName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = $"http{(App.MinIOUseSsl == true ? "s" : "")}://{App.MinIOEndPoint}/{App.MinIOImageBucket}/{fileSaveName}";
                await mc.PutObjectAsync(App.MinIOImageBucket, fileSaveName, file.OpenReadStream(), file.Length);
                DakaFile dakaFile = new DakaFile
                {
                    BucketName = App.MinIOImageBucket,
                    SaveName = fileSaveName,
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileSize = file.Length,
                    FileExtension = Path.GetExtension(file.FileName),
                    CteateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                using (DbContext my = App.sqlite)
                {
                    my.Insert(dakaFile);
                }
                dynamic _file = new ExpandoObject();
                _file.Id = dakaFile.Id;
                _file.FilePath = filePath;
                json.code = 0;
                json.msg = $"上传成功";
                json.data = _file;
                return json;
            }
            catch (Exception e)
            {
                logger.Error($"{e}");
                json.code = 1;
                json.msg = $"{e}";
                json.data = null;
                return json;
            }
        }

        [HttpGet, Route("download")]
        public async Task<IActionResult> Download([SwaggerParameter("文件ID")] int? id
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
            DakaFile dakaFile = null;
            using (DbContext my = App.sqlite)
            {
                dakaFile = my.QueryByKey<DakaFile>(id);
            }
            if (dakaFile == null)
            {
                json.code = 1;
                json.msg = $"未找到相关文件信息";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
            if (dakaFile.IsDelete == true)
            {
                json.code = 1;
                json.msg = $"文件已被删除";
                return Content(JsonConvert.SerializeObject(json), "application/json", Encoding.UTF8);
            }
            try
            {
                MinioClient mc = new MinioClient(App.MinIOAPIEndPoint, App.MinIOAccessKey, App.MinIOSecertKey);
                string filepath = Path.Combine(App.SystemTempPath, dakaFile.SaveName);
                string new_iamge_filepath = Path.Combine(App.SystemTempPath, $"{Guid.NewGuid()}{dakaFile.FileExtension}");
                string dirpath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
                await mc.GetObjectAsync(App.MinIOImageBucket, dakaFile.SaveName, (x =>
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        x.CopyTo(fs);
                        fs.Flush();
                        fs.Dispose();
                    }
                    x.Dispose();
                }));
                return File(System.IO.File.ReadAllBytes(filepath), "application/octet-stream", dakaFile.SaveName);
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
        public async Task<object> Delete([SwaggerParameter("文件ID")] int? id
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
            DakaFile dakaFile = null;
            using (DbContext my = App.sqlite)
            {
                dakaFile = my.QueryByKey<DakaFile>(id);
            }
            if (dakaFile == null)
            {
                json.code = 1;
                json.msg = $"未找到相关文件信息";
                return json;
            }
            if (dakaFile.IsDelete == true)
            {
                json.code = 1;
                json.msg = $"文件已被删除";
                return json;
            }
            try
            {
                MinioClient mc = new MinioClient(App.MinIOAPIEndPoint, App.MinIOAccessKey, App.MinIOSecertKey);
                await mc.RemoveObjectAsync(App.MinIOImageBucket, dakaFile.SaveName);
                using (DbContext my = App.sqlite)
                {
                    dakaFile.IsDelete = true;
                    my.UpdateOnly<DakaFile>(dakaFile, a => new { a.IsDelete });
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
