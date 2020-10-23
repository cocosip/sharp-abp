using FellowOakDicom;
using SharpAbp.Abp.FoDicom.TestObjects;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace SharpAbp.Abp.FoDicom
{
    public class DicomTagTest
    {
        [Fact]
        public void DicomTag_Object_Serialize_Deserialize_Test()
        {
            var o1 = new TestObject1()
            {
                Id = 1,
                Name = "zhangsan",
                Tag1 = DicomTag.StudyInstanceUID,
                Tag2 = DicomTag.AccessionNumber
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(new DicomTagConverter());

            var json = JsonSerializer.Serialize(o1, options);
            var o2 = JsonSerializer.Deserialize<TestObject1>(json, options);

            Assert.Equal(1, o2.Id);
            Assert.Equal("zhangsan", o2.Name);
            Assert.Equal(DicomTag.StudyInstanceUID, o2.Tag1);
            Assert.Equal(DicomTag.AccessionNumber, o2.Tag2);
        }


        [Fact]
        public void DicomTag_Object_List_Serialize_Deserialize_Test()
        {
            var o1 = new TestObject1()
            {
                Id = 1,
                Name = "zhangsan",
                Tag1 = DicomTag.StudyInstanceUID,
                Tag2 = DicomTag.AccessionNumber
            };

            var o2 = new TestObject1()
            {
                Id = 2,
                Name = "lisi",
                Tag1 = DicomTag.StudyID,
                Tag2 = DicomTag.StudyDate
            };

            var list = new List<TestObject1>()
            {
                o1,
                o2
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(new DicomTagConverter());

            var json = JsonSerializer.Serialize(list, options);
            var list2 = JsonSerializer.Deserialize<List<TestObject1>>(json, options);

            Assert.Equal(2, list2.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal("zhangsan", list[0].Name);
            Assert.Equal(DicomTag.StudyInstanceUID, list[0].Tag1);
            Assert.Equal(DicomTag.AccessionNumber, list[0].Tag2);

            Assert.Equal(2, list[1].Id);
            Assert.Equal("lisi", list[1].Name);
            Assert.Equal(DicomTag.StudyID, list[1].Tag1);
            Assert.Equal(DicomTag.StudyDate, list[1].Tag2);

        }

        [Fact]
        public void DicomTag_Dictionary_Serialize_Deserialize_Test()
        {
            var dict = new Dictionary<DicomTag, string>()
            {
                { DicomTag.AccessionNumber,"123456" },
                { DicomTag.SeriesNumber,"3" },
                { DicomTag.PatientName,"zhangsan" }
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(new DictionaryTKeyDicomTagTValueConverter());
            options.Converters.Add(new DicomTagConverter());
            var json = JsonSerializer.Serialize(dict, options);

            Assert.NotEmpty(json);

            var dict2 = JsonSerializer.Deserialize<Dictionary<DicomTag, string>>(json, options);
            Assert.Equal(3, dict2.Count);
            Assert.Equal("123456", dict2[DicomTag.AccessionNumber]);
            Assert.Equal("3", dict2[DicomTag.SeriesNumber]);
            Assert.Equal("zhangsan", dict2[DicomTag.PatientName]);

        }

    }
}
