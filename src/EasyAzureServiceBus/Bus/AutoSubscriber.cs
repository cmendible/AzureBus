namespace EasyAzureServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Lets you scan assemblies for implementations of <see cref="IConsume{T}"/> so that
    /// these will get registrered as subscribers in the bus.
    /// </summary>
    public class AutoSubscriber
    {
        private readonly IBus bus;
        private readonly string subscriptionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSubscriber"/> class.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        public AutoSubscriber(IBus bus, string subscriptionId)
        {
            this.bus = bus;
            this.subscriptionId = subscriptionId;
        }

        /// <summary>
        /// Registers all consumers in passed assembly. The SubscriptionId per consumer
        /// method is determined by <seealso cref="GenerateSubscriptionId"/>.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for consumers.</param>
        public virtual void Subscribe(params Assembly[] assemblies)
        {
            var genericBusSubscribeMethod = GetSubscribeMethodOfBus();
            var subscriptionInfos = GetSubscriptionInfos(assemblies.SelectMany(a => a.GetTypes()));

            foreach (KeyValuePair<Type, ConsumerInfo[]> kv in subscriptionInfos)
            {
                foreach (ConsumerInfo subscriptionInfo in kv.Value)
                {
                    Type dispatchMethodType = typeof(Action<>).MakeGenericType(subscriptionInfo.MessageType);
                    string subscriptionId = DefaultSubscriptionIdGenerator(subscriptionInfo);
                    MethodInfo busSubscribeMethod = genericBusSubscribeMethod.MakeGenericMethod(subscriptionInfo.MessageType);

                    MethodInfo consumeMethod = subscriptionInfo.ConcreteType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .Where(m => m.Name == "Consume" && m.GetParameters().Any(predicate => predicate.ParameterType == subscriptionInfo.MessageType))
                        .SingleOrDefault();

                    Delegate dispatchDelegate = Delegate.CreateDelegate(dispatchMethodType, Activator.CreateInstance(subscriptionInfo.ConcreteType), consumeMethod);
                    busSubscribeMethod.Invoke(bus, new object[] { subscriptionId, dispatchDelegate });
                }
            }
        }

        /// <summary>
        /// The default subscription identifier generator.
        /// </summary>
        /// <param name="consumerInfo">The c.</param>
        /// <returns></returns>
        protected virtual string DefaultSubscriptionIdGenerator(ConsumerInfo consumerInfo)
        {
            StringBuilder r = new StringBuilder();
            string unique = string.Concat(this.subscriptionId, ":", consumerInfo.ConcreteType.FullName, ":", consumerInfo.MessageType.FullName);

            using (MD5 md5 = MD5.Create())
            {
                byte[] buff = md5.ComputeHash(Encoding.UTF8.GetBytes(unique));
                foreach (byte b in buff)
                {
                    r.Append(b.ToString("x2"));
                }
            }

            return string.Concat(this.subscriptionId, "_", r.ToString());
        }

        /// <summary>
        /// Gets the subscribe method of the bus.
        /// </summary>
        /// <returns></returns>
        protected virtual MethodInfo GetSubscribeMethodOfBus()
        {
            return bus.GetType().GetMethods()
                .Where(m => m.Name == "Subscribe")
                .Select(m => new { Method = m, Params = m.GetParameters() })
                .Single(m => m.Params.Length == 2
                    && m.Params[0].ParameterType == typeof(string)
                    && m.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>)).Method;
        }

        /// <summary>
        /// Gets the subscription infos.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValuePair<Type, ConsumerInfo[]>> GetSubscriptionInfos(IEnumerable<Type> types)
        {
            foreach (Type concreteType in types.Where(t => t.IsClass))
            {
                ConsumerInfo[] subscriptionInfos = concreteType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsume<>))
                    .Select(i => new ConsumerInfo(concreteType, i, i.GetGenericArguments()[0]))
                    .ToArray();

                if (subscriptionInfos.Any())
                    yield return new KeyValuePair<Type, ConsumerInfo[]>(concreteType, subscriptionInfos);
            }
        }
    }
}
