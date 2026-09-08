using System;
using System.Data;
using Dapper;
using Xunit;

namespace SharpAbp.Abp.Dapper
{
    [CollectionDefinition(Name, DisableParallelization = true)]
    public sealed class DapperTypeHandlerCollection
    {
        public const string Name = "Dapper type handlers";
    }

    [Collection(DapperTypeHandlerCollection.Name)]
    public sealed class DapperTypeHandlerRegistrarTests : IDisposable
    {
        public DapperTypeHandlerRegistrarTests()
        {
            ResetDapperState();
        }

        [Fact]
        public void Register_Should_Use_Dapper_Default_When_No_Mapping_Matches()
        {
            var guid = Guid.Parse("adf8fad4-4a9f-4e76-8bd9-60dbffdf4a2d");
            var options = new SharpAbpDapperOptions();
            options.TypeMappings.Add(new BinaryGuidTypeMapping());

            DapperTypeHandlerRegistrar.Register(options);
            var handler = GetRequiredTypeHandler(typeof(Guid));

            var defaultParameter = new TestDbParameter();
            handler.SetValue(defaultParameter, guid);
            Assert.Equal(guid, defaultParameter.Value);
            Assert.Equal(DbType.Guid, defaultParameter.DbType);

            var binaryParameter = new BinaryDbParameter();
            handler.SetValue(binaryParameter, guid);
            Assert.Equal(guid.ToByteArray(), binaryParameter.Value);
            Assert.Equal(DbType.Binary, binaryParameter.DbType);

            Assert.Equal(guid, handler.Parse(typeof(Guid), guid));
            Assert.Equal(guid, handler.Parse(typeof(Guid), guid.ToByteArray()));
        }

        [Fact]
        public void Register_Should_Use_One_Handler_For_Value_And_Nullable_Types()
        {
            var options = new SharpAbpDapperOptions();
            options.TypeMappings.Add(new BinaryGuidTypeMapping());

            DapperTypeHandlerRegistrar.Register(options);

            var valueHandler = GetRequiredTypeHandler(typeof(Guid));
            var nullableHandler = GetRequiredTypeHandler(typeof(Guid?));
            Assert.Same(valueHandler, nullableHandler);
        }

        [Fact]
        public void Register_Should_Not_Remove_Unrelated_Type_Handlers()
        {
            SqlMapper.AddTypeHandler(new TestValueTypeHandler());
            var options = new SharpAbpDapperOptions();
            options.TypeMappings.Add(new BinaryGuidTypeMapping());

            DapperTypeHandlerRegistrar.Register(options);

            Assert.True(SqlMapper.HasTypeHandler(typeof(TestValue)));
        }

        [Fact]
        public void Register_Should_Preserve_Existing_Handler_For_The_Same_Type_As_Fallback()
        {
            var expected = Guid.Parse("c6a97184-0bd6-4570-9107-fcd9319480ad");
            SqlMapper.AddTypeHandler(new ExistingGuidTypeHandler(expected));
            var options = new SharpAbpDapperOptions();
            options.TypeMappings.Add(new BinaryGuidTypeMapping());

            DapperTypeHandlerRegistrar.Register(options);
            var handler = GetRequiredTypeHandler(typeof(Guid));

            Assert.Equal(expected, handler.Parse(typeof(Guid), "existing-handler"));
        }

        [Fact]
        public void Register_Should_Use_Dapper_Default_Enum_Parameter_Value_When_No_Mapping_Matches()
        {
            var options = new SharpAbpDapperOptions();
            options.TypeMappings.Add(new BinaryObjectTypeMapping());
            DapperTypeHandlerRegistrar.Register(options);
            var handler = GetRequiredTypeHandler(typeof(object));
            var parameter = new TestDbParameter();

            handler.SetValue(parameter, TestStatus.Active);

            Assert.Equal(DbType.Object, parameter.DbType);
            Assert.Equal((short)1, Assert.IsType<short>(parameter.Value));
        }

        [Fact]
        public void Register_Should_Use_Dapper_Conversion_Operator_When_No_Parse_Mapping_Matches()
        {
            var options = new SharpAbpDapperOptions();
            options.TypeMappings.Add(new BinaryOperatorValueTypeMapping());
            DapperTypeHandlerRegistrar.Register(options);
            var handler = GetRequiredTypeHandler(typeof(OperatorValue));

            var result = handler.Parse(typeof(OperatorValue), 42);

            Assert.Equal(new OperatorValue(42), result);
        }

        [Fact]
        public void Dispatcher_Should_Reject_Ambiguous_Mappings()
        {
            var options = new SharpAbpDapperOptions();
            options.TypeMappings.Add(new AlwaysMatchingGuidTypeMapping());
            options.TypeMappings.Add(new AnotherAlwaysMatchingGuidTypeMapping());
            DapperTypeHandlerRegistrar.Register(options);
            var handler = GetRequiredTypeHandler(typeof(Guid));

            var exception = Assert.Throws<InvalidOperationException>(() =>
                handler.SetValue(new TestDbParameter(), Guid.NewGuid()));

            Assert.Contains("Multiple Dapper type mappings", exception.Message);
        }

        [Fact]
        public void Register_Should_Preserve_Configured_Instances_Of_The_Same_Mapping_Type()
        {
            var options = new SharpAbpDapperOptions();
            options.TypeMappings.Add(new ConfigurableGuidTypeMapping(typeof(FirstDbParameter), DbType.Binary));
            options.TypeMappings.Add(new ConfigurableGuidTypeMapping(typeof(SecondDbParameter), DbType.String));
            DapperTypeHandlerRegistrar.Register(options);
            var handler = GetRequiredTypeHandler(typeof(Guid));

            var firstParameter = new FirstDbParameter();
            handler.SetValue(firstParameter, Guid.NewGuid());
            Assert.Equal(DbType.Binary, firstParameter.DbType);

            var secondParameter = new SecondDbParameter();
            handler.SetValue(secondParameter, Guid.NewGuid());
            Assert.Equal(DbType.String, secondParameter.DbType);
        }

        public void Dispose()
        {
            ResetDapperState();
        }

#pragma warning disable CS0618
        private static SqlMapper.ITypeHandler GetRequiredTypeHandler(Type type)
        {
            SqlMapper.LookupDbType(type, "value", false, out var handler);
            return Assert.IsAssignableFrom<SqlMapper.ITypeHandler>(handler);
        }
#pragma warning restore CS0618

        private static void ResetDapperState()
        {
            SqlMapper.ResetTypeHandlers();
            SqlMapper.AddTypeMap(typeof(Guid), DbType.Guid);
            SqlMapper.AddTypeMap(typeof(bool), DbType.Boolean);
        }

        private sealed class BinaryGuidTypeMapping : DapperTypeMapping<Guid>
        {
            public override bool CanSetValue(IDbDataParameter parameter) => parameter is BinaryDbParameter;

            public override bool CanParse(object? value) => value is byte[] { Length: 16 };

            public override void SetValue(IDbDataParameter parameter, Guid value)
            {
                parameter.DbType = DbType.Binary;
                parameter.Value = value.ToByteArray();
            }

            public override Guid Parse(object value) => new((byte[])value);
        }

        private sealed class AlwaysMatchingGuidTypeMapping : DapperTypeMapping<Guid>
        {
            public override bool CanSetValue(IDbDataParameter parameter) => true;
            public override bool CanParse(object? value) => true;
            public override void SetValue(IDbDataParameter parameter, Guid value) => parameter.Value = value;
            public override Guid Parse(object value) => Guid.Empty;
        }

        private sealed class AnotherAlwaysMatchingGuidTypeMapping : DapperTypeMapping<Guid>
        {
            public override bool CanSetValue(IDbDataParameter parameter) => true;
            public override bool CanParse(object? value) => true;
            public override void SetValue(IDbDataParameter parameter, Guid value) => parameter.Value = value;
            public override Guid Parse(object value) => Guid.Empty;
        }

        private sealed class ConfigurableGuidTypeMapping : DapperTypeMapping<Guid>
        {
            private readonly Type _parameterType;
            private readonly DbType _dbType;

            public ConfigurableGuidTypeMapping(Type parameterType, DbType dbType)
            {
                _parameterType = parameterType;
                _dbType = dbType;
            }

            public override bool CanSetValue(IDbDataParameter parameter) => parameter.GetType() == _parameterType;
            public override bool CanParse(object? value) => false;

            public override void SetValue(IDbDataParameter parameter, Guid value)
            {
                parameter.DbType = _dbType;
                parameter.Value = value;
            }

            public override Guid Parse(object value) => throw new NotSupportedException();
        }

        private sealed class FirstDbParameter : TestDbParameter
        {
        }

        private sealed class SecondDbParameter : TestDbParameter
        {
        }

        private readonly record struct TestValue(int Value);

        private enum TestStatus : short
        {
            Active = 1
        }

        private sealed class BinaryObjectTypeMapping : DapperTypeMapping<object>
        {
            public override bool CanSetValue(IDbDataParameter parameter) => parameter is BinaryDbParameter;
            public override bool CanParse(object? value) => false;
            public override void SetValue(IDbDataParameter parameter, object value) => parameter.Value = value;
            public override object Parse(object value) => value;
        }

        private readonly record struct OperatorValue(int Value)
        {
            public static implicit operator OperatorValue(int value) => new(value);
        }

        private sealed class BinaryOperatorValueTypeMapping : DapperTypeMapping<OperatorValue>
        {
            public override bool CanSetValue(IDbDataParameter parameter) => parameter is BinaryDbParameter;
            public override bool CanParse(object? value) => value is byte[];
            public override void SetValue(IDbDataParameter parameter, OperatorValue value) => parameter.Value = value.Value;
            public override OperatorValue Parse(object value) => new(((byte[])value).Length);
        }

        private sealed class TestValueTypeHandler : SqlMapper.TypeHandler<TestValue>
        {
            public override void SetValue(IDbDataParameter parameter, TestValue value) => parameter.Value = value.Value;
            public override TestValue Parse(object value) => new(Convert.ToInt32(value));
        }

        private sealed class ExistingGuidTypeHandler : SqlMapper.TypeHandler<Guid>
        {
            private readonly Guid _parsedValue;

            public ExistingGuidTypeHandler(Guid parsedValue)
            {
                _parsedValue = parsedValue;
            }

            public override void SetValue(IDbDataParameter parameter, Guid value) => parameter.Value = value;
            public override Guid Parse(object value) => _parsedValue;
        }

    }
}
