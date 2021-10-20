using Chloe.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakaPathAppFileService.Domains
{
    public class File
    {
        private int _id;
        [Column("id", IsPrimaryKey = true), AutoIncrement]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private string _fileName;
        [Column("file_name")]
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        private string _bucketName;
        [Column("bucket_name")]
        public string BucketName
        {
            get
            {
                return _bucketName;
            }
            set
            {
                _bucketName = value;
            }
        }

        private string _saveName;
        [Column("save_name")]
        public string SaveName
        {
            get
            {
                return _saveName;
            }
            set
            {
                _saveName = value;
            }
        }

        private string _filePath;
        [Column("file_path")]
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
            }
        }

        private long? _fileSize;
        [Column("file_size")]
        public long? FileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
            }
        }

        private string _fileExtension;
        [Column("file_extension")]
        public string FileExtension
        {
            get
            {
                return _fileExtension;
            }
            set
            {
                _fileExtension = value;
            }
        }

        private DateTime? _cteateTime;
        [Column("cteate_time")]
        public DateTime? CteateTime
        {
            get
            {
                return _cteateTime;
            }
            set
            {
                _cteateTime = value;
            }
        }

        private DateTime? _updateTime;
        [Column("update_time")]
        public DateTime? UpdateTime
        {
            get
            {
                return _updateTime;
            }
            set
            {
                _updateTime = value;
            }
        }

        private bool? _isDelete = false;
        [Column("is_delete")]
        public bool? IsDelete
        {
            get
            {
                return _isDelete;
            }
            set
            {
                _isDelete = value;
            }
        }
    }
}
