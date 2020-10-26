using FellowOakDicom;
using System;
using Xunit;

namespace SharpAbp.Abp.FoDicom
{
    public class FoDicomExtensionsTest
    {
        [Fact]
        public void GetAge_Normal_Test()
        {
            var dicomDataset1 = new DicomDataset
            {
                { DicomTag.PatientAge, "048Y" }
            };
            var age1 = dicomDataset1.GetAge(DicomTag.PatientAge);

            Assert.Equal(48, age1.Age);
            Assert.Equal(DicomAgeMode.Y, age1.Mode);

            var dicomDataset2 = new DicomDataset();
            dicomDataset2.AddOrUpdate(DicomTag.SpecificCharacterSet, "GB18030");
            dicomDataset2.NotValidated();
            dicomDataset2.Add(DicomTag.PatientAge, "043岁");
            var age2 = dicomDataset2.GetAge(DicomTag.PatientAge);

            Assert.Equal(43, age2.Age);
            Assert.Equal(DicomAgeMode.Y, age2.Mode);

        }


        [Fact]
        public void GetAge_Other_Mode_Test()
        {
            var dicomDataset1 = new DicomDataset
            {
                { DicomTag.PatientAge, "012M" }
            };
            var age1 = dicomDataset1.GetAge(DicomTag.PatientAge);

            Assert.Equal(12, age1.Age);
            Assert.Equal(DicomAgeMode.M, age1.Mode);

            var dicomDataset2 = new DicomDataset();
            dicomDataset2.AddOrUpdate(DicomTag.SpecificCharacterSet, "GB18030");
            dicomDataset2.NotValidated();
            dicomDataset2.Add(DicomTag.PatientAge, "003月");
            var age2 = dicomDataset2.GetAge(DicomTag.PatientAge);

            Assert.Equal(3, age2.Age);
            Assert.Equal(DicomAgeMode.M, age2.Mode);

        }

        [Fact]
        public void GetDate_Test()
        {
            var dicomDataset = new DicomDataset();
            dicomDataset.AddOrUpdate(DicomTag.PatientBirthDate, "20150206");
            var d1 = dicomDataset.GetDate(DicomTag.PatientBirthDate);
            Assert.Equal(2015, d1.Value.Year);
            Assert.Equal(2, d1.Value.Month);
            Assert.Equal(6, d1.Value.Day);

            var d2 = dicomDataset.GetDate(DicomTag.StudyDate, new DateTime(2020, 01, 01));
            Assert.Equal(2020, d2.Value.Year);
            Assert.Equal(1, d2.Value.Month);
            Assert.Equal(1, d2.Value.Day);
        }

        [Fact]
        public void GetTime_Test()
        {
            var dicomDataset = new DicomDataset();
            dicomDataset.AddOrUpdate(DicomTag.StudyTime, "231520");
            dicomDataset.AddOrUpdate(DicomTag.SeriesTime, "182015.333");
            dicomDataset.AddOrUpdate(DicomTag.PatientBirthTime, "2019");
            var d1 = dicomDataset.GetTime(DicomTag.StudyTime);
            Assert.Equal(23, d1.Value.Hour);
            Assert.Equal(15, d1.Value.Minute);
            Assert.Equal(20, d1.Value.Second);

            var d2 = dicomDataset.GetTime(DicomTag.SeriesTime);
            Assert.Equal(18, d2.Value.Hour);
            Assert.Equal(20, d2.Value.Minute);
            Assert.Equal(15, d2.Value.Second);
            Assert.Equal(333, d2.Value.Millisecond);

            var d3 = dicomDataset.GetTime(DicomTag.PatientBirthTime);
            Assert.Equal(20, d3.Value.Hour);
            Assert.Equal(19, d3.Value.Minute);
            Assert.Equal(0, d3.Value.Second);
        }


        [Fact]
        public void GetEncodingValue_Test()
        {
            var dicomDataset = new DicomDataset();
            dicomDataset.NotValidated();
            dicomDataset.AddOrUpdate(DicomTag.SpecificCharacterSet, "SO_IR 100");

            dicomDataset.AddOrUpdate(new DicomPersonName(DicomTag.PatientName, DicomEncoding.GetEncoding("SO_IR 100"), FellowOakDicom.IO.ByteConverter.ToByteBuffer("张三", DicomEncoding.GetEncoding("GB18030"))));

            dicomDataset.AddOrUpdate(new DicomPersonName(DicomTag.BodyPartExamined, DicomEncoding.GetEncoding("SO_IR 100"), FellowOakDicom.IO.ByteConverter.ToByteBuffer("膝盖", DicomEncoding.GetEncoding("GB18030"))));

            var value1 = dicomDataset.GetEncodingValue(DicomTag.PatientName);
            var value2 = dicomDataset.GetSingleValueOrDefault(DicomTag.PatientName, "");
            Assert.NotEqual(value1, value2);
            Assert.Equal("张三", value1);

            var value3 = dicomDataset.GetEncodingValue(DicomTag.BodyPartExamined);
            var value4 = dicomDataset.GetSingleValueOrDefault(DicomTag.BodyPartExamined, "");
            Assert.NotEqual(value3, value4);
            Assert.Equal("膝盖", value3);
        }





    }
}
