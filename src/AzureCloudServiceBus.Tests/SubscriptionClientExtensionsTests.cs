// using System;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;
// using AutoFixture;
// using Microsoft.Azure.ServiceBus;
// using Newtonsoft.Json;
// using Xunit;
//
// namespace Squadron.AzureServiceBus.Tests
// {
//     public class SubscriptionClientExtensionsTests
//         : IClassFixture<AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions>>
//     {
//         private readonly AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions> _serviceBusResource;
//
//         public SubscriptionClientExtensionsTests(AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions> serviceBusResource)
//         {
//             _serviceBusResource = serviceBusResource;
//         }
//
//         [Fact(Skip = "Needs azure subscription")]
//         public async Task ReceivedMessage_NoTask_DeserializeContent_CreateDto()
//         {
//             var fixture = new Fixture();
//             UserCreated userCreated = fixture.Create<UserCreated>();
//             Message message = CreateMessage(userCreated);
//
//             ITopicClient topicClient = _serviceBusResource.GetTopicClient("foo");
//             ISubscriptionClient subscriptionClient = _serviceBusResource.GetSubscriptionClient("foo", "test1");
//
//             await topicClient.SendAsync(message);
//
//             UserCreated result = await subscriptionClient.AwaitMessageAsync(Deserialize<UserCreated>);
//
//             Assert.Equal(userCreated, result);
//         }
//
//         [Fact(Skip = "Needs azure subscription")]
//         public async Task ReceivedMessage_WithTask_DeserializeContent_CreateDto()
//         {
//             var fixture = new Fixture();
//             UserCreated userCreated = fixture.Create<UserCreated>();
//             Message message = CreateMessage(userCreated);
//
//             ITopicClient topicClient = _serviceBusResource.GetTopicClient("foo");
//             ISubscriptionClient subscriptionClient = _serviceBusResource.GetSubscriptionClient("foo", "test1");
//
//             await topicClient.SendAsync(message);
//
//             UserCreated result = await subscriptionClient.AwaitMessageAsync(DeserializeAsync<UserCreated>);
//
//             Assert.Equal(userCreated, result);
//         }
//
//         [Fact(Skip = "Needs azure subscription")]
//         public async Task NoMessage_Should_Trigger_Timeout()
//         {
//             ISubscriptionClient subscriptionClient = _serviceBusResource.GetSubscriptionClient("foo", "test1");
//
//             await Assert.ThrowsAsync<TaskCanceledException>(() =>
//                 subscriptionClient.AwaitMessageAsync(DeserializeAsync<UserCreated>, TimeSpan.FromSeconds(5)));
//         }
//
//         [Fact(Skip = "Needs azure subscription")]
//         public async Task ReceivedMessage_CanTrigger_Timeout()
//         {
//             var fixture = new Fixture();
//             UserCreated userCreated = fixture.Create<UserCreated>();
//             Message message = CreateMessage(userCreated);
//
//             ITopicClient topicClient = _serviceBusResource.GetTopicClient("foo");
//             ISubscriptionClient subscriptionClient = _serviceBusResource.GetSubscriptionClient("foo", "test1");
//
//             await topicClient.SendAsync(message);
//
//             await Assert.ThrowsAsync<TaskCanceledException>(() =>
//                     subscriptionClient.AwaitMessageAsync(async (msg, token) =>
//                 {
//                     await Task.Delay(TimeSpan.FromMinutes(5), token);
//                     return Deserialize<UserCreated>(message, token);
//                 }, TimeSpan.FromSeconds(5))
//             );
//         }
//
//         private static Message CreateMessage(UserCreated userCreated)
//         {
//             var message = new Message
//             {
//                 Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userCreated)),
//                 UserProperties = { ["EventType"] = "test1" }
//             };
//             return message;
//         }
//
//         private static T Deserialize<T>(Message message, CancellationToken cancellationToken)
//         {
//             return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));
//         }
//
//         private static Task<T> DeserializeAsync<T>(Message message, CancellationToken cancellationToken)
//         {
//             return Task.FromResult(Deserialize<T>(message, cancellationToken));
//         }
//
//         private class UserCreated : IEquatable<UserCreated>
//         {
//             public bool Equals(UserCreated other)
//             {
//                 if (ReferenceEquals(null, other)) return false;
//                 if (ReferenceEquals(this, other)) return true;
//                 return UserId == other.UserId && UserName == other.UserName && DisplayName == other.DisplayName;
//             }
//
//             public override bool Equals(object obj)
//             {
//                 if (ReferenceEquals(null, obj)) return false;
//                 if (ReferenceEquals(this, obj)) return true;
//                 if (obj.GetType() != this.GetType()) return false;
//                 return Equals((UserCreated) obj);
//             }
//
//             public override int GetHashCode()
//             {
//                 return HashCode.Combine(UserId, UserName, DisplayName);
//             }
//
//             public int UserId { get; set; }
//             public string UserName { get; set; }
//             public string DisplayName { get; set; }
//         }
//     }
// }
