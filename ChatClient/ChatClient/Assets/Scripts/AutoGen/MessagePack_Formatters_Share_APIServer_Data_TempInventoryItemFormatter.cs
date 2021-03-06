// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.Share.APIServer.Data
{
    using System;
    using System.Buffers;
    using MessagePack;

    public sealed class TempInventoryItemFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Share.APIServer.Data.TempInventoryItem>
    {


        public void Serialize(ref MessagePackWriter writer, global::Share.APIServer.Data.TempInventoryItem value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.ItemId, options);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.ItemInstanceId, options);
            writer.Write(value.RemainingUses);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<string, string>>().Serialize(ref writer, value.CustomData, options);
        }

        public global::Share.APIServer.Data.TempInventoryItem Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __ItemId__ = default(string);
            var __ItemInstanceId__ = default(string);
            var __RemainingUses__ = default(int);
            var __CustomData__ = default(global::System.Collections.Generic.Dictionary<string, string>);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __ItemId__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __ItemInstanceId__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __RemainingUses__ = reader.ReadInt32();
                        break;
                    case 3:
                        __CustomData__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<string, string>>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Share.APIServer.Data.TempInventoryItem(__ItemId__, __ItemInstanceId__, __RemainingUses__);
            ____result.ItemId = __ItemId__;
            ____result.ItemInstanceId = __ItemInstanceId__;
            ____result.RemainingUses = __RemainingUses__;
            ____result.CustomData = __CustomData__;
            return ____result;
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
