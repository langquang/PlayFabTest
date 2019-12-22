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

namespace MessagePack.Formatters.SourceShare.Share.TransportData
{
    using System;
    using System.Buffers;
    using MessagePack;

    public sealed class CommonErrorResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::SourceShare.Share.TransportData.CommonErrorResponse>
    {


        public void Serialize(ref MessagePackWriter writer, global::SourceShare.Share.TransportData.CommonErrorResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(0);
        }

        public global::SourceShare.Share.TransportData.CommonErrorResponse Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::SourceShare.Share.TransportData.CommonErrorResponse();
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
