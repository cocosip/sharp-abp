using Spool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Spool
{
    public class SpoolPoolTest : AbpSpoolTestBase
    {
        private readonly ISpoolPool _spoolPool;

        public SpoolPoolTest()
        {
            _spoolPool = GetRequiredService<ISpoolPool>();
        }


        [Fact]
        public async Task Write_Read_Return_Release_Test()
        {
            var content = Encoding.UTF8.GetBytes("Hello");
            var writeFile = await _spoolPool.WriteAsync(content, ".txt");
            Assert.True(File.Exists(writeFile.Path));

            Assert.Equal(1, _spoolPool.GetPendingCount());
            Assert.Equal(0, _spoolPool.GetProcessingCount());
            var getFiles = _spoolPool.Get(2);

            Assert.Single(getFiles);
            Assert.Equal(0, _spoolPool.GetPendingCount());
            Assert.Equal(0, _spoolPool.GetProcessingCount());
  
            _spoolPool.Return("default", getFiles);
            Assert.Equal(1, _spoolPool.GetPendingCount());
            //未开启自动归还,被取走的数据始终为0
            Assert.Equal(0, _spoolPool.GetProcessingCount());

            var reGetFiles = _spoolPool.Get(3);

            _spoolPool.Release("default", reGetFiles);
            foreach(var reGetFile in reGetFiles)
            {
                Assert.False(File.Exists(reGetFile.Path));
            }

            _spoolPool.Dispose();

            var poolPath = Path.Combine(AppContext.BaseDirectory, "default1");
            Assert.True(Directory.Exists(poolPath));
            Directory.Delete(poolPath, true);

            Assert.False(Directory.Exists(poolPath));
        }


    }
}
