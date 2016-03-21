namespace AzureBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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
        /// <param name="subscription">The subscription identifier.</param>
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
                    MethodInfo busSubscribeMethod = genericBusSubscribeMethod.MakeGenericMethod(subscriptionInfo.MessageType);

                    MethodInfo consumeMethod = subscriptionInfo.ConcreteType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .Where(m => m.Name == "Consume" && m.GetParameters().Any(predicate => predicate.ParameterType == subscriptionInfo.MessageType))
                        .SingleOrDefault();

                    Delegate dispatchDelegate = Delegate.CreateDelegate(dispatchMethodType, Activator.CreateInstance(subscriptionInfo.ConcreteType), consumeMethod);
                    busSubscribeMethod.Invoke(bus, new object[] { string.Format("{0}.{1}", this.subscriptionId, subscriptionInfo.ConcreteType.Name), dispatchDelegate });
                }
            }
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
