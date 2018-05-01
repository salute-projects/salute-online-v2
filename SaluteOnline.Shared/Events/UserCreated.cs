using Avro;
using Avro.Generic;
using Avro.Specific;

namespace SaluteOnline.Shared.Events
{
    public class UserCreated : ISpecificRecord
    {
        // ReSharper disable once InconsistentNaming
        public static Schema _SCHEMA = Schema.Parse("{\"type\":\"record\",\"name\":\"UserCreated\",\"namespace\":\"SaluteOnline.Shared.Event" +
                "s\",\"fields\":[{\"name\":\"SubjectId\",\"doc\":\"ID of subject\",\"type\":[\"null\",\"string\"]}" +
                ",{\"name\":\"UserId\",\"doc\":\"ID of user\",\"type\":\"long\"}]}");

        public string SubjectId { get; set; }
        public long UserId { get; set; }

        public virtual Schema Schema => _SCHEMA;

        public object Get(int fieldPos)
        {
            switch (fieldPos)
            {
                case 0: return SubjectId;
                case 1: return UserId;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
            }
        }

        public void Put(int fieldPos, object fieldValue)
        {
            switch (fieldPos)
            {
                case 0: SubjectId = (string)fieldValue; break;
                case 1: UserId = (long)fieldValue; break;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
            };
        }
    }
}
