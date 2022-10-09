namespace BotBuilderMock
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ProtoBuf;
    // Copyright (c) Microsoft Corporation. All rights reserved.
    // Licensed under the MIT License.

    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Mail;

    namespace Microsoft.Bot.Schema
    {

        [ProtoContract]
        public class ChannelAccount
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            [ProtoMember(2)]
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [ProtoMember(3)]
            [JsonProperty(PropertyName = "aadObjectId")]
            public string AadObjectId { get; set; }

            [ProtoMember(4)]
            //[JsonExtensionData(ReadData = true, WriteData = true)]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
            public /*JObject*/string Properties { get; set; } = String.Empty; /* new JObject();*/ // HACK: cannot serialize JObject
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(5)]
            [JsonProperty(PropertyName = "role")]
            public string Role { get; set; }
        }

        [ProtoContract]
        public class ConversationAccount
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "isGroup")]
            public bool? IsGroup { get; set; }

            [ProtoMember(2)]
            [JsonProperty(PropertyName = "conversationType")]
            public string ConversationType { get; set; }

            [ProtoMember(3)]
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            [ProtoMember(4)]
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [ProtoMember(5)]
            [JsonProperty(PropertyName = "aadObjectId")]
            public string AadObjectId { get; set; }

            [ProtoMember(6)]
            //[JsonExtensionData(ReadData = true, WriteData = true)]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
            public /*JObject*/string Properties { get; set; } = string.Empty; /* new JObject();*/ // HACK: cannot serialize JObject
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(7)]
            [JsonProperty(PropertyName = "role")]
            public string Role { get; set; }

            [ProtoMember(8)]
            [JsonProperty(PropertyName = "tenantId")]
            public string TenantId { get; set; }
        }

        [ProtoContract]
        public partial class MessageReaction
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }
        }

        [ProtoContract]
        public class SuggestedActions
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "to")]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking compat).
            public IList<string> To { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(2)]
            [JsonProperty(PropertyName = "actions")]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking compat).
            public IList<String> Actions { get; set; }  // HACK: type CardAction replaced with String
#pragma warning restore CA2227 // Collection properties should be read only
        }

        [ProtoContract]
        public class Entity
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }

            [ProtoMember(2)]
            [JsonExtensionData(ReadData = true, WriteData = true)]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
            public JObject Properties { get; set; } = new JObject();
#pragma warning restore CA2227 // Collection properties should be read only
        }

        [ProtoContract]
        public class ConversationReference
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "activityId")]
            public string ActivityId { get; set; }

            [ProtoMember(2)]
            [JsonProperty(PropertyName = "user")]
            public ChannelAccount User { get; set; }

            [ProtoMember(3)]
            [JsonProperty(PropertyName = "bot")]
            public ChannelAccount Bot { get; set; }

            [ProtoMember(4)]
            [JsonProperty(PropertyName = "conversation")]
            public ConversationAccount Conversation { get; set; }

            [ProtoMember(5)]
            [JsonProperty(PropertyName = "channelId")]
            public string ChannelId { get; set; }

            [ProtoMember(6)]
            [JsonProperty(PropertyName = "locale")]
            public string Locale { get; set; }

            [ProtoMember(7)]
            [JsonProperty(PropertyName = "serviceUrl")]
#pragma warning disable CA1056 // Uri properties should not be strings
            public string ServiceUrl { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings
        }

        [ProtoContract]
        public partial class TextHighlight
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }

            [ProtoMember(2)]
            [JsonProperty(PropertyName = "occurrence")]
            public int? Occurrence { get; set; }
        }

        [ProtoContract]
        public partial class SemanticAction
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            [ProtoMember(2)]
            [JsonProperty(PropertyName = "entities")]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking compat).
            public IDictionary<string, Entity> Entities { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(3)]
            [JsonProperty(PropertyName = "state")]
            public string State { get; set; }
        }

        [ProtoContract]
        public partial class Activity
        {
            [ProtoMember(1)]
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }

            [ProtoMember(2)]
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            [ProtoMember(3)]
            [JsonProperty(PropertyName = "timestamp")]
            public System.DateTime? Timestamp { get; set; } // Hack: used to be DateTimeOffset, not serializable with protobuf.net

            [ProtoMember(4)]
            [JsonProperty(PropertyName = "localTimestamp")]
            public System.DateTime? LocalTimestamp { get; set; } // Hack: used to be DateTimeOffset, not serializable with protobuf.net

            [ProtoMember(5)]
            [JsonProperty(PropertyName = "localTimezone")]
            public string LocalTimezone { get; set; }

            [ProtoMember(6)]
            [JsonProperty(PropertyName = "serviceUrl")]
            public string ServiceUrl { get; set; }

            [ProtoMember(7)]
            [JsonProperty(PropertyName = "channelId")]
            public string ChannelId { get; set; }

            [ProtoMember(8)]
            [JsonProperty(PropertyName = "from")]
            public ChannelAccount From { get; set; }

            [ProtoMember(9)]
            [JsonProperty(PropertyName = "conversation")]
            public ConversationAccount Conversation { get; set; }

            [ProtoMember(10)]
            [JsonProperty(PropertyName = "recipient")]
            public ChannelAccount Recipient { get; set; }

            [ProtoMember(11)]
            [JsonProperty(PropertyName = "textFormat")]
            public string TextFormat { get; set; }

            [ProtoMember(12)]
            [JsonProperty(PropertyName = "attachmentLayout")]
            public string AttachmentLayout { get; set; }

            [ProtoMember(13)]
            [JsonProperty(PropertyName = "membersAdded")]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
            public IList<ChannelAccount> MembersAdded { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(14)]
            [JsonProperty(PropertyName = "membersRemoved")]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
            public IList<ChannelAccount> MembersRemoved { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(15)]
            [JsonProperty(PropertyName = "reactionsAdded")]
#pragma warning disable CA2227 // Collection properties should be read only  (we can't change this without breaking binary compat)
            public IList<MessageReaction> ReactionsAdded { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(16)]
            [JsonProperty(PropertyName = "reactionsRemoved")]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
            public IList<MessageReaction> ReactionsRemoved { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(17)]
            [JsonProperty(PropertyName = "topicName")]
            public string TopicName { get; set; }

            [ProtoMember(18)]
            [JsonProperty(PropertyName = "historyDisclosed")]
            public bool? HistoryDisclosed { get; set; }

            [ProtoMember(19)]
            [JsonProperty(PropertyName = "locale")]
            public string Locale { get; set; }

            [ProtoMember(20)]
            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }

            [ProtoMember(21)]
            [JsonProperty(PropertyName = "speak")]
            public string Speak { get; set; }

            [ProtoMember(22)]
            [JsonProperty(PropertyName = "inputHint")]
            public string InputHint { get; set; }

            [ProtoMember(23)]
            [JsonProperty(PropertyName = "summary")]
            public string Summary { get; set; }

            [ProtoMember(24)]
            [JsonProperty(PropertyName = "suggestedActions")]
            public SuggestedActions SuggestedActions { get; set; }

            [ProtoMember(25)]
            [JsonProperty(PropertyName = "attachments")]
#pragma warning disable CA2227 // Collection properties should be read only  (we can't change this without breaking binary compat)
            public IList<Attachment> Attachments { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(26)]
            [JsonProperty(PropertyName = "entities")]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
            public IList<Entity> Entities { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(27)]
            [JsonProperty(PropertyName = "channelData")]
#pragma warning disable CA1721 // Property names should not match get methods (we can't change this without changing binary compat).
            
             // HACK - object
             public string ChannelData { get; set; }
#pragma warning restore CA1721 // Property names should not match get methods

            [ProtoMember(28)]
            [JsonProperty(PropertyName = "action")]
            public string Action { get; set; }

            [ProtoMember(29)]
            [JsonProperty(PropertyName = "replyToId")]
            public string ReplyToId { get; set; }

            [ProtoMember(30)]
            [JsonProperty(PropertyName = "label")]
            public string Label { get; set; }

            [ProtoMember(31)]
            [JsonProperty(PropertyName = "valueType")]
            public string ValueType { get; set; }

            [ProtoMember(32)]
            [JsonProperty(PropertyName = "value")]
            public string Value { get; set; }   // HACK - object

            [ProtoMember(33)]
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [ProtoMember(34)]
            [JsonProperty(PropertyName = "relatesTo")]
            public ConversationReference RelatesTo { get; set; }

            [ProtoMember(35)]
            [JsonProperty(PropertyName = "code")]
            public string Code { get; set; }

            [ProtoMember(36)]
            [JsonProperty(PropertyName = "expiration")]
            public System.DateTime? Expiration { get; set; } // Hack: used to be DateTimeOffset, not serializable with protobuf.net

            [ProtoMember(37)]
            [JsonProperty(PropertyName = "importance")]
            public string Importance { get; set; }

            [ProtoMember(38)]
            [JsonProperty(PropertyName = "deliveryMode")]
            public string DeliveryMode { get; set; }

            [ProtoMember(39)]
            [JsonProperty(PropertyName = "listenFor")]
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
            public IList<string> ListenFor { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(40)]
            [JsonProperty(PropertyName = "textHighlights")]
#pragma warning disable CA2227 // Collection properties should be read only  (we can't change this without breaking binary compat)
            public IList<TextHighlight> TextHighlights { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

            [ProtoMember(41)]
            [JsonProperty(PropertyName = "semanticAction")]
            public SemanticAction SemanticAction { get; set; }

            [ProtoMember(42)]
            [JsonProperty(PropertyName = "callerId")]
            public string CallerId { get; set; }
        }
    }

}