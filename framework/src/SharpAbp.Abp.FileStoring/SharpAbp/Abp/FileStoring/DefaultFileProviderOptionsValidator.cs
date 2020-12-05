//using JetBrains.Annotations;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using Volo.Abp;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.Reflection;

//namespace SharpAbp.Abp.FileStoring
//{
//    public class DefaultFileProviderOptionsValidator : IFileProviderValuesValidator, ITransientDependency
//    {
//        protected AbpFileStoringOptions Options { get; }
//        public DefaultFileProviderOptionsValidator(IOptions<AbpFileStoringOptions> options)
//        {
//            Options = options.Value;
//        }

//        public virtual void Validate([NotNull] string provider, Dictionary<string, string> keyValuePairs)
//        {
//            Check.NotNullOrWhiteSpace(provider, nameof(provider));
//            var providerConfiguration = Options.Providers.GetConfiguration(provider);

//            if (providerConfiguration == null)
//            {
//                throw new AbpException($"Could not find any provider configuration for provider '{provider}'.");
//            }

//            foreach (var propertyKv in providerConfiguration.GetValues())
//            {
//                if (keyValuePairs.ContainsKey(propertyKv.Key))
//                {
//                    string value = "";
//                    try
//                    {
//                        keyValuePairs.TryGetValue(propertyKv.Key, out value);
//                        _ = TypeHelper.ConvertFromString(propertyKv.Value.Type, value);
//                    }
//                    catch (Exception ex)
//                    {
//                        throw new AbpException($"Provider '{provider}' could not convert value '{value}' to type '{propertyKv.Value.Type.FullName}',exception:{ex.Message} .");
//                    }
//                }
//                else
//                {
//                    throw new AbpException($"Provider '{provider}' miss name {propertyKv.Key}'s value.");
//                }
//            }
//        }
//    }
//}
